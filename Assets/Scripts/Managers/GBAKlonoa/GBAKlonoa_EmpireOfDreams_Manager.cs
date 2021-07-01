using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class GBAKlonoa_EmpireOfDreams_Manager : BaseGameManager
    {
        public const int CellSize = GBAConstants.TileSize;
        public const string GetROMFilePath = "ROM.gba";

        public const int FixCount = 0x0D;

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Enumerable.Range(1, 6).Select(w => new GameInfo_World(w, Enumerable.Range(0, 9).ToArray())).ToArray());

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            //await GenerateLevelObjPalettesAsync(context.GetR1Settings());
            //return null;
            var rom = FileFactory.Read<GBAKlonoa_EmpireOfDreams_ROM>(GetROMFilePath, context);
            var settings = context.GetR1Settings();
            var globalLevelIndex = (settings.World - 1) * 9 + settings.Level;
            var normalLevelIndex = (settings.World - 1) * 7 + (settings.Level - 1);
            var isMap = settings.Level == 0;
            var isBoss = settings.Level == 8;

            //GenerateAnimSetTable(context, rom);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var pal_8 = Util.ConvertGBAPalette(rom.MapPalettes[globalLevelIndex]);
            var pal_4 = Util.ConvertAndSplitGBAPalette(rom.MapPalettes[globalLevelIndex]);

            var maps = Enumerable.Range(0, 3).Select(mapIndex =>
            {
                var width = rom.MapWidths[globalLevelIndex].Widths[mapIndex];
                var is8Bit = mapIndex == 2;
                var map = rom.Maps[globalLevelIndex].Maps[mapIndex];

                Func<int, Color[]> getPalFunc = null;

                if (!is8Bit)
                {
                    Dictionary<int, int> tilesetPalettes = new Dictionary<int, int>();

                    foreach (var m in map)
                    {
                        if (!tilesetPalettes.ContainsKey(m.TileMapY))
                            tilesetPalettes[m.TileMapY] = m.PaletteIndex;
                        else if (tilesetPalettes[m.TileMapY] != m.PaletteIndex && m.TileMapY != 0)
                            Debug.LogWarning($"Tile {m.TileMapY} has several possible palettes: {tilesetPalettes[m.TileMapY]} - {m.PaletteIndex}");
                    }

                    getPalFunc = x => pal_4[tilesetPalettes.TryGetItem(x)];
                }

                var imgData = rom.TileSets[globalLevelIndex].TileSets[mapIndex];

                // First map can use tiles from the second tileset as well
                if (mapIndex == 0)
                    imgData = imgData.Concat(new byte[512 * 0x20 - imgData.Length]).Concat(rom.TileSets[globalLevelIndex].TileSets[1]).ToArray();

                return new Unity_Map
                {
                    Width = width,
                    Height = (ushort)(map.Length / width),
                    TileSet = new Unity_TileSet[]
                    {
                        new Unity_TileSet(Util.ToTileSetTexture(
                            imgData: imgData,
                            pal: pal_8,
                            encoding: is8Bit ? Util.TileEncoding.Linear_8bpp : Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize, 
                            flipY: false, 
                            getPalFunc: getPalFunc), CellSize)
                    },
                    MapTiles = map.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                };
            }).ToArray();

            Controller.DetailedState = "Loading objects";
            await Controller.WaitIfNecessary();

            var allAnimSets = rom.FixObjectGraphics.Concat(rom.LevelObjectGraphics[globalLevelIndex]);
            var allOAMCollections = rom.FixObjectOAMCollections.Concat(rom.LevelObjectOAMCollections[globalLevelIndex]).ToArray();
            var objPal = Util.ConvertAndSplitGBAPalette(rom.GetLevelObjPal(globalLevelIndex));

            var fixObjects = rom.FixObjects;

            GBAKlonoa_LoadedObject[][] levelObjects;
            
            if (isMap)
            {
                levelObjects = new GBAKlonoa_LoadedObject[][]
                {
                    rom.WorldMapObjectCollection.Objects.Select((x, index) => x.ToLoadedObject((short)(FixCount + index))).ToArray()
                };
            }
            else
            {
                levelObjects = Enumerable.Range(0, 5).Select(sector => rom.LevelObjectCollection.Objects.Select((obj, index) => obj.ToLoadedObject((short)(FixCount + index), sector)).ToArray()).ToArray();
            }

            var objmanager = new Unity_ObjectManager_GBAKlonoa(
                context: context,
                animSets: LoadAnimSets(
                    context: context, 
                    animSets: allAnimSets, 
                    oamCollections: allOAMCollections, 
                    palettes: objPal, 
                    objects: fixObjects.Concat(levelObjects[0]).ToArray(),
                    levelTextSpritePointer: isMap || isBoss ? null : rom.LevelTextSpritePointers[normalLevelIndex]));

            var objects = new List<Unity_Object>();

            // If we're in an actual level we add Klonoa to each defined start position
            if (!isMap)
            {
                GBAKlonoa_LevelStartInfo[] startInfos;

                // Boss levels have a separate array with a single entry
                if (isBoss)
                {
                    startInfos = new GBAKlonoa_LevelStartInfo[]
                    {
                        rom.BossLevelStartInfos[settings.World - 1]
                    };
                }
                else
                {
                    startInfos = rom.LevelStartInfos[normalLevelIndex].StartInfos;
                }

                // Add Klonoa at each start position
                objects.AddRange(startInfos.Select(x => new Unity_Object_GBAKlonoa(objmanager, new GBAKlonoa_LoadedObject(0, 0, x.XPos, x.YPos, 0, 0, 0, 0, 0x6e), x, allOAMCollections[0])));
            }

            // Add fixed objects, except Klonoa (first one)
            objects.AddRange(fixObjects.Skip(1).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, null, allOAMCollections[x.OAMIndex])));

            // Add level objects, duplicating them for each sector they appear in (if flags is 28 we assume it's unused in the sector)
            objects.AddRange(levelObjects.SelectMany(x => x).Where(x => isMap || x.Value_8 != 28).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, (BinarySerializable)x.LevelObj ?? x.WorldMapObj, allOAMCollections[x.OAMIndex])));
            
            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: objects,
                cellSize: CellSize,
                defaultLayer: 2,
                isometricData: isMap ? Unity_IsometricData.Mode7(CellSize) : null);
        }

        public Texture2D[] GetAnimFrames(GBAKlonoa_AnimationFrame[] frames, GBAKlonoa_ObjectOAMCollection oamCollection, Color[][] palettes)
        {
            var output = new Texture2D[frames.Length];

            var frameCache = new Dictionary<GBAKlonoa_AnimationFrame, Texture2D>();

            var shapes = GBAConstants.SpriteShapes;

            for (int frameIndex = 0; frameIndex < frames.Length; frameIndex++)
            {
                GBAKlonoa_AnimationFrame frame = frames[frameIndex];

                if (!frameCache.ContainsKey(frame))
                {
                    var spriteRects = oamCollection.OAMs.Select(x =>
                        new RectInt(x.XPos - oamCollection.OAMs[0].XPos, x.YPos - oamCollection.OAMs[0].YPos, shapes[x.Shape].Width, shapes[x.Shape].Height)).ToArray();

                    var tex = TextureHelpers.CreateTexture2D(spriteRects.Max(x => x.xMax), spriteRects.Max(x => x.yMax));

                    var tileIndex = 0;

                    for (int spriteIndex = 0; spriteIndex < oamCollection.Count; spriteIndex++)
                    {
                        var oam = oamCollection.OAMs[spriteIndex];
                        var shape = shapes[oam.Shape];

                        for (int y = 0; y < shape.Height; y += CellSize)
                        {
                            for (int x = 0; x < shape.Width; x += CellSize)
                            {
                                tex.FillInTile(
                                    imgData: frame.ImgData,
                                    imgDataOffset: tileIndex * 0x20,
                                    pal: palettes.ElementAtOrDefault(oam.PaletteIndex) ?? Util.CreateDummyPalette(16).Select(c => c.GetColor()).ToArray(),
                                    encoding: Util.TileEncoding.Linear_4bpp,
                                    tileWidth: CellSize,
                                    flipTextureY: true,
                                    flipTextureX: false,
                                    tileX: x + spriteRects[spriteIndex].x,
                                    tileY: y + spriteRects[spriteIndex].y);

                                tileIndex++;
                            }
                        }
                    }

                    tex.Apply();

                    frameCache.Add(frame, tex);
                }

                output[frameIndex] = frameCache[frame];
            }

            return output;
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet LoadAnimSet(GBAKlonoa_ObjectGraphics objectGraphics, GBAKlonoa_ObjectOAMCollection oamCollection, Color[][] palettes)
        {
            if (oamCollection.Count != 1)
                Debug.LogWarning($"Animation uses multiple sprites!");

            return new Unity_ObjectManager_GBAKlonoa.AnimSet(
                animations: Enumerable.Range(0, objectGraphics.Animations.Length).
                    Select(animIndex => objectGraphics.Animations[animIndex]?.Frames?.Length == 0 ? null : new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(
                        animFrameFunc: () => GetAnimFrames(objectGraphics.Animations[animIndex].Frames, oamCollection, palettes).Select(x => x.CreateSprite()).ToArray(), 
                        oamCollection: oamCollection)).ToArray(), 
                displayName: $"{objectGraphics.AnimationsPointer}", 
                oamCollections: new GBAKlonoa_ObjectOAMCollection[]
                {
                    oamCollection
                });
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet[] LoadAnimSets(Context context, IEnumerable<GBAKlonoa_ObjectGraphics> animSets, GBAKlonoa_ObjectOAMCollection[] oamCollections, Color[][] palettes, GBAKlonoa_LoadedObject[] objects, Pointer levelTextSpritePointer)
        {
            var loadedAnimSetsDictionary = new Dictionary<Pointer, Unity_ObjectManager_GBAKlonoa.AnimSet>();
            var loadedAnimSets = new List<Unity_ObjectManager_GBAKlonoa.AnimSet>();

            // Start by adding a null entry to use as default
            loadedAnimSets.Add(new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[0], $"NULL", new GBAKlonoa_ObjectOAMCollection[0]));

            foreach (var animSet in animSets)
            {
                var oam = oamCollections.ElementAtOrDefault(objects.ElementAtOrDefault(animSet.ObjIndex)?.OAMIndex ?? -1);

                if (animSet.AnimationsPointer == null || oam == null) 
                    continue;
                
                if (!loadedAnimSetsDictionary.ContainsKey(animSet.AnimationsPointer))
                {
                    loadedAnimSetsDictionary[animSet.AnimationsPointer] = LoadAnimSet(animSet, oam, palettes);
                    loadedAnimSets.Add(loadedAnimSetsDictionary[animSet.AnimationsPointer]);
                }
                else
                {
                    loadedAnimSetsDictionary[animSet.AnimationsPointer].OAMCollections.Add(oam);
                }
            }

            var s = context.Deserializer;
            var file = context.GetFile(GetROMFilePath);
            var specialAnimIndex = 0;
            var shapes = GBAConstants.SpriteShapes;

            IEnumerable<SpecialAnimation> specialAnims = SpecialAnimations;

            // Manually append the level text to the special animations
            if (levelTextSpritePointer != null)
                specialAnims = specialAnims.Append(new SpecialAnimation(new long[]
                {
                    levelTextSpritePointer.AbsoluteOffset
                }, true, 12));

            // Load special animations. These are all manually loaded into VRAM by the game.
            foreach (var specialAnim in specialAnims)
            {
                // If a group count is specified we read x sprites one after another assumed to be stored in VRAM in the same order
                if (specialAnim.GroupCount != null)
                {
                    var oams = objects.Where(x => specialAnim.IsFix && x.Index == specialAnim.Index || !specialAnim.IsFix && x.ObjType == specialAnim.Index).Select(x => oamCollections[x.OAMIndex]).ToArray();

                    if (!oams.Any())
                        continue;

                    // We assume each object only has a single sprite (OAM)
                    int tileIndex = oams.Min(x => x.OAMs[0].TileIndex);

                    // We assume every animation has a single frame
                    var offset = specialAnim.FrameOffsets[0];

                    for (int groupSprite = 0; groupSprite < specialAnim.GroupCount.Value; groupSprite++)
                    {
                        var oam = oams.First(x => x.OAMs[0].TileIndex == tileIndex);

                        loadSpecialAnimation(new GBAKlonoa_ObjectOAMCollection[]
                        {
                            oam
                        }, null, new long[]
                        {
                            offset
                        }, specialAnim.FramesCount);

                        var tilesCount = (shapes[oam.OAMs[0].Shape].Width / CellSize) * (shapes[oam.OAMs[0].Shape].Height / CellSize);
                        tileIndex += tilesCount;
                        offset += tilesCount * 0x20;
                    }
                }
                else
                {
                    // Find an object which uses this animation
                    var oams = objects.Where(x => specialAnim.IsFix && x.Index == specialAnim.Index || !specialAnim.IsFix && x.ObjType == specialAnim.Index).Select(x => oamCollections[x.OAMIndex]).ToArray();

                    if (!oams.Any())
                        continue;

                    loadSpecialAnimation(oams, specialAnim.Offset, specialAnim.FrameOffsets, specialAnim.FramesCount);
                }

                void loadSpecialAnimation(IReadOnlyList<GBAKlonoa_ObjectOAMCollection> oams, long? offset, IReadOnlyList<long> frameOffsets, int framesCount)
                {
                    var imgDataLength = oams[0].OAMs.Select(o => (shapes[o.Shape].Width / CellSize) * (shapes[o.Shape].Height / CellSize) * 0x20).Sum();
                    var animsPointer = new Pointer(offset ?? frameOffsets[0], file);

                    Pointer[] framePointers;

                    if (offset != null)
                        framePointers = s.DoAt(animsPointer, () => s.SerializePointerArray(null, framesCount, name: $"SpecialAnimations[{specialAnimIndex}]"));
                    else
                        framePointers = frameOffsets.Select(p => new Pointer(p, file)).ToArray();

                    var frames = new GBAKlonoa_AnimationFrame[framePointers.Length];

                    for (int frameIndex = 0; frameIndex < framePointers.Length; frameIndex++)
                    {
                        var imgData = s.DoAt(framePointers[frameIndex], () => s.SerializeArray<byte>(null, imgDataLength, name: $"SpecialAnimations[{specialAnimIndex}][{frameIndex}]"));

                        if (imgData == null)
                            throw new Exception($"Image data is null for special animation! Pointer: {framePointers[frameIndex]}");

                        frames[frameIndex] = new GBAKlonoa_AnimationFrame()
                        {
                            ImgData = imgData
                        };
                    }

                    var animSet = new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[]
                    {
                        new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(() => GetAnimFrames(frames, oams[0], palettes).Select(x => x.CreateSprite()).ToArray(), oams[0])
                    }, $"{animsPointer}", oams);

                    loadedAnimSets.Add(animSet);

                    specialAnimIndex++;
                }
            }

            return loadedAnimSets.ToArray();
        }

        public void GenerateAnimSetTable(Context context, GBAKlonoa_EmpireOfDreams_ROM rom)
        {
            var s = context.Deserializer;
            var file = rom.Offset.File;

            var levelObjectGraphics = new GBAKlonoa_ObjectGraphics[rom.LevelObjectGraphicsPointers.Length][];

            for (int i = 0; i < levelObjectGraphics.Length; i++)
            {
                s.DoAt(rom.LevelObjectGraphicsPointers[i], () =>
                {
                    levelObjectGraphics[i] = s.SerializeObjectArrayUntil<GBAKlonoa_ObjectGraphics>(levelObjectGraphics[i], x => x.AnimationsPointerValue == 0, () => new GBAKlonoa_ObjectGraphics(), name: $"{nameof(levelObjectGraphics)}[{i}]");
                });
            }

            var fixGraphics1 = s.DoAt(new Pointer(0x0805553c, file), () => s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(null, 9));
            var fixGraphics2 = s.DoAt(new Pointer(0x080555a8, file), () => s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(null, 9));


            var str = new StringBuilder();

            var allGraphics = levelObjectGraphics.
                SelectMany(x => x).
                Concat(fixGraphics1).
                Concat(fixGraphics2).
                Select(x => x.AnimationsPointer).
                Where(x => x != null).
                Distinct().
                OrderBy(x => x.AbsoluteOffset).
                ToArray();

            foreach (var p in allGraphics)
            {
                var index = allGraphics.FindItemIndex(x => x == p) + 1;

                long length = 1;

                if (index < allGraphics.Length)
                    length = (allGraphics[index].AbsoluteOffset - p.AbsoluteOffset) / 4;

                str.AppendLine($"new AnimSetInfo(0x{p.AbsoluteOffset:X8}, {length}),");
            }

            str.ToString().CopyToClipboard();
        }

        public async UniTask GenerateLevelObjPalettesAsync(GameSettings gameSettings)
        {
            var graphicsPalettes = new Dictionary<long, int>();
            var output = new StringBuilder();

            var hasFilledDictionary = false;

            for (int i = 0; i < 2; i++)
            {
                foreach (var world in GetLevels(gameSettings).First().Worlds)
                {
                    foreach (var level in world.Maps)
                    {
                        Controller.DetailedState = $"Processing {world.Index}-{level}";
                        await Controller.WaitIfNecessary();

                        var settings = new GameSettings(gameSettings.GameModeSelection, gameSettings.GameDirectory, world.Index, level);
                        using var context = new R1Context(settings);

                        await LoadFilesAsync(context);

                        var rom = FileFactory.Read<GBAKlonoa_EmpireOfDreams_ROM>(GetROMFilePath, context);

                        if (settings.Level == 0)
                            continue;

                        var globalLevelIndex = (settings.World - 1) * 9 + settings.Level;
                        var normalLevelIndex = (settings.World - 1) * 7 + (settings.Level - 1);

                        var objGraphics = rom.LevelObjectGraphics[globalLevelIndex];
                        var objects = rom.LevelObjectCollection.Objects;
                        var objOAM = rom.LevelObjectOAMCollections[globalLevelIndex];

                        var objPalIndices = rom.GetLevelObjPalIndices(globalLevelIndex);

                        try
                        {
                            if (!hasFilledDictionary)
                            {
                                if (objPalIndices.Length == 0)
                                    continue;

                                foreach (var graphic in objGraphics)
                                {
                                    if (graphic.AnimationsPointer == null)
                                        continue;

                                    if (graphicsPalettes.ContainsKey(graphic.AnimationsPointer.AbsoluteOffset))
                                        continue;

                                    var obj = objects[graphic.ObjIndex - FixCount];
                                    var oam = objOAM[obj.OAMIndex - FixCount];
                                    var palIndex = oam.OAMs[0].PaletteIndex;

                                    // Ignore fix palette
                                    if (palIndex < 2)
                                        continue;

                                    var globalPalIndex = objPalIndices[palIndex - 2];
                                    graphicsPalettes.Add(graphic.AnimationsPointer.AbsoluteOffset, globalPalIndex);
                                }

                                foreach (var specialAnim in SpecialAnimations.Where(x => !x.IsFix))
                                {
                                    if (graphicsPalettes.ContainsKey(specialAnim.Offset ?? specialAnim.FrameOffsets[0]))
                                        continue;

                                    var obj = objects.FirstOrDefault(x => x.ObjType == specialAnim.Index);

                                    if (obj == null)
                                        continue;

                                    var oam = objOAM[obj.OAMIndex - FixCount];
                                    var palIndex = oam.OAMs[0].PaletteIndex;

                                    // Ignore fix palette
                                    if (palIndex < 2)
                                        continue;

                                    var globalPalIndex = objPalIndices[palIndex - 2];
                                    graphicsPalettes.Add(specialAnim.Offset ?? specialAnim.FrameOffsets[0], globalPalIndex);
                                }
                            }
                            else
                            {
                                var palIndices = new int[objOAM.SelectMany(x => x.OAMs).Max(x => x.PaletteIndex) - 2 + 1];

                                for (int j = 0; j < palIndices.Length; j++)
                                    palIndices[j] = -1;

                                foreach (var graphic in objGraphics)
                                {
                                    if (graphic.AnimationsPointer == null)
                                        continue;

                                    if (!graphicsPalettes.ContainsKey(graphic.AnimationsPointer.AbsoluteOffset))
                                        continue;

                                    var globalPalIndex = graphicsPalettes[graphic.AnimationsPointer.AbsoluteOffset];

                                    var obj = objects[graphic.ObjIndex - FixCount];
                                    var oam = objOAM[obj.OAMIndex - FixCount];
                                    var palIndex = oam.OAMs[0].PaletteIndex;

                                    palIndices[palIndex - 2] = globalPalIndex;
                                }

                                foreach (var specialAnim in SpecialAnimations.Where(x => !x.IsFix))
                                {
                                    if (!graphicsPalettes.ContainsKey(specialAnim.Offset ?? specialAnim.FrameOffsets[0]))
                                        continue;

                                    var globalPalIndex = graphicsPalettes[specialAnim.Offset ?? specialAnim.FrameOffsets[0]];

                                    var obj = objects.FirstOrDefault(x => x.ObjType == specialAnim.Index);

                                    if (obj == null)
                                        continue;

                                    var oam = objOAM[obj.OAMIndex - FixCount];
                                    var palIndex = oam.OAMs[0].PaletteIndex;

                                    palIndices[palIndex - 2] = globalPalIndex;
                                }

                                output.AppendLine($"{globalLevelIndex} => new int[] {{ {String.Join(", ", palIndices)} }},");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error at level {world.Index}-{level}: {ex.Message}{Environment.NewLine}{ex}");
                        }
                    }
                }

                if (!hasFilledDictionary)
                {
                    var str = new StringBuilder();

                    foreach (var g in graphicsPalettes)
                    {
                        str.AppendLine($"0x{g.Key} -> {g.Value}");
                    }

                    str.ToString().CopyToClipboard();
                }

                hasFilledDictionary = true;
            }

            output.ToString().CopyToClipboard();
        }

        public override async UniTask LoadFilesAsync(Context context) => await context.AddMemoryMappedFile(GetROMFilePath, GBAConstants.Address_ROM);

        // TODO: Support EU and JP versions
        public AnimSetInfo[] AnimSetInfos => new AnimSetInfo[]
        {
            new AnimSetInfo(0x0818958C, 19),
            new AnimSetInfo(0x081895D8, 9),
            new AnimSetInfo(0x081895FC, 3),
            new AnimSetInfo(0x08189608, 13),
            new AnimSetInfo(0x0818963C, 12),
            new AnimSetInfo(0x0818966C, 19),
            new AnimSetInfo(0x081896B8, 28),
            new AnimSetInfo(0x08189728, 38),
            new AnimSetInfo(0x081897C0, 72),
            new AnimSetInfo(0x081898E0, 4),
            new AnimSetInfo(0x081898F0, 2),
            new AnimSetInfo(0x081898F8, 2),
            new AnimSetInfo(0x08189900, 1),
            new AnimSetInfo(0x08189904, 3),
            new AnimSetInfo(0x08189910, 2),
            new AnimSetInfo(0x08189918, 3),
            new AnimSetInfo(0x08189924, 1),
            new AnimSetInfo(0x08189928, 3),
            new AnimSetInfo(0x08189934, 2),
            new AnimSetInfo(0x0818993C, 3),
            new AnimSetInfo(0x08189948, 1),
            new AnimSetInfo(0x0818994C, 1),
            new AnimSetInfo(0x08189950, 2),
            new AnimSetInfo(0x08189958, 1),
            new AnimSetInfo(0x0818995C, 3),
            new AnimSetInfo(0x08189968, 3),
            new AnimSetInfo(0x08189974, 3),
            new AnimSetInfo(0x08189980, 3),
            new AnimSetInfo(0x0818998C, 2),
            new AnimSetInfo(0x08189994, 1),
            new AnimSetInfo(0x08189998, 1),
            new AnimSetInfo(0x0818999C, 3),
            new AnimSetInfo(0x081899A8, 1),
            new AnimSetInfo(0x081899AC, 19),
            new AnimSetInfo(0x081899F8, 7),
            new AnimSetInfo(0x08189A14, 1),
            new AnimSetInfo(0x08189A18, 1),
            new AnimSetInfo(0x08189A1C, 1),
            new AnimSetInfo(0x08189A20, 1),
        };

        // NOTE: These animations are handled by the game manually copying the image data to VRAM, either in a global function or in one of the
        //       level load functions. For simplicity we match them with the objects based on index if fix or object type if not. Hopefully this
        //       will always work, assuming that every object of the same type will always have the same graphics. The way the game does this is
        //       by having each object specify a tile index in VRAM (the OAM data) for what it should display. This allows each object to display
        //       different graphics. Problem is we don't know what the tile index corresponds to as the VRAM is allocated differently for each
        //       level and it is almost entirely hard-coded in the level load functions.
        public SpecialAnimation[] SpecialAnimations => new SpecialAnimation[]
        {
            // Fix
            new SpecialAnimation(new long[]
            {
                0x0805c8e8,
                0x0809ac08
            }, true, 9), // Klonoa's attack
            new SpecialAnimation(new long[]
            {
                0x0805c968,
                0x0809ac88,
            }, true, 10), // Klonoa's attack (small)
            new SpecialAnimation(new long[]
            {
                0x0805c9e8,
            }, true, 11), // Level text (VISION)
            
            // Collectibles
            new SpecialAnimation(0x0818b9b8, 4, false, 44), // Green gem
            new SpecialAnimation(0x0818b9c8, 4, false, 45), // Blue gem
            new SpecialAnimation(0x0818b9d8, 4, false, 7), // Heart
            new SpecialAnimation(0x0818b9e8, 4, false, 3), // Star

            // Other
            new SpecialAnimation(0x0805f508, false, 111), // Block
            new SpecialAnimation(0x0805f708, false, 1), // Red key
            new SpecialAnimation(0x0805f788, false, 2), // Blue key
            new SpecialAnimation(0x0805f808, false, 5), // Locked door
            new SpecialAnimation(0x0805fb08, false, 64), // Ramp
            new SpecialAnimation(0x08060608, false, 39), // Vertical platform
            new SpecialAnimation(0x08060708, false, 41), // Horizontal platform

            // Boss fight 1?

            new SpecialAnimation(0x08061c28, false, 42), // Fence
            new SpecialAnimation(0x08061d28, false, 9), // Leaf
            new SpecialAnimation(0x08061d28, false, 10, groupCount: 5), // Leaves
            new SpecialAnimation(0x08061fc8, false, 114), // Red switch
            new SpecialAnimation(0x08062048, false, 60), // Blocked door
            new SpecialAnimation(0x08062148, false, 59), // One-way door
            new SpecialAnimation(0x08062248, false, 43), // Blue platform
            new SpecialAnimation(0x08062348, false, 62), // Weight platform top
            new SpecialAnimation(0x080623c8, false, 61), // Weight platform spring
            new SpecialAnimation(0x080627c8, false, 115), // Rotation switch
        };

        public class AnimSetInfo
        {
            public AnimSetInfo(long offset, int animCount)
            {
                Offset = offset;
                AnimCount = animCount;
            }

            public long Offset { get; }
            public int AnimCount { get; }
        }

        public class SpecialAnimation
        {
            public SpecialAnimation(long framesArrayOffset, int framesCount, bool isFix, int index)
            {
                Offset = framesArrayOffset;
                FramesCount = framesCount;
                IsFix = isFix;
                Index = index;
            }
            public SpecialAnimation(long frameOffset, bool isFix, int index, int? groupCount = null)
            {
                FrameOffsets = new long[]
                {
                    frameOffset
                };
                IsFix = isFix;
                Index = index;
                GroupCount = groupCount;
            }
            public SpecialAnimation(long[] frameOffsets, bool isFix, int index)
            {
                FrameOffsets = frameOffsets;
                IsFix = isFix;
                Index = index;
            }

            /// <summary>
            /// The offset for every frame in the animation
            /// </summary>
            public long[] FrameOffsets { get; }

            /// <summary>
            /// If specified this is the offset to the frames pointer array, otherwise <see cref="FrameOffsets"/> should be set
            /// </summary>
            public long? Offset { get; }

            /// <summary>
            /// The amount of frames in the animation if <see cref="Offset"/> is specified
            /// </summary>
            public int FramesCount { get; }

            /// <summary>
            /// Indicates if the animation is for a fix object, otherwise it's for a level object
            /// </summary>
            public bool IsFix { get; }

            /// <summary>
            /// If <see cref="IsFix"/> this is the object index (0-12), otherwise it's the object type
            /// </summary>
            public int Index { get; }

            /// <summary>
            /// If specified this indicates how many sprites are used for this group of sprites. A very hacky solution to the wind leaves.
            /// </summary>
            public int? GroupCount { get; }
        }
    }
}