﻿using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_EOD_Manager : GBAKlonoa_BaseManager
    {
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
            var rom = FileFactory.Read<GBAKlonoa_EOD_ROM>(context, GetROMFilePath);
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

            var collisionLines = !isMap && !isBoss ? GetSectorCollisionLines(rom.MapSectors[normalLevelIndex]) : null;

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

            var objects = new List<Unity_SpriteObject>();

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
                objects.AddRange(startInfos.Select(x => new Unity_Object_GBAKlonoa(objmanager, new GBAKlonoa_LoadedObject(0, 0, x.XPos, x.YPos, 0, 0, 0, 0, 0x6e), x, allOAMCollections[0], x.Sector - 1)));
            }

            // Add fixed objects, except Klonoa (first one) and object 11/12 for maps since it's not used
            objects.AddRange(fixObjects.Skip(isMap ? 0 : 1).Where(x => !isMap || (x.Index != 11 && x.Index != 12)).Select(x => new Unity_Object_GBAKlonoa(objmanager, x, null, allOAMCollections[x.OAMIndex], -1)));

            // Add level objects, duplicating them for each sector they appear in (if flags is 28 we assume it's unused in the sector)
            objects.AddRange(levelObjects.SelectMany((x, i) => x.Select(o => new
            {
                Data = o,
                SectorIndex = i
            })).Where(x => isMap || x.Data.Value_8 != 28).Select(x => new Unity_Object_GBAKlonoa(objmanager, x.Data, (BinarySerializable)x.Data.LevelObj ?? x.Data.WorldMapObj, allOAMCollections[x.Data.OAMIndex], levelObjects.Length > 1 ? x.SectorIndex : -1)));

            if (isMap) 
                CorrectWorldMapObjectPositions(objects, maps[2].Width, maps[2].Height);

            return new Unity_Level()
            {
                Maps = maps,
                ObjManager = objmanager,
                EventData = objects,
                CellSize = CellSize,
                DefaultLayer = 2,
                IsometricData = isMap ? Unity_IsometricData.Mode7(CellSize) : null,
                CollisionLines = collisionLines
            };
        }

        public Unity_ObjectManager_GBAKlonoa.AnimSet[] LoadAnimSets(Context context, GBAKlonoa_EOD_ROM rom, IEnumerable<GBAKlonoa_ObjectGraphics> animSets, GBAKlonoa_ObjectOAMCollection[] oamCollections, Color[][] palettes, IList<GBAKlonoa_LoadedObject> objects, Pointer levelTextSpritePointer)
        {
            var settings = context.GetR1Settings();
            var globalLevelIndex = GetGlobalLevelIndex(settings.World, settings.Level);
            var isBoss = settings.Level == 8;

            var loadedAnimSets = new List<Unity_ObjectManager_GBAKlonoa.AnimSet>();

            // Start by adding a null entry to use as default
            loadedAnimSets.Add(new Unity_ObjectManager_GBAKlonoa.AnimSet(new Unity_ObjectManager_GBAKlonoa.AnimSet.Animation[0], $"NULL", new GBAKlonoa_OAM[0][]));

            // Load animations declared through the graphics data
            LoadAnimSets_ObjGraphics(loadedAnimSets, animSets, oamCollections, palettes, objects);

            // Load animations from allocated tiles. This is what the game does for practically all animations, but for simplicity we mainly do this for the maps.
            var allocationInfos = LevelVRAMAllocationInfos.TryGetItem(globalLevelIndex);

            if (allocationInfos != null)
                LoadAnimSets_AllocatedTiles(loadedAnimSets, context, allocationInfos, oamCollections, palettes);

            // Load special (hard-coded) animations
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

            LoadAnimSets_SpecialAnims(loadedAnimSets, context, specialAnims, palettes, objects, oamCollections, FixCount);

            return loadedAnimSets.ToArray();
        }

        public void GenerateAnimSetTable(Context context, GBAKlonoa_EOD_ROM rom)
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
                        using var context = new Ray1MapContext(settings);

                        await LoadFilesAsync(context);

                        var rom = FileFactory.Read<GBAKlonoa_EOD_ROM>(context, GetROMFilePath);

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

        // TODO: Support EU and JP versions
        public override AnimSetInfo[] AnimSetInfos => new AnimSetInfo[]
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
    }
}