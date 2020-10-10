using System;

namespace R1Engine
{
    public class GBARRR_ROM : GBA_ROMBase
    {
        // Global data
        public GBARRR_OffsetTable OffsetTable { get; set; }
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
        //public ARGB1555Color[] TilePalette { get; set; } // TODO: Use GetLevelTilePaletteOffsetIndex to get correct tile palette
        public ARGB1555Color[] SpritePalette { get; set; }

        // Tables
        public GBARRR_GraphicsTableEntry[][] GraphicsTable0 { get; set; }
        public uint[][] GraphicsTable1 { get; set; }
        public uint[][] GraphicsTable2 { get; set; }
        public uint[][] GraphicsTable3 { get; set; }
        public uint[][] GraphicsTable4 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get pointer table
            var pointerTable = PointerTables.GBARRR_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize offset table
            OffsetTable = s.DoAt(pointerTable[GBARRR_Pointer.OffsetTable], () => s.SerializeObject<GBARRR_OffsetTable>(OffsetTable, name: nameof(OffsetTable)));

            // Serialize level info
            LevelInfo = s.DoAt(pointerTable[GBARRR_Pointer.LevelInfo], () => s.SerializeObjectArray<GBARRR_LevelInfo>(LevelInfo, 35, name: nameof(LevelInfo)));

            // Serialize localization
            OffsetTable.DoAtBlock(s.Context, 3, size =>
                Localization = s.SerializeObject<GBARRR_LocalizationBlock>(Localization, name: nameof(Localization)));

            // Serialize tile maps
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].LevelTilesetIndex, size =>
                LevelTileset = s.SerializeObject<GBARRR_Tileset>(LevelTileset, name: nameof(LevelTileset), onPreSerialize: x => x.BlockSize = size));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].BG0TilesetIndex, size =>
                BG0TileSet = s.SerializeObject<GBARRR_Tileset>(BG0TileSet, name: nameof(BG0TileSet), onPreSerialize: x => x.BlockSize = size));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].FGTilesetIndex, size =>
                FGTileSet = s.SerializeObject<GBARRR_Tileset>(FGTileSet, name: nameof(FGTileSet), onPreSerialize: x => x.BlockSize = size));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].BG1TilesetIndex, size =>
                BG1TileSet = s.SerializeObject<GBARRR_Tileset>(BG1TileSet, name: nameof(BG1TileSet), onPreSerialize: x => x.BlockSize = size));

            // Serialize level scene
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].SceneIndex, size => LevelScene = s.SerializeObject<GBARRR_Scene>(LevelScene, name: nameof(LevelScene)));

            // Serialize maps
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].BG0MapIndex, size => 
                BG0Map = s.SerializeObject<GBARRR_BGMapBlock>(BG0Map, name: nameof(BG0Map)));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].BG1MapIndex, size => 
                BG1Map = s.SerializeObject<GBARRR_BGMapBlock>(BG1Map, name: nameof(BG1Map)));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].CollisionMapIndex, size => 
                CollisionMap = s.SerializeObject<GBARRR_MapBlock>(CollisionMap, name: nameof(CollisionMap), onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Collision));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].LevelMapIndex, size => 
                LevelMap = s.SerializeObject<GBARRR_MapBlock>(LevelMap, name: nameof(LevelMap), onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Tiles));
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].FGMapIndex, size => 
                FGMap = s.SerializeObject<GBARRR_MapBlock>(FGMap, name: nameof(FGMap), onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Foreground));

            // Serialize sprite palette
            OffsetTable.DoAtBlock(s.Context, LevelInfo[s.GameSettings.Level].SpritePaletteIndex, size =>
                SpritePalette = s.SerializeObjectArray<ARGB1555Color>(SpritePalette, 0x100, name: nameof(SpritePalette)));

            // Serialize tables
            s.DoAt(pointerTable[GBARRR_Pointer.GraphicsTables], () =>
            {
                var counts = new uint[] { 0x47, 0x40, 0x45, 0x44, 0x50, 0x42 };

                if (GraphicsTable0 == null) GraphicsTable0 = new GBARRR_GraphicsTableEntry[counts.Length][];
                if (GraphicsTable1 == null) GraphicsTable1 = new uint[counts.Length][];
                if (GraphicsTable2 == null) GraphicsTable2 = new uint[counts.Length][];
                if (GraphicsTable3 == null) GraphicsTable3 = new uint[counts.Length][];
                if (GraphicsTable4 == null) GraphicsTable4 = new uint[counts.Length][];

                for (int i = 0; i < counts.Length; i++)
                {
                    GraphicsTable0[i] = s.SerializeObjectArray<GBARRR_GraphicsTableEntry>(GraphicsTable0[i], counts[i], name: $"{nameof(GraphicsTable0)}[{i}]");
                    GraphicsTable1[i] = s.SerializeArray<uint>(GraphicsTable1[i], counts[i], name: $"{nameof(GraphicsTable1)}[{i}]");
                    GraphicsTable2[i] = s.SerializeArray<uint>(GraphicsTable2[i], counts[i], name: $"{nameof(GraphicsTable2)}[{i}]");
                    GraphicsTable3[i] = s.SerializeArray<uint>(GraphicsTable3[i], counts[i], name: $"{nameof(GraphicsTable3)}[{i}]");
                    GraphicsTable4[i] = s.SerializeArray<uint>(GraphicsTable4[i], counts[i], name: $"{nameof(GraphicsTable4)}[{i}]");
                }
            });
        }

        // Recreated from level load function
        public int? GetLevelTilePaletteOffsetIndex(int level)
        {
            switch (level)
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

                // 28 uses 0x3A6, 0x3A7 and 0x3A8

                case 29:
                case 31:
                    return 0x3A9;
            }

            return null;
        }
    }
}