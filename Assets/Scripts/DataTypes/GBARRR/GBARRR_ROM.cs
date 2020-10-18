using System;

namespace R1Engine
{
    public class GBARRR_ROM : GBA_ROMBase
    {
        // Global data
        public GBARRR_OffsetTable OffsetTable { get; set; }
        public GBARRR_LevelInfo[] VillageLevelInfo { get; set; }
        public GBARRR_LevelInfo[] LevelInfo { get; set; }
        public GBARRR_LocalizationBlock Localization { get; set; }

        // Tilesets
        public GBARRR_Tileset LevelTileset { get; set; }
        public GBARRR_Tileset BG0TileSet { get; set; }
        public GBARRR_Tileset FGTileSet { get; set; }
        public GBARRR_Tileset BG1TileSet { get; set; }

        // Map data
        public GBARRR_BGMapBlock BG0Map { get; set; }
        public GBARRR_BGMapBlock BG1Map { get; set; }
        public GBARRR_Scene LevelScene { get; set; }
        public GBARRR_MapBlock CollisionMap { get; set; }
        public GBARRR_MapBlock LevelMap { get; set; }
        public GBARRR_MapBlock FGMap { get; set; }

        // Palettes
        public ARGB1555Color[] TilePalette { get; set; }
        public ARGB1555Color[] SpritePalette { get; set; }

        // Tables
        public GBARRR_GraphicsTableEntry[][] GraphicsTable0 { get; set; }
        public uint[][] GraphicsTable1 { get; set; }
        public uint[][] GraphicsTable2 { get; set; }
        public uint[][] GraphicsTable3 { get; set; }
        public uint[][] GraphicsTable4 { get; set; }

        // Mode7
        public Pointer[] Mode7_MapTilesPointers { get; set; }
        public Pointer[] Mode7_BG1TilesPointers { get; set; }
        public Pointer[] Mode7_Unk1TilesPointers { get; set; }
        public Pointer[] Mode7_BG0TilesPointers { get; set; }
        public Pointer[] Mode7_Unk2TilesPointers { get; set; }
        public Pointer[] Mode7_MapPointers { get; set; }
        public Pointer[] Mode7_CollisionMapPointers { get; set; }
        public Pointer[] Mode7_TilePalettePointers { get; set; }
        public Pointer[] Mode7_SpritePalette1Pointers { get; set; }
        public Pointer[] Mode7_SpritePalette2Pointers { get; set; }
        public byte[] Mode7_MapTiles { get; set; }
        public byte[] Mode7_BG1Tiles { get; set; }
        public byte[] Mode7_Unk1Tiles { get; set; }
        public byte[] Mode7_BG0Tiles { get; set; }
        public byte[] Mode7_Unk2Tiles { get; set; }
        public MapTile[] Mode7_MapData { get; set; }
        public MapTile[] Mode7_CollisionMapData { get; set; }
        public ARGB1555Color[] Mode7_TilePalette { get; set; }
        public ARGB1555Color[] Mode7_SpritePalette1 { get; set; }
        public ARGB1555Color[] Mode7_SpritePalette2 { get; set; }


        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get pointer table
            var pointerTable = PointerTables.GBARRR_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize offset table
            OffsetTable = s.DoAt(pointerTable[GBARRR_Pointer.OffsetTable], () => s.SerializeObject<GBARRR_OffsetTable>(OffsetTable, name: nameof(OffsetTable)));

            // Serialize localization
            OffsetTable.DoAtBlock(s.Context, 3, size =>
                Localization = s.SerializeObject<GBARRR_LocalizationBlock>(Localization, name: nameof(Localization)));

