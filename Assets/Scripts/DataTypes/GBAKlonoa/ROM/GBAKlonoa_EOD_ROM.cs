using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;

namespace R1Engine
{
    public class GBAKlonoa_EOD_ROM : GBA_ROMBase
    {
        // Info
        public GBAKlonoa_EOD_WaterSkiInfo[] WaterSkiInfos { get; set; }
        public GBAKlonoa_LevelStartInfos[] LevelStartInfos { get; set; }
        public GBAKlonoa_LevelStartInfo[] BossLevelStartInfos { get; set; }

        // Maps
        public GBAKlonoa_EOD_Maps[] Maps { get; set; }
        public GBAKlonoa_EOD_MapWidths[] MapWidths { get; set; }
        public GBAKlonoa_MapSectors[] MapSectors { get; set; }
        public GBAKlonoa_EOD_TileSets[] TileSets { get; set; }
        public Pointer[] MapPalettePointers { get; set; }
        public RGBA5551Color[][] MapPalettes { get; set; }

        // Objects
        public Pointer[] CompressedObjTileBlockPointers { get; set; }
        public byte[][] CompressedObjTileBlocks { get; set; }
        public GBAKlonoa_LoadedObject[] FixObjects { get; set; }
        public GBAKlonoa_LevelObjectCollection LevelObjectCollection { get; set; } // For current level only - too slow to read all of them
        public GBAKlonoa_WorldMapObjectCollection WorldMapObjectCollection { get; set; }
        public GBAKlonoa_ObjectGraphics[] FixObjectGraphics { get; set; }
        public Pointer[] LevelObjectGraphicsPointers { get; set; }
        public GBAKlonoa_ObjectGraphics[][] LevelObjectGraphics { get; set; }
        public GBAKlonoa_ObjectOAMCollection[] FixObjectOAMCollections { get; set; }
        public Pointer[] LevelObjectOAMCollectionPointers { get; set; }
        public GBAKlonoa_ObjectOAMCollection[][] LevelObjectOAMCollections { get; set; }
        public GBAKlonoa_ObjPal[] ObjectPalettes { get; set; }
        public Pointer[] LevelTextSpritePointers { get; set; }

        // TODO: Use pointer tables
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var settings = s.GetR1Settings();
            var globalLevelIndex = GBAKlonoa_EOD_Manager.GetGlobalLevelIndex(settings.World, settings.Level);
            const int normalWorldsCount = GBAKlonoa_EOD_Manager.NormalWorldsCount;
            const int levelsCount = GBAKlonoa_EOD_Manager.LevelsCount;
            const int normalLevelsCount = GBAKlonoa_EOD_Manager.NormalLevelsCount;
            var isMap = settings.Level == 0;
            var isBoss = settings.Level == 8;

            // Serialize water-ski infos
            s.DoAt(new Pointer(0x080d821c, Offset.File), () => WaterSkiInfos = s.SerializeObjectArray<GBAKlonoa_EOD_WaterSkiInfo>(WaterSkiInfos, 7, name: nameof(WaterSkiInfos)));

            var isWaterSkii = WaterSkiInfos.Any(x => x.World == settings.World && x.Level == settings.Level);

            // Serialize level start positions
            if (isBoss)
            {
                s.DoAt(new Pointer(0x080d6458, Offset.File), () => BossLevelStartInfos = s.SerializeObjectArray<GBAKlonoa_LevelStartInfo>(BossLevelStartInfos, normalWorldsCount, name: nameof(BossLevelStartInfos)));
            }
            else if (!isMap)
            {
                s.DoAt(new Pointer(0x080d48c8, Offset.File), () => LevelStartInfos = s.SerializeObjectArray<GBAKlonoa_LevelStartInfos>(LevelStartInfos, normalLevelsCount, name: nameof(LevelStartInfos)));
            }

            // Serialize maps
            s.DoAt(new Pointer(0x081892BC, Offset.File), () =>
            {
                Maps ??= new GBAKlonoa_EOD_Maps[levelsCount];

                for (int i = 0; i < Maps.Length; i++)
                    Maps[i] = s.SerializeObject<GBAKlonoa_EOD_Maps>(Maps[i], x => x.Pre_SerializeData = i == globalLevelIndex, name: $"{nameof(Maps)}[{i}]");
            });

            // Serialize map widths
            s.DoAt(new Pointer(0x08051c76, Offset.File), () => MapWidths = s.SerializeObjectArray<GBAKlonoa_EOD_MapWidths>(MapWidths, levelsCount, name: nameof(MapWidths)));
            
