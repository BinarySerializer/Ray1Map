using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;

namespace R1Engine
{
    public class GBAKlonoa_DCT_ROM : GBA_ROMBase
    {
        // Info
        public GBAKlonoa_LevelStartInfos[] LevelStartInfos { get; set; }

        // Maps
        public GBAKlonoa_DCT_Map[] Maps { get; set; }
        public RGBA5551Color[] FixTilePalette { get; set; }
        public GBAKlonoa_MapSectors[] MapSectors { get; set; }

        // Objects
        public GBAKlonoa_LoadedObject[] FixObjects { get; set; }
        public GBAKlonoa_LevelObjectCollection LevelObjectCollection { get; set; } // For current level only - too slow to read all of them
        public GBAKlonoa_WorldMapObjectCollection WorldMapObjectCollection { get; set; }
        public GBAKlonoa_ObjectGraphics[] FixObjectGraphics { get; set; }
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

            var settings = s.GetR1Settings();
            var globalLevelIndex = GBAKlonoa_DCT_Manager.GetGlobalLevelIndex(settings.World, settings.Level);
            const int normalWorldsCount = GBAKlonoa_DCT_Manager.NormalWorldsCount;
            const int levelsCount = GBAKlonoa_DCT_Manager.LevelsCount;
            const int normalLevelsCount = GBAKlonoa_DCT_Manager.NormalLevelsCount;
            var isMap = settings.Level == 0;
            var isBoss = settings.Level == 9;
            var isWaterSki = settings.Level == 4;
            var isUnderWater = settings.World == 4 && (settings.Level == 5 || settings.Level == 1 || settings.Level == 7);

            // Serialize level start positions
            if (!isMap && !isWaterSki)
                s.DoAt(new Pointer(0x0810ca00, Offset.File), () => LevelStartInfos = s.SerializeObjectArray<GBAKlonoa_LevelStartInfos>(LevelStartInfos, normalLevelsCount, name: nameof(LevelStartInfos)));

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

            if (!isMap)
            {
                // Serialize graphics data
                s.DoAt(new Pointer(0x08070d40, Offset.File), () => GraphicsDatas = s.SerializeObjectArray<GBAKlonoa_DCT_GraphicsData>(GraphicsDatas, 154, name: nameof(GraphicsDatas)));
            }

            // Serialize fixed OAM collections
            s.DoAt(new Pointer(0x0808ec68, Offset.File), () => FixObjectOAMCollections = s.SerializeObjectArray<GBAKlonoa_ObjectOAMCollection>(FixObjectOAMCollections, FixObjects.Max(x => x.OAMIndex) + 1, name: nameof(FixObjectOAMCollections)));

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