            if (GBA_RRR_Manager.GetCurrentGameMode(s.GameSettings) != GBA_RRR_Manager.GameMode.Mode7)
            {
                // Serialize level info
                VillageLevelInfo = s.DoAt(pointerTable[GBARRR_Pointer.VillageLevelInfo],
                    () => s.SerializeObjectArray<GBARRR_LevelInfo>(VillageLevelInfo, 3,
                        name: nameof(VillageLevelInfo)));
                LevelInfo = s.DoAt(pointerTable[GBARRR_Pointer.LevelInfo],
                    () => s.SerializeObjectArray<GBARRR_LevelInfo>(LevelInfo, 32, name: nameof(LevelInfo)));

                // Get the current level info
                var lvlInfo = GetLevelInfo(s.GameSettings);

                // Serialize tile maps
                OffsetTable.DoAtBlock(s.Context, lvlInfo.LevelTilesetIndex, size =>
                    LevelTileset = s.SerializeObject<GBARRR_Tileset>(LevelTileset, name: nameof(LevelTileset),
                        onPreSerialize: x => x.BlockSize = size));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.BG0TilesetIndex, size =>
                    BG0TileSet = s.SerializeObject<GBARRR_Tileset>(BG0TileSet, name: nameof(BG0TileSet),
                        onPreSerialize: x => x.BlockSize = size));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.FGTilesetIndex, size =>
                    FGTileSet = s.SerializeObject<GBARRR_Tileset>(FGTileSet, name: nameof(FGTileSet),
                        onPreSerialize: x => x.BlockSize = size));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.BG1TilesetIndex, size =>
                    BG1TileSet = s.SerializeObject<GBARRR_Tileset>(BG1TileSet, name: nameof(BG1TileSet),
                        onPreSerialize: x => x.BlockSize = size));

                // Serialize level scene
                OffsetTable.DoAtBlock(s.Context, lvlInfo.SceneIndex,
                    size => LevelScene = s.SerializeObject<GBARRR_Scene>(LevelScene, name: nameof(LevelScene)));

                // Serialize maps
                OffsetTable.DoAtBlock(s.Context, lvlInfo.BG0MapIndex, size =>
                    BG0Map = s.SerializeObject<GBARRR_BGMapBlock>(BG0Map, name: nameof(BG0Map)));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.BG1MapIndex, size =>
                    BG1Map = s.SerializeObject<GBARRR_BGMapBlock>(BG1Map, name: nameof(BG1Map)));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.CollisionMapIndex, size =>
                    CollisionMap = s.SerializeObject<GBARRR_MapBlock>(CollisionMap, name: nameof(CollisionMap),
                        onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Collision));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.LevelMapIndex, size =>
                    LevelMap = s.SerializeObject<GBARRR_MapBlock>(LevelMap, name: nameof(LevelMap),
                        onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Tiles));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.FGMapIndex, size =>
                    FGMap = s.SerializeObject<GBARRR_MapBlock>(FGMap, name: nameof(FGMap),
                        onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Foreground));

                // Serialize palettes
                var tilePalIndex = GetLevelTilePaletteOffsetIndex(s.GameSettings);
                if (tilePalIndex != null)
                    OffsetTable.DoAtBlock(s.Context, tilePalIndex.Value, size =>
                        TilePalette = s.SerializeObjectArray<ARGB1555Color>(TilePalette, 0x100, name: nameof(TilePalette)));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.SpritePaletteIndex, size =>
                    SpritePalette = s.SerializeObjectArray<ARGB1555Color>(SpritePalette, 0x100, name: nameof(SpritePalette)));

                // Serialize tables
                s.DoAt(pointerTable[GBARRR_Pointer.GraphicsTables], () =>
                {
                    var counts = new uint[] {0x47, 0x40, 0x45, 0x44, 0x50, 0x42};

                    if (GraphicsTable0 == null) GraphicsTable0 = new GBARRR_GraphicsTableEntry[counts.Length][];
                    if (GraphicsTable1 == null) GraphicsTable1 = new uint[counts.Length][];
                    if (GraphicsTable2 == null) GraphicsTable2 = new uint[counts.Length][];
                    if (GraphicsTable3 == null) GraphicsTable3 = new uint[counts.Length][];
                    if (GraphicsTable4 == null) GraphicsTable4 = new uint[counts.Length][];

                    for (int i = 0; i < counts.Length; i++)
                    {
                        GraphicsTable0[i] = s.SerializeObjectArray<GBARRR_GraphicsTableEntry>(GraphicsTable0[i],
                            counts[i], name: $"{nameof(GraphicsTable0)}[{i}]");
                        GraphicsTable1[i] = s.SerializeArray<uint>(GraphicsTable1[i], counts[i],
                            name: $"{nameof(GraphicsTable1)}[{i}]");
                        GraphicsTable2[i] = s.SerializeArray<uint>(GraphicsTable2[i], counts[i],
                            name: $"{nameof(GraphicsTable2)}[{i}]");
                        GraphicsTable3[i] = s.SerializeArray<uint>(GraphicsTable3[i], counts[i],
                            name: $"{nameof(GraphicsTable3)}[{i}]");
                        GraphicsTable4[i] = s.SerializeArray<uint>(GraphicsTable4[i], counts[i],
                            name: $"{nameof(GraphicsTable4)}[{i}]");
                        if (i == 2 || i == 3) s.Serialize<uint>(1, name: "Padding");
                    }
                });
            }
            else
            {
                Mode7_MapTilesPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_MapTiles], () => s.SerializePointerArray(Mode7_MapTilesPointers, 3, name: nameof(Mode7_MapTilesPointers)));
                Mode7_BG1TilesPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_BG1Tiles], () => s.SerializePointerArray(Mode7_BG1TilesPointers, 3, name: nameof(Mode7_BG1TilesPointers)));
                Mode7_Unk1TilesPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_Unk1Tiles], () => s.SerializePointerArray(Mode7_Unk1TilesPointers, 3, name: nameof(Mode7_Unk1TilesPointers)));
                Mode7_BG0TilesPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_BG0Tiles], () => s.SerializePointerArray(Mode7_BG0TilesPointers, 3, name: nameof(Mode7_BG0TilesPointers)));
                Mode7_Unk2TilesPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_Unk2Tiles], () => s.SerializePointerArray(Mode7_Unk2TilesPointers, 3, name: nameof(Mode7_Unk2TilesPointers)));
                Mode7_MapPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_MapData], () => s.SerializePointerArray(Mode7_MapPointers, 3, name: nameof(Mode7_MapPointers)));
                Mode7_CollisionMapPointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_CollisionMapData], () => s.SerializePointerArray(Mode7_CollisionMapPointers, 3, name: nameof(Mode7_CollisionMapPointers)));
                Mode7_TilePalettePointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_TilePalette], () => s.SerializePointerArray(Mode7_TilePalettePointers, 3, name: nameof(Mode7_TilePalettePointers)));
                Mode7_SpritePalette1Pointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_SpritePalette1], () => s.SerializePointerArray(Mode7_SpritePalette1Pointers, 3, name: nameof(Mode7_SpritePalette1Pointers)));
                Mode7_SpritePalette2Pointers = s.DoAt(pointerTable[GBARRR_Pointer.Mode7_SpritePalette2], () => s.SerializePointerArray(Mode7_SpritePalette2Pointers, 3, name: nameof(Mode7_SpritePalette2Pointers)));

                // Compressed
                s.DoAt(Mode7_MapTilesPointers[s.GameSettings.Level], () => {
                    s.DoEncoded(new RNCEncoder(hasHeader: false), () => Mode7_MapTiles = s.SerializeArray<byte>(Mode7_MapTiles, s.CurrentLength, name: nameof(Mode7_MapTiles)));
                });
                //Mode7_BG1Tiles =
                //Mode7_Unk1Tiles =
                //Mode7_BG0Tiles =
                //Mode7_Unk2Tiles =
                Mode7_MapData = s.DoAt(Mode7_MapPointers[s.GameSettings.Level], () => s.SerializeObjectArray<MapTile>(Mode7_MapData, 256 * 256, onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Mode7Tiles, name: nameof(Mode7_MapData)));
                Mode7_CollisionMapData = s.DoAt(Mode7_CollisionMapPointers[s.GameSettings.Level], () => s.SerializeObjectArray<MapTile>(Mode7_CollisionMapData, 256 * 256, onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Collision, name: nameof(Mode7_CollisionMapData)));
                Mode7_TilePalette = s.DoAt(Mode7_TilePalettePointers[s.GameSettings.Level], () => s.SerializeObjectArray<ARGB1555Color>(Mode7_TilePalette, 16 * 16, name: nameof(Mode7_TilePalette)));
                Mode7_SpritePalette1 = s.DoAt(Mode7_SpritePalette1Pointers[s.GameSettings.Level], () => s.SerializeObjectArray<ARGB1555Color>(Mode7_SpritePalette1, 16, name: nameof(Mode7_SpritePalette1)));
                Mode7_SpritePalette2 = s.DoAt(Mode7_SpritePalette2Pointers[s.GameSettings.Level], () => s.SerializeObjectArray<ARGB1555Color>(Mode7_SpritePalette2, 16, name: nameof(Mode7_SpritePalette2)));
            }
        }

        public GBARRR_LevelInfo GetLevelInfo(GameSettings settings)
        {
            switch (GBA_RRR_Manager.GetCurrentGameMode(settings))
            {
                case GBA_RRR_Manager.GameMode.Game:
                    return LevelInfo[settings.Level];

                case GBA_RRR_Manager.GameMode.Village:
                    return VillageLevelInfo[settings.Level];

                case GBA_RRR_Manager.GameMode.Mode7:
                    throw new Exception("Mode7 maps do not use level info");
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Recreated from level load function
        public int? GetLevelTilePaletteOffsetIndex(GameSettings settings)
        {
            if (GBA_RRR_Manager.GetCurrentGameMode(settings) == GBA_RRR_Manager.GameMode.Village)
            {
                switch (settings.Level)
                {
                    case 0:
                        return 0x3A6;

                    case 1:
                        return 0x3A7;

                    case 2:
                        return 0x3A8;
                }
            }

            switch (settings.Level)
            {
                case 0:
                case 24:
                    return 0x395;

                case 1:
                    return 0x396;

                case 2:
                    return 0x38F;

                case 3:
                    return 0x390;

                // 4 is Mode7

                case 5:
                    return 0x38E;

                case 6:
                    return 0x3A2;

                // 7 ?
                // 8 ?

                case 9:
                    return 0x393;

                case 10:
                    return 0x391;

                case 11:
                    return 0x3A3;

                // 12 is Mode7

                case 13:
                    return 0x392;

                // 14 ?
                // 15 ?

                case 16:
                    return 0x3A4;

                // 17 ?
                // 18 is Mode7
                // 19 ?
                // 20 ?
                // 21 ?
                // 22 ?

                case 23:
                    return 0x398;

                // 25 ?

                case 26:
                    return 0x397;

                case 27:
                    return 0x394;

                // 28 is village

                case 29:
                case 31:
                    return 0x3A9;
            }

            return null;
        }
    }
}