            // Serialize map sectors
            s.DoAt(new Pointer(0x080d2e88, Offset.File), () => MapSectors = s.SerializeObjectArray<GBAKlonoa_MapSectors>(MapSectors, normalLevelsCount, name: nameof(MapSectors)));

            // Serialize tile sets
            s.DoAt(new Pointer(0x08189034, Offset.File), () =>
            {
                TileSets ??= new GBAKlonoa_EOD_TileSets[levelsCount];

                for (int i = 0; i < TileSets.Length; i++)
                    TileSets[i] = s.SerializeObject<GBAKlonoa_EOD_TileSets>(TileSets[i], x => x.Pre_SerializeData = i == globalLevelIndex, name: $"{nameof(TileSets)}[{i}]");
            });

            // Serialize palettes
            s.DoAt(new Pointer(0x08188f5C, Offset.File), () => MapPalettePointers = s.SerializePointerArray(MapPalettePointers, levelsCount, name: nameof(MapPalettePointers)));

            MapPalettes ??= new RGBA5551Color[MapPalettePointers.Length][];
            s.DoAt(MapPalettePointers[globalLevelIndex], () =>
            {
                s.DoEncoded(new GBAKlonoa_EOD_Encoder(), () =>
                {
                    MapPalettes[globalLevelIndex] = s.SerializeObjectArray<RGBA5551Color>(MapPalettes[globalLevelIndex], 256, name: $"{nameof(MapPalettes)}[{globalLevelIndex}]");
                });
            });

            // Serialize compressed object tiles
            if (isMap || isBoss)
            {
                s.DoAt(new Pointer(0x0818b7ac, Offset.File), () => CompressedObjTileBlockPointers = s.SerializePointerArray(CompressedObjTileBlockPointers, normalWorldsCount * 2, name: nameof(CompressedObjTileBlockPointers)));

                var compressedBlockIndex = GBAKlonoa_EOD_Manager.GetCompressedMapOrBossBlockIndex(settings.World, settings.Level);

                CompressedObjTileBlocks ??= new byte[CompressedObjTileBlockPointers.Length][];
                s.DoAt(CompressedObjTileBlockPointers[compressedBlockIndex], () =>
                {
                    s.DoEncoded(new GBAKlonoa_EOD_Encoder(), () =>
                    {
                        CompressedObjTileBlocks[compressedBlockIndex] = s.SerializeArray<byte>(CompressedObjTileBlocks[compressedBlockIndex], s.CurrentLength, name: $"{nameof(CompressedObjTileBlocks)}[{compressedBlockIndex}]");
                    });
                });

                // Allocate the compressed block. This fixes animations which use pointers to ram, such as the bosses.
                var file = new MemoryMappedByteArrayFile(s.Context, GBAKlonoa_BaseManager.CompressedObjTileBlockName, 0x02000904, CompressedObjTileBlocks[compressedBlockIndex]);
                s.Context.AddFile(file);
            }

            // Initialize fixed objects
            FixObjects = GBAKlonoa_LoadedObject.GetFixedObjects(settings.EngineVersion, settings.World, settings.Level).ToArray();

            // Serialize level objects
            if (isMap)
            {
                // Each level has 25 object slots and each object is 8 bytes
                var mapObjOffset = (settings.World - 1) * 25 * 8;

                s.DoAt(new Pointer(0x0811717c + mapObjOffset, Offset.File), () => WorldMapObjectCollection = s.SerializeObject<GBAKlonoa_WorldMapObjectCollection>(WorldMapObjectCollection, name: nameof(WorldMapObjectCollection)));
            }
            else
            {
                // Each level has 100 object slots, each world has 8 level slots and each object is 44 bytes
                var lvlObjOffset = ((settings.World - 1) * 800 + (settings.Level - 1) * 100) * 44;

                s.DoAt(new Pointer(0x080e2b64 + lvlObjOffset, Offset.File), () => LevelObjectCollection = s.SerializeObject<GBAKlonoa_LevelObjectCollection>(LevelObjectCollection, name: nameof(LevelObjectCollection)));
            }

            // Serialize fixed graphics
            if (!isWaterSkii || isBoss)
            {
                s.DoAt(new Pointer(0x0805553c, Offset.File), () => FixObjectGraphics = s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(FixObjectGraphics, 9, name: nameof(FixObjectGraphics)));
            }
            else
            {
                s.DoAt(new Pointer(0x080555a8, Offset.File), () => FixObjectGraphics = s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(FixObjectGraphics, 9, name: nameof(FixObjectGraphics)));    
            }

