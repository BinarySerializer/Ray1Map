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
    public class GBAKlonoa_Manager : BaseGameManager
    {
        public const int CellSize = GBAConstants.TileSize;
        public const string GetROMFilePath = "ROM.gba";

        public const int FixCount = 0x0D;

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Enumerable.Range(1, 6).Select(w => new GameInfo_World(w, Enumerable.Range(0, 9).ToArray())).ToArray());

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            var rom = FileFactory.Read<GBAKlonoa_ROM>(GetROMFilePath, context);
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

                return new Unity_Map
                {
                    Width = width,
                    Height = (ushort)(map.Length / width),
                    TileSet = new Unity_TileSet[]
                    {
                        new Unity_TileSet(Util.ToTileSetTexture(
                            imgData: rom.TileSets[globalLevelIndex].TileSets[mapIndex],
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
                objects.AddRange(startInfos.Select(x => new Unity_Object_GBAKlonoa(objmanager, new GBAKlonoa_LoadedObject(0, 0, x.XPos, x.YPos, 0, 0, 0, 0, 0x6e), null, allOAMCollections[0])));
            }

            // Add fixed objects, except Klonoa (first one)
            objects.AddRange(fixObjects.Skip(1).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, null, allOAMCollections[x.OAMIndex])));

            // Add level objects, duplicating them for each sector they appear in (if flags is 28 we assume it's unused in the sector)
            objects.AddRange(levelObjects.SelectMany(x => x).Where(x => x.Value_8 != 28).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, x.LevelObj, allOAMCollections[x.OAMIndex])));
            
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
                oamCollection: oamCollection);
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet[] LoadAnimSets(Context context, IEnumerable<GBAKlonoa_ObjectGraphics> animSets, GBAKlonoa_ObjectOAMCollection[] oamCollections, Color[][] palettes, GBAKlonoa_LoadedObject[] objects, Pointer levelTextSpritePointer)
        {
            var loadedAnimSetsDictionary = new Dictionary<Pointer, Unity_ObjectManager_GBAKlonoa.AnimSet>();
            var loadedAnimSets = new List<Unity_ObjectManager_GBAKlonoa.AnimSet>();

            // Start by adding a null entry to use as default
            loadedAnimSets.Add(new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[0], $"NULL", null));

            foreach (var animSet in animSets)
            {
                var oam = oamCollections[objects[animSet.ObjIndex].OAMIndex];

                if (animSet.AnimationsPointer == null) 
                    continue;
                
                if (!loadedAnimSetsDictionary.ContainsKey(animSet.AnimationsPointer))
                {
                    loadedAnimSetsDictionary[animSet.AnimationsPointer] = LoadAnimSet(animSet, oam, palettes);
                    loadedAnimSets.Add(loadedAnimSetsDictionary[animSet.AnimationsPointer]);
                }

                loadedAnimSetsDictionary[animSet.AnimationsPointer].ObjIndices.Add(animSet.ObjIndex);
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
                // Find an object which uses this animation
                var obj = objects.FirstOrDefault(x => specialAnim.IsFix && x.Index == specialAnim.Index || !specialAnim.IsFix && x.ObjType == specialAnim.Index);

                if (obj == null)
                    continue;

                var oam = oamCollections[obj.OAMIndex];
                var imgDataLength = oam.OAMs.Select(o => (shapes[o.Shape].Width / CellSize) * (shapes[o.Shape].Height / CellSize) * 0x20).Sum();
                var animsPointer = new Pointer(specialAnim.Offset ?? specialAnim.FrameOffsets[0], file);

                Pointer[] framePointers;

                if (specialAnim.Offset != null)
                    framePointers = s.DoAt(animsPointer, () => s.SerializePointerArray(null, specialAnim.FramesCount, name: $"SpecialAnimations[{specialAnimIndex}]"));
                else
                    framePointers = specialAnim.FrameOffsets.Select(p => new Pointer(p, file)).ToArray();

                var frames = new GBAKlonoa_AnimationFrame[framePointers.Length];

                for (int frameIndex = 0; frameIndex < framePointers.Length; frameIndex++)
                {
                    var imgData = s.DoAt(framePointers[frameIndex], () => s.SerializeArray<byte>(null, imgDataLength, name: $"SpecialAnimations[{specialAnimIndex}][{frameIndex}]"));
                    
                    frames[frameIndex] = new GBAKlonoa_AnimationFrame()
                    {
                        ImgData = imgData
                    };
                }

                var animSet = new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[]
                {
                    new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(() => GetAnimFrames(frames, oam, palettes).Select(x => x.CreateSprite()).ToArray(), oam)
                }, $"{animsPointer}", oam);

                if (specialAnim.IsFix)
                    animSet.ObjIndices.Add(specialAnim.Index);
                else
                    animSet.ObjTypeIndices.Add(specialAnim.Index);

                loadedAnimSets.Add(animSet);

                specialAnimIndex++;
            }

            return loadedAnimSets.ToArray();
        }

        public void GenerateAnimSetTable(Context context, GBAKlonoa_ROM rom)
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
            public SpecialAnimation(long offset, int framesCount, bool isFix, int index)
            {
                Offset = offset;
                FramesCount = framesCount;
                IsFix = isFix;
                Index = index;
            }
            public SpecialAnimation(long[] frameOffsets, bool isFix, int index)
            {
                FrameOffsets = frameOffsets;
                IsFix = isFix;
                Index = index;
            }

            public long[] FrameOffsets { get; }
            public long? Offset { get; }
            public int FramesCount { get; }
            public bool IsFix { get; }
            public int Index { get; } // If fix this is the object index, otherwise it's the object type
        }
    }
}