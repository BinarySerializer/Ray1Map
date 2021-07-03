using BinarySerializer;
using BinarySerializer.GBA;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class GBAKlonoa_EmpireOfDreams_Manager : BaseGameManager
    {
        public const int CellSize = GBAConstants.TileSize;
        public const string GetROMFilePath = "ROM.gba";

        public const string CompressedObjTileBlockName = "CompressedObjTileBlock";

        public const int FixCount = 0x0D; // Fixed objects loaded into every level
        public const int NormalWorldsCount = 6; // World 0 is reserved for either menus or cutscenes, so normal worlds are 1-6
        public const int LevelsCount = NormalWorldsCount * 9; // Map + 7 levels + boss
        public const int NormalLevelsCount = NormalWorldsCount * 7; // 7 levels

        public static int GetGlobalLevelIndex(int world, int level) => (world - 1) * 9 + level;
        public static int GetNormalLevelIndex(int world, int level) => (world - 1) * 7 + (level - 1);
        public static int GetCompressedMapOrBossBlockIndex(int world, int level) => ((level << 0x18) >> 0x1b) * 6 + -1 + world;

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Enumerable.Range(1, 6).Select(w => new GameInfo_World(w, Enumerable.Range(0, 9).ToArray())).ToArray());

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            //await GenerateLevelObjPalettesAsync(context.GetR1Settings());
            //return null;
            var rom = FileFactory.Read<GBAKlonoa_EmpireOfDreams_ROM>(GetROMFilePath, context);
            var settings = context.GetR1Settings();
            var globalLevelIndex = GetGlobalLevelIndex(settings.World, settings.Level);
            var normalLevelIndex = GetNormalLevelIndex(settings.World, settings.Level);
            var isMap = settings.Level == 0;
            var isBoss = settings.Level == 8;

            //GenerateAnimSetTable(context, rom);

            Controller.DetailedState = "Loading maps";
            await Controller.WaitIfNecessary();

            var maps = Enumerable.Range(0, 3).Select(mapIndex =>
            {
                var width = rom.MapWidths[globalLevelIndex].Widths[mapIndex];
                var is8Bit = mapIndex == 2;
                var map = rom.Maps[globalLevelIndex].Maps[mapIndex];

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
                        LoadTileSet(imgData, rom.MapPalettes[globalLevelIndex], is8Bit, map)
                    },
                    MapTiles = map.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
                    Settings3D = isMap &&  mapIndex == 2 ? Unity_Map.FreeCameraSettings.Mode7 : null,
                };
            }).ToArray();

            // Add line collision for the sector bounds
            var collisionLines = new List<Unity_CollisionLine>();

            if (!isMap && !isBoss)
            {
                var sectorIndex = 0;

                foreach (var sector in rom.MapSectors[normalLevelIndex].Sectors)
                {
                    var color = new Color(1, 0.92f - (0.1f * sectorIndex), 0.016f + (0.15f * sectorIndex));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X, sector.Y), 
                        new Vector2(sector.X, sector.Y + sector.Height), 
                        color));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X, sector.Y + sector.Height), 
                        new Vector2(sector.X + sector.Width, sector.Y + sector.Height),
                        color));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X + sector.Width, sector.Y),
                        new Vector2(sector.X + sector.Width, sector.Y + sector.Height),
                        color));

                    collisionLines.Add(new Unity_CollisionLine(
                        new Vector2(sector.X, sector.Y),
                        new Vector2(sector.X + sector.Width, sector.Y),
                        color));

                    sectorIndex++;
                }
            }

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
                // Get the objects for every sector (except for in bosses)
                levelObjects = Enumerable.Range(0, isBoss ? 1 : 5).Select(sector => rom.LevelObjectCollection.Objects.Select((obj, index) => obj.ToLoadedObject((short)(FixCount + index), sector)).ToArray()).ToArray();
            }

            var firstLoadedObjects = new List<GBAKlonoa_LoadedObject>();

            firstLoadedObjects.AddRange(fixObjects);

            for (int lvlObjIndex = 0; lvlObjIndex < levelObjects[0].Length; lvlObjIndex++)
                firstLoadedObjects.Add(levelObjects.Select(x => x[lvlObjIndex]).FirstOrDefault(x => isMap || x.Value_8 != 28 ));

            var objmanager = new Unity_ObjectManager_GBAKlonoa(
                context: context,
                animSets: LoadAnimSets(
                    context: context, 
                    rom: rom,
                    animSets: allAnimSets, 
                    oamCollections: allOAMCollections, 
                    palettes: objPal, 
                    objects: firstLoadedObjects,
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
                    startInfos = new GBAKlonoa_LevelStartInfo[0].
                        Append(rom.LevelStartInfos[normalLevelIndex].StartInfo_Entry).
                        Concat(rom.LevelStartInfos[normalLevelIndex].StartInfos_Yellow).
                        Concat(rom.LevelStartInfos[normalLevelIndex].StartInfos_Green).
                        ToArray();
                }

                // Add Klonoa at each start position
                objects.AddRange(startInfos.Select(x => new Unity_Object_GBAKlonoa(objmanager, new GBAKlonoa_LoadedObject(0, 0, x.XPos, x.YPos, 0, 0, 0, 0, 0x6e), x, allOAMCollections[0])));
            }

            // Add fixed objects, except Klonoa (first one) and object 11/12 for maps since it's not used
            objects.AddRange(fixObjects.Skip(isMap ? 0 : 1).Where(x => !isMap || (x.Index != 11 && x.Index != 12)).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, null, allOAMCollections[x.OAMIndex])));

            // Add level objects, duplicating them for each sector they appear in (if flags is 28 we assume it's unused in the sector)
            objects.AddRange(levelObjects.SelectMany(x => x).Where(x => isMap || x.Value_8 != 28).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, (BinarySerializable)x.LevelObj ?? x.WorldMapObj, allOAMCollections[x.OAMIndex])));

            if (isMap) 
            {
                foreach (var obj in objects) 
                {
                    var o = (Unity_Object_GBAKlonoa)obj;
                    var x = o.XPosition;
                    var y = o.YPosition;
                    var fx = maps[2].Width * CellSize;
                    var fy = maps[2].Height * CellSize;

                    var cos = Mathf.Cos(((y / 128f) + 0.5f) * Mathf.PI) * x;
                    var sin = Mathf.Sin(((y / 128f) + 0.5f) * Mathf.PI) * x;
                    o.XPosition = (short)(cos + fx / 2);
                    o.YPosition = (short)(sin + fy / 2);
                }
            }

            return new Unity_Level(
                maps: maps,
                objManager: objmanager,
                eventData: objects,
                cellSize: CellSize,
                defaultLayer: 2,
                isometricData: isMap ? Unity_IsometricData.Mode7(CellSize) : null,
                collisionLines: collisionLines.ToArray());
        }

        public Unity_TileSet LoadTileSet(byte[] tileSet, RGBA5551Color[] pal, bool is8bit, MapTile[] mapTiles_4)
        {
            Texture2D tex;
            var additionalTiles = new List<Texture2D>();
            var tileSize = is8bit ? 0x40 : 0x20;
            var paletteIndices = Enumerable.Range(0, tileSet.Length / tileSize).Select(x => new List<byte>()).ToArray();
            var tilesCount = tileSet.Length / tileSize;

            if (is8bit)
            {
                tex = Util.ToTileSetTexture(tileSet, Util.ConvertGBAPalette(pal), Util.TileEncoding.Linear_8bpp, CellSize, false);
            }
            else
            {
                var palettes = Util.ConvertAndSplitGBAPalette(pal);

                foreach (var m in mapTiles_4)
                {
                    if (m.TileMapY < paletteIndices.Length && !paletteIndices[m.TileMapY].Contains(m.PaletteIndex))
                        paletteIndices[m.TileMapY].Add(m.PaletteIndex);
                }

                tex = Util.ToTileSetTexture(tileSet, palettes[0], Util.TileEncoding.Linear_4bpp, CellSize, false, getPalFunc: x =>
                {
                    var p = paletteIndices[x].ElementAtOrDefault(0);
                    return palettes[p];
                });

                // Add additional tiles for tiles with multiple palettes
                for (int tileIndex = 0; tileIndex < paletteIndices.Length; tileIndex++)
                {
                    for (int palIndex = 1; palIndex < paletteIndices[tileIndex].Count; palIndex++)
                    {
                        var p = paletteIndices[tileIndex][palIndex];

                        var tileTex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        // Create a new tile
                        tileTex.FillInTile(
                            imgData: tileSet,
                            imgDataOffset: tileSize * tileIndex,
                            pal: palettes[p],
                            encoding: Util.TileEncoding.Linear_4bpp,
                            tileWidth: CellSize,
                            flipTextureY: false,
                            tileX: 0,
                            tileY: 0);

                        // Modify all tiles where this is used
                        foreach (MapTile t in mapTiles_4.Where(x => x.TileMapY == tileIndex && x.PaletteIndex == p))
                        {
                            t.TileMapY = (ushort)(tilesCount + additionalTiles.Count);
                        }

                        // Add to additional tiles list
                        additionalTiles.Add(tileTex);
                    }
                }
            }

            // Create the tile array
            var tiles = new Unity_TileTexture[tilesCount + additionalTiles.Count];

            // Keep track of the index
            var index = 0;

            // Add every normal tile
            for (int y = 0; y < tex.height; y += CellSize)
            {
                for (int x = 0; x < tex.width; x += CellSize)
                {
                    if (index >= tilesCount)
                        break;

                    // Create a tile
                    tiles[index++] = tex.CreateTile(new Rect(x, y, CellSize, CellSize));
                }
            }

            // Add additional tiles
            foreach (Texture2D t in additionalTiles)
                tiles[index++] = t.CreateTile();

            return new Unity_TileSet(tiles);
        }

        public Texture2D[] GetAnimFrames(GBAKlonoa_AnimationFrame[] frames, GBAKlonoa_ObjectOAMCollection oamCollection, Color[][] palettes, int imgDataOffset = 0)
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
                                    imgDataOffset: imgDataOffset + tileIndex * 0x20,
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

            var anims = objectGraphics.Animations;

            // Sometimes the last animation is null
            if (anims.Length > 1 && anims.Last() == null)
                anims = anims.Take(anims.Length - 1).ToArray();

            return new Unity_ObjectManager_GBAKlonoa.AnimSet(
                animations: Enumerable.Range(0, anims.Length).
                    Select(animIndex => anims[animIndex]?.Frames?.Length > 0 ? new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(
                        animFrameFunc: () => GetAnimFrames(anims[animIndex].Frames, oamCollection, palettes).Select(x => x.CreateSprite()).ToArray(), 
                        oamCollection: oamCollection,
                        animSpeeds: anims[animIndex]?.Frames?.Select(f => (int?)f?.Speed).ToArray()) : null).ToArray(), 
                displayName: $"0x{objectGraphics.AnimationsPointer.StringAbsoluteOffset}", 
                oamCollections: new GBAKlonoa_ObjectOAMCollection[]
                {
                    oamCollection
                });
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet[] LoadAnimSets(Context context, GBAKlonoa_EmpireOfDreams_ROM rom, IEnumerable<GBAKlonoa_ObjectGraphics> animSets, GBAKlonoa_ObjectOAMCollection[] oamCollections, Color[][] palettes, IList<GBAKlonoa_LoadedObject> objects, Pointer levelTextSpritePointer)
        {
            var settings = context.GetR1Settings();
            var s = context.Deserializer;
            var romFile = context.GetFile(GetROMFilePath);
            var globalLevelIndex = GetGlobalLevelIndex(settings.World, settings.Level);
            var isBoss = settings.Level == 8;

            var loadedAnimSetsDictionary = new Dictionary<Pointer, Unity_ObjectManager_GBAKlonoa.AnimSet>();
            var loadedAnimSets = new List<Unity_ObjectManager_GBAKlonoa.AnimSet>();

            // Start by adding a null entry to use as default
            loadedAnimSets.Add(new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[0], $"NULL", new GBAKlonoa_ObjectOAMCollection[0]));

            // Load animations declared through the graphics data
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

            // Load animations from allocated tiles. This is what the game does for practically all animations, but for simplicity we mainly do this for the maps.
            var allocationInfos = LevelVRAMAllocationInfos.TryGetItem(globalLevelIndex);

            if (allocationInfos != null)
            {
                var memFile = context.GetFile(CompressedObjTileBlockName);

                var vramTileIndex = 0;

                var allocatedVRAMData = allocationInfos.Select((allocationInfo, alloctionIndex) =>
                {
                    if (allocationInfo.TileIndex != null)
                        vramTileIndex = allocationInfo.TileIndex.Value;

                    var tileIndex = vramTileIndex;
                    var p = new Pointer(allocationInfo.Offset, allocationInfo.Offset >= GBAConstants.Address_ROM ? romFile : memFile);
                    var bytes = s.DoAt(p, () => s.SerializeArray<byte>(null, allocationInfo.Length, $"ObjTiles[{alloctionIndex}]"));

                    vramTileIndex += (allocationInfo.Length / 0x20);

                    return new
                    {
                        SourcePointer = p,
                        ImgData = bytes,
                        TileIndex = tileIndex
                    };
                }).ToArray();

                var loadedTiles = new HashSet<int>();

                foreach (var oamCollection in oamCollections)
                {
                    var oam = oamCollection.OAMs[0];

                    if (loadedTiles.Contains(oam.TileIndex))
                        continue;

                    var data = allocatedVRAMData.FirstOrDefault(x => x.TileIndex == oam.TileIndex);

                    if (data == null)
                        continue;

                    var frames = new GBAKlonoa_AnimationFrame[]
                    {
                        new GBAKlonoa_AnimationFrame()
                        {
                            ImgData = data.ImgData
                        }
                    };

                    var animSet = new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[]
                    {
                        new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation(() => GetAnimFrames(frames, oamCollection, palettes).Select(x => x.CreateSprite()).ToArray(), oamCollection)
                    }, $"0x{data.SourcePointer.StringAbsoluteOffset}", new GBAKlonoa_ObjectOAMCollection[]
                    {
                        oamCollection
                    });

                    loadedAnimSets.Add(animSet);

                    loadedTiles.Add(oam.TileIndex);
                }
            }

            // Load special (hard-coded) animations
            var specialAnimIndex = 0;
            var shapes = GBAConstants.SpriteShapes;

            IEnumerable<SpecialAnimation> specialAnims = SpecialAnimations;

            // Manually append the level text to the special animations (VISION/BOSS)
            if (isBoss)
            {
                // BOSS
                specialAnims = specialAnims.Append(new SpecialAnimation(0x080a4888, true, 11));

                // CLEAR
                specialAnims = specialAnims.Append(new SpecialAnimation(0x080a5088, true, 12));
            }
            else
            {
                // VISION
                specialAnims = specialAnims.Append(new SpecialAnimation(0x0805c9e8, true, 11));

                // Level text (1-1 etc.)
                if (levelTextSpritePointer != null)
                    specialAnims = specialAnims.Append(new SpecialAnimation(levelTextSpritePointer.AbsoluteOffset, true, 12));
            }

            // Load special animations. These are all manually loaded into VRAM by the game.
            foreach (var specialAnim in specialAnims)
            {
                // If a group count is specified we read x sprites one after another assumed to be stored in VRAM in the same order
                if (specialAnim.GroupCount != null)
                {
                    var oams = objects.Where(x => x != null && specialAnim.MatchesObject(x)).Select(x => oamCollections[x.OAMIndex]).ToArray();

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
                    // Find an object which uses this animation (filter out 0,0 objects here too since some unused objects get placed there with wrong graphics...)
                    var oams = objects.Where(x => x != null && (!(x.XPos == 0 && x.YPos == 0) || x.Index < FixCount) && specialAnim.MatchesObject(x)).Select(x => oamCollections[x.OAMIndex]).ToArray();

                    if (!oams.Any())
                        continue;

                    loadSpecialAnimation(oams, specialAnim.Offset, specialAnim.FrameOffsets, specialAnim.FramesCount);
                }

                void loadSpecialAnimation(IReadOnlyList<GBAKlonoa_ObjectOAMCollection> oams, long? offset, IReadOnlyList<long> frameOffsets, int framesCount)
                {
                    var imgDataLength = oams[0].OAMs.Select(o => (shapes[o.Shape].Width / CellSize) * (shapes[o.Shape].Height / CellSize) * 0x20).Sum();
                    var animsPointer = new Pointer(offset ?? frameOffsets[0], romFile);

                    Pointer[] framePointers;

                    if (offset != null)
                        framePointers = s.DoAt(animsPointer, () => s.SerializePointerArray(null, framesCount, name: $"SpecialAnimations[{specialAnimIndex}]"));
                    else
                        framePointers = frameOffsets.Select(p => new Pointer(p, romFile)).ToArray();

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
                    }, $"0x{animsPointer.StringAbsoluteOffset}", oams);

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

                        var globalLevelIndex = GetGlobalLevelIndex(settings.World, settings.Level);

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

                                    if (specialAnim.ObjParam_1 != null || specialAnim.ObjParam_2 != null)
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

                                    if (specialAnim.ObjParam_1 != null || specialAnim.ObjParam_2 != null)
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

            // This is actually a single set of 19 animations, but due to different palettes being used we split them up
            new AnimSetInfo(0x0818966C, 8),
            new AnimSetInfo(0x0818966C + (4 * 8), 11),

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

            // This is actually a single set of 3 animations, but due to the last two using a different shape we split them up
            new AnimSetInfo(0x0818995C, 1),
            new AnimSetInfo(0x0818995C + 4, 2),

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
            
            // Collectibles
            new SpecialAnimation(0x0818b9b8, 4, false, 44), // Green gem
            new SpecialAnimation(0x0818b9c8, 4, false, 45), // Blue gem
            new SpecialAnimation(0x0818b9d8, 4, false, 7), // Heart
            new SpecialAnimation(0x0818b9e8, 4, false, 3), // Star

            // Other
            new SpecialAnimation(0x0805f508, false, 111, objParam_1: 0), // Block
            new SpecialAnimation(0x0805f708, false, 1), // Red key
            new SpecialAnimation(0x0805f788, false, 2), // Blue key
            new SpecialAnimation(0x0805f808, false, 5), // Locked door
            new SpecialAnimation(0x0805fb08, false, 64), // Ramp
            new SpecialAnimation(0x08060608, false, 39), // Vertical platform
            new SpecialAnimation(0x08060708, false, 41), // Horizontal platform
            new SpecialAnimation(0x08061c28, false, 42), // Fence
            new SpecialAnimation(0x08061d28, false, 9), // Leaf
            new SpecialAnimation(0x08061d28, false, 10, groupCount: 5), // Leaves
            new SpecialAnimation(0x08061fc8, false, 114, objParam_2: 1), // Red switch
            new SpecialAnimation(0x08062048, false, 60), // Blocked door
            new SpecialAnimation(0x08062148, false, 59), // One-way door
            new SpecialAnimation(0x08062248, false, 43), // Blue platform
            new SpecialAnimation(0x08062348, false, 62), // Weight platform top
            new SpecialAnimation(0x080623c8, false, 61), // Weight platform spring
            new SpecialAnimation(0x080627c8, false, 115), // Rotation switch
            new SpecialAnimation(0x08062848, false, 4), // Green key
            new SpecialAnimation(0x08062ae8, false, 112), // Breakable door
            new SpecialAnimation(0x08062ee8, false, 111, objParam_1: 2), // Up block
            new SpecialAnimation(0x080630e8, false, 55), // Weight switch
            new SpecialAnimation(0x08063168, false, 111, objParam_1: 4), // Right block
            new SpecialAnimation(0x08063368, false, 116), // Big-small block toggle
            new SpecialAnimation(0x080633e8, false, 53), // Big-small block
            new SpecialAnimation(0x080635e8, false, 114, objParam_2: 3), // Triple red switch
            new SpecialAnimation(0x08063668, false, 111, objParam_1: 3), // Down block
            new SpecialAnimation(0x08063ae8, false, 40), // Vertical water platform
            new SpecialAnimation(0x08063fe8, false, 113), // Water switch
            new SpecialAnimation(0x08064868, false, 54), // Red arrow
            new SpecialAnimation(0x08064a68, false, 117), // Blue arrow
            new SpecialAnimation(0x08065168, false, 37, objParam_1: 0), // Attachment block
            new SpecialAnimation(0x08065368, false, 37, objParam_1: 1), // Stationary attachment block
            new SpecialAnimation(0x08065568, false, 111, objParam_1: 5), // Left block

            // 0x08064c68 - bubble enemies from boss
        };

        public Dictionary<int, MapVRAMAllocationInfo[]> LevelVRAMAllocationInfos => new Dictionary<int, MapVRAMAllocationInfo[]>()
        {
            // 1-0
            [0] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x0805dbe8, 0x300, tileIndex: 296),
                new MapVRAMAllocationInfo(0x0805dee8, 0x200),
                new MapVRAMAllocationInfo(0x0805e0e8, 0x200),
                new MapVRAMAllocationInfo(0x0805e2e8, 0x400),
                new MapVRAMAllocationInfo(0x0805e6e8, 0x400),

                new MapVRAMAllocationInfo(0x0805eae8, 0x200),
                new MapVRAMAllocationInfo(0x0805eae8, 0x200),
                new MapVRAMAllocationInfo(0x0805eae8, 0x200),
                new MapVRAMAllocationInfo(0x0805eae8, 0x200),
                new MapVRAMAllocationInfo(0x0805eae8, 0x200),
                new MapVRAMAllocationInfo(0x0805eae8, 0x200),
                new MapVRAMAllocationInfo(0x0805eae8, 0x200),
                new MapVRAMAllocationInfo(0x0805eae8, 0x200),

                new MapVRAMAllocationInfo(0x02000904, 0x800),
                new MapVRAMAllocationInfo(0x02001104, 0x800),
                new MapVRAMAllocationInfo(0x02001904, 0x200),
                new MapVRAMAllocationInfo(0x02001b04, 0x200),
                new MapVRAMAllocationInfo(0x02001d04, 0x200),
                new MapVRAMAllocationInfo(0x02002704, 0x200),
                new MapVRAMAllocationInfo(0x02002904, 0x200),
                new MapVRAMAllocationInfo(0x02002b04, 0x200),
                new MapVRAMAllocationInfo(0x02002d04, 0x200),
                new MapVRAMAllocationInfo(0x02002104, 0x200),
                new MapVRAMAllocationInfo(0x02002304, 0x200),
                new MapVRAMAllocationInfo(0x02002504, 0x200),
                new MapVRAMAllocationInfo(0x02001f04, 0x200),
            },

            // 1-8
            [8] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x08060a88, 0x600, tileIndex: 284), // Boss health bar
                new MapVRAMAllocationInfo(0x08061888, 0x100, tileIndex: 620), // Shadow
            },

            // 2-0
            [9] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x02000904, 0x800, tileIndex: 408),
                new MapVRAMAllocationInfo(0x02001104, 0x800),
                new MapVRAMAllocationInfo(0x02001904, 0x200),
                new MapVRAMAllocationInfo(0x02001b04, 0x200),
                new MapVRAMAllocationInfo(0x02001d04, 0x200),
                new MapVRAMAllocationInfo(0x02001f04, 0x200),
            },

            // 2-8
            [17] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x08060a88, 0x600, tileIndex: 284), // Boss health bar
            },

            // 3-0
            [18] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x02000904, 0x800, tileIndex: 408),
                new MapVRAMAllocationInfo(0x02001104, 0x200),
                new MapVRAMAllocationInfo(0x02001d04, 0x200),
                new MapVRAMAllocationInfo(0x02001f04, 0x200),
                new MapVRAMAllocationInfo(0x02001904, 0x200),
                new MapVRAMAllocationInfo(0x02001704, 0x200),
                new MapVRAMAllocationInfo(0x02001b04, 0x200),
            },

            // 3-8
            [26] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x08060a88, 0x600, tileIndex: 284), // Boss health bar
            },

            // 4-0
            [27] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x02000904, 0x800, tileIndex: 408),
                new MapVRAMAllocationInfo(0x02002304, 0x200),
                new MapVRAMAllocationInfo(0x02002104, 0x200),
                new MapVRAMAllocationInfo(0x02002504, 0x200),
                new MapVRAMAllocationInfo(0x02002704, 0x200),
                new MapVRAMAllocationInfo(0x02002904, 0x200),
                new MapVRAMAllocationInfo(0x02002b04, 0x200),
                new MapVRAMAllocationInfo(0x02002d04, 0x200),
                new MapVRAMAllocationInfo(0x02002f04, 0x200),
            },

            // 4-8
            [35] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x08060a88, 0x600, tileIndex: 284), // Boss health bar
            },

            // 5-0
            [36] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x02004704, 0x200, tileIndex: 408),
                new MapVRAMAllocationInfo(0x02001104, 0x800),
                new MapVRAMAllocationInfo(0x02000904, 0x800),
                new MapVRAMAllocationInfo(0x02004104, 0x200),
                new MapVRAMAllocationInfo(0x02004304, 0x200),
                new MapVRAMAllocationInfo(0x02004504, 0x200),
            },

            // 5-8
            [44] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x08060a88, 0x600, tileIndex: 284), // Boss health bar
            },

            // 6-8
            [53] = new MapVRAMAllocationInfo[]
            {
                new MapVRAMAllocationInfo(0x08060a88, 0x600, tileIndex: 284), // Boss health bar
            },
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
            public SpecialAnimation(long frameOffset, bool isFix, int index, int? groupCount = null, int? objParam_1 = null, int? objParam_2 = null)
            {
                FrameOffsets = new long[]
                {
                    frameOffset
                };
                IsFix = isFix;
                Index = index;
                GroupCount = groupCount;
                ObjParam_1 = objParam_1;
                ObjParam_2 = objParam_2;
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

            /// <summary>
            /// The param the object has to be set to for this animation to be applied
            /// </summary>
            public int? ObjParam_1 { get; }

            public int? ObjParam_2 { get; }

            public bool MatchesObject(GBAKlonoa_LoadedObject obj)
            {
                if (ObjParam_1 != null && ObjParam_1 != obj.Param_1)
                    return false;

                if (ObjParam_2 != null && ObjParam_2 != obj.Param_2)
                    return false;

                return IsFix && obj.Index == Index ||
                       !IsFix && obj.ObjType == Index;
            }
        }

        public class MapVRAMAllocationInfo
        {
            public MapVRAMAllocationInfo(int offset, int length, int? tileIndex = null)
            {
                Offset = offset;
                Length = length;
                TileIndex = tileIndex;
            }

            public int Offset { get; }
            public int Length { get; }
            public int? TileIndex { get; }
        }
    }
}