            // Serialize level graphics
            s.DoAt(new Pointer(0x08189a24, Offset.File), () => LevelObjectGraphicsPointers = s.SerializePointerArray(LevelObjectGraphicsPointers, levelsCount, name: nameof(LevelObjectGraphicsPointers)));

            LevelObjectGraphics ??= new GBAKlonoa_ObjectGraphics[LevelObjectGraphicsPointers.Length][];
            s.DoAt(LevelObjectGraphicsPointers[globalLevelIndex], () =>
            {
                LevelObjectGraphics[globalLevelIndex] = s.SerializeObjectArrayUntil<GBAKlonoa_ObjectGraphics>(
                    obj: LevelObjectGraphics[globalLevelIndex], 
                    conditionCheckFunc: x => x.AnimationsPointerValue == 0, 
                    getLastObjFunc: () => new GBAKlonoa_ObjectGraphics(),
                    onPreSerialize: x => x.Pre_LevelObjects = LevelObjectCollection,
                    name: $"{nameof(LevelObjectGraphics)}[{globalLevelIndex}]");
            });

            // Serialize fixed OAM collections
            s.DoAt(new Pointer(0x08078fc8, Offset.File), () => FixObjectOAMCollections = s.SerializeObjectArray<GBAKlonoa_ObjectOAMCollection>(FixObjectOAMCollections, FixObjects.Length, name: nameof(FixObjectOAMCollections)));

            // Serialize level OAM collections
            s.DoAt(new Pointer(0x0818b8e0, Offset.File), () => LevelObjectOAMCollectionPointers = s.SerializePointerArray(LevelObjectOAMCollectionPointers, levelsCount, name: nameof(LevelObjectOAMCollectionPointers)));

            LevelObjectOAMCollections ??= new GBAKlonoa_ObjectOAMCollection[LevelObjectOAMCollectionPointers.Length][];
            s.DoAt(LevelObjectOAMCollectionPointers[globalLevelIndex], () =>
            {
                byte max;
                
                if (isMap)
                {
                    max = WorldMapObjectCollection.Objects.Max(x => x.OAMIndex);
                }
                else
                {
                    if (LevelObjectCollection.Objects.Any())
                        max = LevelObjectCollection.Objects.Max(x => x.OAMIndex);
                    else
                        max = GBAKlonoa_EOD_Manager.FixCount - 1;
                }

                LevelObjectOAMCollections[globalLevelIndex] = s.SerializeObjectArray<GBAKlonoa_ObjectOAMCollection>(LevelObjectOAMCollections[globalLevelIndex], max - GBAKlonoa_EOD_Manager.FixCount + 1, name: $"{nameof(LevelObjectOAMCollections)}[{globalLevelIndex}]");
            });

            // Serialize object palettes
            s.DoAt(new Pointer(0x08077e28, Offset.File), () => ObjectPalettes = s.SerializeObjectArray<GBAKlonoa_ObjPal>(ObjectPalettes, 141, name: nameof(ObjectPalettes)));

            // Serialize level text sprites
            if (!isMap && !isBoss)
                s.DoAt(new Pointer(0x0818b800, Offset.File), () => LevelTextSpritePointers = s.SerializePointerArray(LevelTextSpritePointers, normalLevelsCount, name: nameof(LevelTextSpritePointers)));
        }

