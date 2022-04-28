using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Nintendo;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_DCT_ROM : GBA_ROMBase
    {
        // Info
        public GBAKlonoa_LevelStartInfos[] LevelStartInfos { get; set; }
        public Pointer[] WaterSkiDataPointers { get; set; }
        public GBAKlonoa_DCT_WaterSkiData[] WaterSkiDatas { get; set; }

        // Maps
        public GBAKlonoa_DCT_Map[] Maps { get; set; }
        public RGBA5551Color[] FixTilePalette { get; set; }
        public GBAKlonoa_MapSectors[] MapSectors { get; set; }
        public Pointer[] MapGraphicsPointers { get; set; }
        public GBAKlonoa_ObjectGraphics[][] MapGraphics { get; set; }

        // Objects
        public Pointer[] CompressedWorldObjTileBlockPointers { get; set; }
        public byte[][] CompressedWorldObjTileBlocks { get; set; }
        public Pointer[] CompressedBossObjTileBlockPointers { get; set; }
        public byte[][] CompressedBossObjTileBlocks { get; set; }
        public byte[] CompressedWorldMapObjTileBlock { get; set; }
        public GBAKlonoa_LoadedObject[] FixObjects { get; set; }
        public GBAKlonoa_LevelObjectCollection LevelObjectCollection { get; set; } // For current level only - too slow to read all of them
        public GBAKlonoa_WorldMapObjectCollection WorldMapObjectCollection { get; set; }
        public GBAKlonoa_ObjectGraphics[] FixObjectGraphics { get; set; }
        public Pointer[] WorldMapObjectGraphicPointers { get; set; }
        public GBAKlonoa_ObjectGraphics[][] WorldMapObjectGraphics { get; set; }
        public GBAKlonoa_DCT_GraphicsData[] GraphicsDatas { get; set; }
        public GBAKlonoa_ObjectOAMCollection[] FixObjectOAMCollections { get; set; }
        public Pointer[] WorldMapObjectOAMCollectionPointers { get; set; }
        public GBAKlonoa_ObjectOAMCollection[][] WorldMapObjectOAMCollections { get; set; }
        public GBAKlonoa_ObjPal[] FixObjectPalettes { get; set; }
        public Pointer[] LevelNumSpritePointers { get; set; }

        // TODO: Use pointer tables
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Serialize game info
            SerializeInfo(s);

            // Serialize map data
            SerializeMapData(s);

            // Serialize compressed block data
            SerializeCompressedBlockData(s);

            // Serialize object data
            SerializeObjData(s);
        }

        protected void SerializeInfo(SerializerObject s)
        {
            var settings = s.GetR1Settings();
            const int normalLevelsCount = GBAKlonoa_DCT_Manager.NormalLevelsCount;
            var isMap = settings.Level == 0;
            var isWaterSki = settings.Level == 4;
            var worldIndex = settings.World - 1;

            // Serialize level start positions
            if (!isMap && !isWaterSki)
                s.DoAt(new Pointer(0x0810ca00, Offset.File), () => LevelStartInfos = s.SerializeObjectArray<GBAKlonoa_LevelStartInfos>(LevelStartInfos, normalLevelsCount, name: nameof(LevelStartInfos)));

            // Serialize waterski data
            if (isWaterSki && settings.World != 6)
            {
                s.DoAt(new Pointer(0x081d6e60, Offset.File), () => WaterSkiDataPointers = s.SerializePointerArray(WaterSkiDataPointers, 5, name: nameof(WaterSkiDataPointers)));

                WaterSkiDatas ??= new GBAKlonoa_DCT_WaterSkiData[WaterSkiDataPointers.Length];
                s.DoAt(WaterSkiDataPointers[worldIndex], () =>
                {
                    WaterSkiDatas[worldIndex] = s.SerializeObject<GBAKlonoa_DCT_WaterSkiData>(WaterSkiDatas[worldIndex], name: $"{nameof(WaterSkiDatas)}[{worldIndex}]");
                });
            }
        }

        protected void SerializeMapData(SerializerObject s)
        {
            var settings = s.GetR1Settings();
            var globalLevelIndex = GBAKlonoa_DCT_Manager.GetGlobalLevelIndex(settings.World, settings.Level);
            const int levelsCount = GBAKlonoa_DCT_Manager.LevelsCount;
            const int normalLevelsCount = GBAKlonoa_DCT_Manager.NormalLevelsCount;

            // Serialize maps
            s.DoAt(new Pointer(0x8052AFC, Offset.File), () =>
            {
                Maps ??= new GBAKlonoa_DCT_Map[levelsCount];

                for (int i = 0; i < Maps.Length; i++)
                    Maps[i] = s.SerializeObject<GBAKlonoa_DCT_Map>(Maps[i], x => x.Pre_SerializeData = i == globalLevelIndex, name: $"{nameof(Maps)}[{i}]");
            });

            s.DoAt(new Pointer(0x083514e4, Offset.File), () => FixTilePalette = s.SerializeObjectArray<RGBA5551Color>(FixTilePalette, 0x20, name: nameof(FixTilePalette)));

            // Serialize map sectors
            s.DoAt(new Pointer(0x0810a480, Offset.File), () => MapSectors = s.SerializeObjectArray<GBAKlonoa_MapSectors>(MapSectors, normalLevelsCount, name: nameof(MapSectors)));
        }

        protected void SerializeCompressedBlockData(SerializerObject s)
        {
            var settings = s.GetR1Settings();
            const int normalWorldsCount = GBAKlonoa_DCT_Manager.NormalWorldsCount;
            var isMap = settings.Level == 0;
            var isBoss = settings.Level == 9;
            var worldIndex = settings.World - 1;

            // Serialize compressed world object tile blocks
            s.DoAt(new Pointer(0x081d65e0, Offset.File), () => CompressedWorldObjTileBlockPointers = s.SerializePointerArray(CompressedWorldObjTileBlockPointers, normalWorldsCount, name: nameof(CompressedWorldObjTileBlockPointers)));

            CompressedWorldObjTileBlocks ??= new byte[CompressedWorldObjTileBlockPointers.Length][];
            s.DoAt(CompressedWorldObjTileBlockPointers[worldIndex], () =>
            {
                s.DoEncoded(new GBAKlonoa_DCT_Encoder(), () =>
                {
                    CompressedWorldObjTileBlocks[worldIndex] = s.SerializeArray<byte>(CompressedWorldObjTileBlocks[worldIndex], s.CurrentLength, name: $"{nameof(CompressedWorldObjTileBlocks)}[{worldIndex}]");
                });
            });

            var worldBlockFile = new StreamFile(s.Context, GBAKlonoa_BaseManager.CompressedWorldObjTileBlockName, new MemoryStream(CompressedWorldObjTileBlocks[worldIndex]));
            s.Context.AddFile(worldBlockFile);
            
            // Serialize compressed boss object tile blocks
            if (isBoss)
            {
                s.DoAt(new Pointer(0x081d65c8, Offset.File), () => CompressedBossObjTileBlockPointers = s.SerializePointerArray(CompressedBossObjTileBlockPointers, normalWorldsCount, name: nameof(CompressedBossObjTileBlockPointers)));

                CompressedBossObjTileBlocks ??= new byte[CompressedBossObjTileBlockPointers.Length][];
                s.DoAt(CompressedBossObjTileBlockPointers[worldIndex], () =>
                {
                    s.DoEncoded(new GBAKlonoa_DCT_Encoder(), () =>
                    {
                        CompressedBossObjTileBlocks[worldIndex] = s.SerializeArray<byte>(CompressedBossObjTileBlocks[worldIndex], s.CurrentLength, name: $"{nameof(CompressedBossObjTileBlocks)}[{worldIndex}]");
                    });
                });

                var blockFile = new StreamFile(s.Context, GBAKlonoa_BaseManager.CompressedObjTileBlockName, new MemoryStream(CompressedBossObjTileBlocks[worldIndex]));
                s.Context.AddFile(blockFile);
            }
            else if (isMap)
            {
                s.DoAt(new Pointer(0x083bc12c, Offset.File), () =>
                {
                    s.DoEncoded(new GBAKlonoa_DCT_Encoder(), () =>
                    {
                        CompressedWorldMapObjTileBlock = s.SerializeArray<byte>(CompressedWorldMapObjTileBlock, s.CurrentLength, name: nameof(CompressedWorldMapObjTileBlock));
                    });
                });

                var blockFile = new StreamFile(s.Context, GBAKlonoa_BaseManager.CompressedObjTileBlockName, new MemoryStream(CompressedWorldMapObjTileBlock));
                s.Context.AddFile(blockFile);
            }
        }

        protected void SerializeObjData(SerializerObject s)
        {
            var settings = s.GetR1Settings();
            var globalLevelIndex = GBAKlonoa_DCT_Manager.GetGlobalLevelIndex(settings.World, settings.Level);
            const int normalWorldsCount = GBAKlonoa_DCT_Manager.NormalWorldsCount;
            const int levelsCount = GBAKlonoa_DCT_Manager.LevelsCount;
            var isMap = settings.Level == 0;
            var isBoss = settings.Level == 9;
            var isWaterSki = settings.Level == 4;
            var isUnderWater = settings.World == 4 && (settings.Level == 5 || settings.Level == 1 || settings.Level == 7);

            // Serialize map graphics
            s.DoAt(new Pointer(0x081d6084, Offset.File), () => MapGraphicsPointers = s.SerializePointerArray(MapGraphicsPointers, levelsCount, name: nameof(MapGraphicsPointers)));

            MapGraphics ??= new GBAKlonoa_ObjectGraphics[MapGraphicsPointers.Length][];
            s.DoAt(MapGraphicsPointers[globalLevelIndex], () =>
            {
                MapGraphics[globalLevelIndex] = s.SerializeObjectArrayUntil<GBAKlonoa_ObjectGraphics>(
                    obj: MapGraphics[globalLevelIndex],
                    conditionCheckFunc: x => x.AnimationsPointer == null,
                    getLastObjFunc: () => new GBAKlonoa_ObjectGraphics(),
                    onPreSerialize: (x, _) => x.Pre_IsMapAnimations = true,
                    name: $"{nameof(MapGraphics)}[{globalLevelIndex}]");
            });

            // Initialize fixed objects
            FixObjects = GBAKlonoa_LoadedObject.GetFixedObjects(settings.EngineVersion, settings.World, settings.Level).ToArray();

            if (isMap)
            {
                var mapObjOffset = (settings.World - 1) * 0xd2;

                s.DoAt(new Pointer(0x08150dac + mapObjOffset, Offset.File), () => WorldMapObjectCollection = s.SerializeObject<GBAKlonoa_WorldMapObjectCollection>(WorldMapObjectCollection, name: nameof(WorldMapObjectCollection)));
            }
            else
            {
                // Each level has 90 object slots, each world has 9 level slots and each object is 44 bytes
                var lvlObjOffset = ((settings.World - 1) * 900 + (settings.Level - 1) * 90) * 44;

                s.DoAt(new Pointer(0x08110260 + lvlObjOffset, Offset.File), () => LevelObjectCollection = s.SerializeObject<GBAKlonoa_LevelObjectCollection>(LevelObjectCollection, name: nameof(LevelObjectCollection)));
            }

            // Serialize fixed graphics
            Pointer fixGraphicsPointer;

            if (isWaterSki)
                fixGraphicsPointer = new Pointer(0x08070ae8, Offset.File);
            else if (isUnderWater)
                fixGraphicsPointer = new Pointer(0x08070b54, Offset.File);
            else
                fixGraphicsPointer = new Pointer(0x08070a10, Offset.File);

            s.DoAt(fixGraphicsPointer, () => FixObjectGraphics = s.SerializeObjectArray<GBAKlonoa_ObjectGraphics>(FixObjectGraphics, 9, name: nameof(FixObjectGraphics)));

            if (isMap)
            {
                // Serialize world map object graphics
                s.DoAt(new Pointer(0x081d5ec0, Offset.File), () => WorldMapObjectGraphicPointers = s.SerializePointerArray(WorldMapObjectGraphicPointers, normalWorldsCount, name: nameof(WorldMapObjectGraphicPointers)));

                WorldMapObjectGraphics ??= new GBAKlonoa_ObjectGraphics[WorldMapObjectGraphicPointers.Length][];
                s.DoAt(WorldMapObjectGraphicPointers[settings.World - 1], () =>
                {
                    WorldMapObjectGraphics[settings.World - 1] = s.SerializeObjectArrayUntil<GBAKlonoa_ObjectGraphics>(
                        obj: WorldMapObjectGraphics[settings.World - 1],
                        conditionCheckFunc: x => x.AnimationsPointerValue == 0,
                        getLastObjFunc: () => new GBAKlonoa_ObjectGraphics(),
                        name: $"{nameof(WorldMapObjectGraphics)}[{settings.World - 1}]");
                });
            }

            if (!isMap)
            {
                // Serialize graphics data
                s.DoAt(new Pointer(0x08070d40, Offset.File), () =>
                {
                    GraphicsDatas ??= new GBAKlonoa_DCT_GraphicsData[154];

                    for (int i = 0; i < GraphicsDatas.Length; i++)
                        GraphicsDatas[i] = s.SerializeObject<GBAKlonoa_DCT_GraphicsData>(GraphicsDatas[i], x =>
                        {
                            x.Pre_IsReferencedInLevel = LevelObjectCollection.Objects.Any(o => o.DCT_GraphicsIndex == i);
                        }, name: $"{nameof(GraphicsDatas)}[{i}]");
                });
            }

            // Serialize fixed OAM collections
            s.DoAt(new Pointer(0x0808ec68, Offset.File), () => FixObjectOAMCollections = s.SerializeObjectArray<GBAKlonoa_ObjectOAMCollection>(FixObjectOAMCollections, FixObjects.Length, name: nameof(FixObjectOAMCollections)));

            if (isMap)
            {
                // Serialize world map OAM collections
                s.DoAt(new Pointer(0x081d6658, Offset.File), () => WorldMapObjectOAMCollectionPointers = s.SerializePointerArray(WorldMapObjectOAMCollectionPointers, normalWorldsCount, name: nameof(WorldMapObjectOAMCollectionPointers)));

                WorldMapObjectOAMCollections ??= new GBAKlonoa_ObjectOAMCollection[WorldMapObjectOAMCollectionPointers.Length][];
                s.DoAt(WorldMapObjectOAMCollectionPointers[settings.World - 1], () =>
                {
                    var max = WorldMapObjectCollection.Objects.Max(x => x.OAMIndex);

                    WorldMapObjectOAMCollections[settings.World - 1] = s.SerializeObjectArray<GBAKlonoa_ObjectOAMCollection>(WorldMapObjectOAMCollections[settings.World - 1], max - GBAKlonoa_EOD_Manager.FixCount + 1, name: $"{nameof(WorldMapObjectOAMCollections)}[{settings.World - 1}]");
                });
            }

            // Serialize fixed object palettes
            s.DoAt(new Pointer(0x0808d988, Offset.File), () => FixObjectPalettes = s.SerializeObjectArray<GBAKlonoa_ObjPal>(FixObjectPalettes, 3, name: nameof(FixObjectPalettes)));

            // Serialize level num sprites
            if (!isMap && !isBoss)
                s.DoAt(new Pointer(0x081d6700, Offset.File), () => LevelNumSpritePointers = s.SerializePointerArray(LevelNumSpritePointers, 8, name: nameof(LevelNumSpritePointers)));
        }
    }
}