        public int[] GetLevelObjPalIndices(int levelIndex)
        {
            return levelIndex switch
            {
                // World 1
                0 => new int[] { 02, 03, 04, 05, 06, 07, 08, 09, 10 },
                1 => new int[] { 39, 40, 41, 42, 43, 44 },
                2 => new int[] { 39, 40, 45, 46, 42, 43, 44 },
                3 => new int[] { 39, 40, 41, 45, 46, 47, 42, 43, 44 },
                4 => new int[] { 40, 41, 45, 48, 43, 44 },
                5 => new int[] { 39, 40, 41, 49, 45, 46, 47, 42, 43, 44 },
                6 => new int[] { 50, 41, 49, 45, 43, 44 },
                7 => new int[] { 39, 40, 49, 45, 51, 46, 47, 42, 43, 44 },
                8 => new int[] { 52, 53, 54, 55, 40, 56, 54, 57 },

                // World 2
                9 => new int[] { 06, 11, 12, 13, 14, 15, 16 },
                10 => new int[] { 39, 40, 41, 45, 58, 46, 47, 42, 43, 44 },
                11 => new int[] { 39, 40, 41, 59, 58, 45, 46, 47, 42, 43, 44 },
                12 => new int[] { 39, 40, 41, 59, 49, 46, 60, 47, 42, 43, 44 },
                13 => new int[] { 40, 41, 45, 58, 48, 43, 44 },
                14 => new int[] { 39, 40, 41, 45, 49, 51, 58, 61, 46, 47, 43, 44 },
                15 => new int[] { 50, 40, 41, 49, 45, 51, 43, 44 },
                16 => new int[] { 39, 40, 41, 58, 62, 61, 63, 46, 47, 43, 44 },
                17 => new int[] { 52, 53, 54, 64, 40, 65, 66, 67, 44 },

                // World 3
                18 => new int[] { 06, 17, 18, 19, 20, 21, 22, 23 },
                19 => new int[] { 39, 40, 49, 68, 46, 69, 47, 42, 43, 44 },
                20 => new int[] { 39, 41, 49, 68, 46, 69, 47, 42, 43, 44 },
                21 => new int[] { 39, 41, 49, 51, 68, 69, 47, 42, 43, 44 },
                22 => new int[] { 40, 41, 45, 58, 48, 43, 44 },
                23 => new int[] { 39, 49, 68, 46, 70, 61, 47, 43 },
                24 => new int[] { 50, 40, 41, 49, 59, 45, 43, 44 },
                25 => new int[] { 39, 41, 58, 68, 46, 70, 61, 71, 43, 44 },
                26 => new int[] { 52, 53, 54, 72, 41, 73, 72, 75, 44 },

                // World 4
                27 => new int[] { 06, 24, 25, 26, 27, 28, 29, 30, 31, 32 },
                28 => new int[] { 39, 40, 41, 58, 49, 76, 46, 77, 83, 47, 42, 43, 44 },
                29 => new int[] { 39, 40, 41, 58, 59, 49, 46, 43, 83, 47, 42, 44 },
                30 => new int[] { 39, 41, 59, 49, 45, 76, 46, 47, 42, 43, 44 },
                31 => new int[] { 40, 41, 45, 58, 48, 43, 44 },
                32 => new int[] { 39, 40, 46, 68, 77, 78, 81, 47, 42, 43, 44 },
                33 => new int[] { 50, 40, 41, 49, 59, 45, 51, 43, 44 },
                34 => new int[] { 39, 40, 41, 58, 82, 46, 42, 43, 83, 81, 47, 44 },
                35 => new int[] { 52, 53, 54, 84, 40, 85, 81, 44 },

                // World 5
                36 => new int[] { 06, 33, 34, 35, 36, 37, 38 },
                37 => new int[] { 39, 40, 45, 86, 47, 42, 43, 44 },
                38 => new int[] { 39, 40, 41, 45, 51, 68, 86, 61, 47, 43, 44 },
                39 => new int[] { 39, 40, 45, 86, 68, 87, 46, 61, 81, 43, 44 },
                40 => new int[] { 40, 41, 45, 58, 48, 43, 44 },
                41 => new int[] { 39, 40, 41, 59, 45, 58, 86, 76, 79, 47, 42, 43, 44 },
                42 => new int[] { 50, 40, 41, 49, 59, 45, 43, 44 },
                43 => new int[] { 39, 40, 41, 51, 59, 45, 86, 68, 46, 47, 88, 42, 43, 44 },
                44 => new int[] { 52, 53, 54, 89, 40, 90, 91, 44 },

                // World 6
                45 => new int[] { 06 }, // Has no palette, but maps usually begin with 06 and that makes the map icons correct
                46 => new int[] { 39, 40, 41, 49, 59, 45, 51, 58, 87, 42, 43, 44 },
                47 => new int[] { 39, 40, 41, 59, 49, 51, 58, 68, 45, 46, 42, 47, 43, 44 },
                48 => new int[] { 39, 40, 41, 45, 51, 92, 63, 42, 43, 44 },
                49 => new int[] { 39, 40, 41, 45, 51, 92, 63, 42, 43, 44 },
                50 => new int[] { 39, 40, 41, 45, 51, 92, 63, 42, 43, 44 },
                51 => new int[] { 39, 40, 41, 45, 51, 92, 63, 42, 43, 44 },
                52 => new int[] { 39, 40, 41, 45, 51, 92, 63, 42, 43, 44 },
                53 => new int[] { 52, 53, 54, 93, 94, 40, 40, 96, 97, 98, 44 },
                _ => new int[0]
            };
        }

        public BaseColor[] GetLevelObjPal(int levelIndex)
        {
            var fix = new int[]
            {
                0, 1
            };

            return fix.Concat(GetLevelObjPalIndices(levelIndex)).Select(x => x == -1 ? Util.CreateDummyPalette(16) : ObjectPalettes[x].Colors).SelectMany(x => x).ToArray();
        }
    }
}