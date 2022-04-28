﻿using System;
using BinarySerializer;
using BinarySerializer.Nintendo;

namespace Ray1Map.GBARRR
{
    public class GBARRR_ROM : GBA_ROMBase
    {
        // Global data
        public GBARRR_OffsetTable OffsetTable { get; set; }
        public GBARRR_LevelInfo[] VillageLevelInfo { get; set; }
        public GBARRR_LevelInfo[] LevelInfo { get; set; }
        public GBARRR_LevelProperties[] LevelProperties { get; set; }
        public GBARRR_LocalizationBlock Localization { get; set; }

        // Tilesets
        public GBARRR_Tileset LevelTileset { get; set; }
        public GBARRR_Tileset BG0TileSet { get; set; }
        public GBARRR_Tileset FGTileSet { get; set; }
        public GBARRR_Tileset BG1TileSet { get; set; }

        // Map data
        public GBARRR_BGMapBlock BG0Map { get; set; }
        public GBARRR_BGMapBlock BG1Map { get; set; }
        public GBARRR_ObjectArray ObjectArray { get; set; }
        public GBARRR_MapBlock CollisionMap { get; set; }
        public GBARRR_MapBlock LevelMap { get; set; }
        public GBARRR_MapBlock FGMap { get; set; }
        public RGBA5551Color[][] AnimatedPalettes { get; set; }

        // Palettes
        public RGBA5551Color[] TilePalette { get; set; }
        public RGBA5551Color[] SpritePalette { get; set; }

        // Tables
        public GBARRR_GraphicsTableEntry[][] GraphicsTable0 { get; set; }
        public uint[][] GraphicsTable1 { get; set; }
        public uint[][] GraphicsTable2 { get; set; }
        public uint[][] GraphicsTable3 { get; set; }
        public uint[][] GraphicsTable4 { get; set; }

        // Mode7
        public Pointer[] Mode7_MapTilesPointers { get; set; }
        public Pointer[] Mode7_BG1TilesPointers { get; set; }
        public Pointer[] Mode7_BG1MapPointers { get; set; }
        public Pointer[] Mode7_BG0TilesPointers { get; set; }
        public Pointer[] Mode7_BG0MapPointers { get; set; }
        public Pointer[] Mode7_MapPointers { get; set; }
        public Pointer[] Mode7_CollisionMapDataPointers { get; set; }
        public Pointer[] Mode7_MapPalettePointers { get; set; }
        public Pointer[] Mode7_BG1PalettePointers { get; set; }
        public Pointer[] Mode7_BG0PalettePointers { get; set; }
        public Pointer[] Mode7_ObjectsPointers { get; set; }
        public Pointer[] Mode7_CollisionTypesPointers { get; set; }
        public byte[] Mode7_BG0Tiles { get; set; }
        public byte[] Mode7_BG1Tiles { get; set; }
        public byte[] Mode7_MapTiles { get; set; }
        public MapTile[] Mode7_BG0MapData { get; set; }
        public MapTile[] Mode7_BG1MapData { get; set; }
        public MapTile[] Mode7_MapData { get; set; }
        public RGBA5551Color[] Mode7_MapPalette { get; set; }
        public RGBA5551Color[] Mode7_BG1Palette { get; set; }
        public RGBA5551Color[] Mode7_BG0Palette { get; set; }
        public RGBA5551Color[] Mode7_TilemapPalette { get; set; }
        public GBARRR_Mode7Object[] Mode7_Objects { get; set; }
        public byte[] Mode7_CollisionTypes { get; set; }
        public ushort[] Mode7_CollisionMapData { get; set; }

        public Pointer[] Mode7_WaypointsPointers { get; set; }
        public short[] Mode7_WaypointsCount { get; set; }
        public GBARRR_Mode7Waypoint[] Mode7_Waypoints { get; set; }

        // Menu
        public Pointer[] Menu_Pointers { get; set; }
        public byte[][] Menu_Tiles { get; set; }
        public MapTile[][] Menu_MapData { get; set; }
        public RGBA5551Color[][] Menu_Palette { get; set; }


        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get pointer table
            var pointerTable = PointerTables.GBARRR_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            // Serialize offset table
            OffsetTable = s.DoAt(pointerTable[DefinedPointer.OffsetTable], () => s.SerializeObject<GBARRR_OffsetTable>(OffsetTable, name: nameof(OffsetTable)));

            // Serialize localization
            OffsetTable.DoAtBlock(s.Context, 3, size =>
                Localization = s.SerializeObject<GBARRR_LocalizationBlock>(Localization, name: nameof(Localization)));

            var gameMode = GBA_RRR_Manager.GetCurrentGameMode(s.GetR1Settings());

            if (gameMode == GBA_RRR_Manager.GameMode.Game || gameMode == GBA_RRR_Manager.GameMode.Village)
            {
                // Serialize level info
                VillageLevelInfo = s.DoAt(pointerTable[DefinedPointer.VillageLevelInfo],
                    () => s.SerializeObjectArray<GBARRR_LevelInfo>(VillageLevelInfo, 3,
                        name: nameof(VillageLevelInfo)));
                LevelInfo = s.DoAt(pointerTable[DefinedPointer.LevelInfo],
                    () => s.SerializeObjectArray<GBARRR_LevelInfo>(LevelInfo, 32, name: nameof(LevelInfo)));
                LevelProperties = s.DoAt(pointerTable[DefinedPointer.LevelProperties],
                    () => s.SerializeObjectArray<GBARRR_LevelProperties>(LevelProperties, 32, name: nameof(LevelProperties)));

                // Get the current level info
                var lvlInfo = GetLevelInfo(s.GetR1Settings());

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
                OffsetTable.DoAtBlock(s.Context, lvlInfo.ObjectArrayIndex,
                    size => ObjectArray = s.SerializeObject<GBARRR_ObjectArray>(ObjectArray, name: nameof(ObjectArray)));

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
                var tilePalIndex = GetLevelTilePaletteOffsetIndex(s.GetR1Settings());
                if (tilePalIndex != null)
                    OffsetTable.DoAtBlock(s.Context, tilePalIndex.Value, size =>
                        TilePalette = s.SerializeObjectArray<RGBA5551Color>(TilePalette, 0x100, name: nameof(TilePalette)));
                OffsetTable.DoAtBlock(s.Context, lvlInfo.SpritePaletteIndex, size =>
                    SpritePalette = s.SerializeObjectArray<RGBA5551Color>(SpritePalette, 0x100, name: nameof(SpritePalette)));

                if (AnimatedPalettes == null)
                    AnimatedPalettes = new RGBA5551Color[5][];

                for (int i = 0; i < AnimatedPalettes.Length; i++)
                    OffsetTable.DoAtBlock(s.Context, 764 + i, size => 
                        AnimatedPalettes[i] = s.SerializeObjectArray<RGBA5551Color>(AnimatedPalettes[i], 0x100, name: $"{nameof(AnimatedPalettes)}[{i}]"));

                // Serialize tables
                s.DoAt(pointerTable[DefinedPointer.GraphicsTables], () =>
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
            else if (gameMode == GBA_RRR_Manager.GameMode.Mode7)
            {
                // Serialize pointer tables
                Mode7_MapTilesPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_MapTiles], () => s.SerializePointerArray(Mode7_MapTilesPointers, 3, name: nameof(Mode7_MapTilesPointers)));
                Mode7_BG1TilesPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_BG1Tiles], () => s.SerializePointerArray(Mode7_BG1TilesPointers, 3, name: nameof(Mode7_BG1TilesPointers)));
                Mode7_BG1MapPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_Bg1Map], () => s.SerializePointerArray(Mode7_BG1MapPointers, 3, name: nameof(Mode7_BG1MapPointers)));
                Mode7_BG0TilesPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_BG0Tiles], () => s.SerializePointerArray(Mode7_BG0TilesPointers, 3, name: nameof(Mode7_BG0TilesPointers)));
                Mode7_BG0MapPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_BG0Map], () => s.SerializePointerArray(Mode7_BG0MapPointers, 3, name: nameof(Mode7_BG0MapPointers)));
                Mode7_MapPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_MapData], () => s.SerializePointerArray(Mode7_MapPointers, 3, name: nameof(Mode7_MapPointers)));
                Mode7_CollisionMapDataPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_CollisionMapData], () => s.SerializePointerArray(Mode7_CollisionMapDataPointers, 3, name: nameof(Mode7_CollisionMapDataPointers)));
                Mode7_MapPalettePointers = s.DoAt(pointerTable[DefinedPointer.Mode7_TilePalette], () => s.SerializePointerArray(Mode7_MapPalettePointers, 3, name: nameof(Mode7_MapPalettePointers)));
                Mode7_BG1PalettePointers = s.DoAt(pointerTable[DefinedPointer.Mode7_BG1Palette], () => s.SerializePointerArray(Mode7_BG1PalettePointers, 3, name: nameof(Mode7_BG1PalettePointers)));
                Mode7_BG0PalettePointers = s.DoAt(pointerTable[DefinedPointer.Mode7_BG0Palette], () => s.SerializePointerArray(Mode7_BG0PalettePointers, 3, name: nameof(Mode7_BG0PalettePointers)));
                Mode7_ObjectsPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_Objects], () => s.SerializePointerArray(Mode7_ObjectsPointers, 3, name: nameof(Mode7_ObjectsPointers)));
                Mode7_CollisionTypesPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_CollisionTypesArray], () => s.SerializePointerArray(Mode7_CollisionTypesPointers, 3, name: nameof(Mode7_CollisionTypesPointers)));
                Mode7_WaypointsCount = s.DoAt(pointerTable[DefinedPointer.Mode7_WaypointsCount], () => s.SerializeArray<short>(Mode7_WaypointsCount, 3, name: nameof(Mode7_WaypointsCount)));
                Mode7_WaypointsPointers = s.DoAt(pointerTable[DefinedPointer.Mode7_Waypoints], () => s.SerializePointerArray(Mode7_WaypointsPointers, 3, name: nameof(Mode7_WaypointsPointers)));

                // Serialize compressed tile data
                s.DoAt(Mode7_MapTilesPointers[s.GetR1Settings().Level], () => {
                    s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Mode7_MapTiles = s.SerializeArray<byte>(Mode7_MapTiles, s.CurrentLength, name: nameof(Mode7_MapTiles)));
                });
                s.DoAt(Mode7_BG0TilesPointers[s.GetR1Settings().Level], () => {
                    s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Mode7_BG0Tiles = s.SerializeArray<byte>(Mode7_BG0Tiles, s.CurrentLength, name: nameof(Mode7_BG0Tiles)));
                });
                s.DoAt(Mode7_BG1TilesPointers[s.GetR1Settings().Level], () => {
                    s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Mode7_BG1Tiles = s.SerializeArray<byte>(Mode7_BG1Tiles, s.CurrentLength, name: nameof(Mode7_BG1Tiles)));
                });

                // Serialize map data
                if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanRavingRabbidsGBAEU)
                {
                    Mode7_MapData = s.DoAt(Mode7_MapPointers[s.GetR1Settings().Level], () => s.SerializeObjectArray<MapTile>(Mode7_MapData, 256 * 256, onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Mode7Tiles, name: nameof(Mode7_MapData)));
                }
                else
                {
                    s.DoAt(Mode7_MapPointers[s.GetR1Settings().Level], () =>
                    {
                        s.DoEncoded(new RNC2Encoder(hasHeader: false), () =>
                            Mode7_MapData = s.SerializeObjectArray<MapTile>(Mode7_MapData, 256 * 256, onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Mode7Tiles, name: nameof(Mode7_MapData)));
                    });
                }
                s.DoAt(Mode7_BG0MapPointers[s.GetR1Settings().Level], () =>
                {
                    s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Mode7_BG0MapData = s.SerializeObjectArray<MapTile>(Mode7_BG0MapData, 32 * 32,
                        onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Foreground, name: nameof(Mode7_BG0MapData)));
                });
                s.DoAt(Mode7_BG1MapPointers[s.GetR1Settings().Level], () =>
                {
                    s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Mode7_BG1MapData = s.SerializeObjectArray<MapTile>(Mode7_BG1MapData, 32 * 32,
                        onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Foreground,
                        name: nameof(Mode7_BG1MapData)));
                });
                s.DoAt(Mode7_ObjectsPointers[s.GetR1Settings().Level], () =>
                {
                    s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Mode7_Objects = s.SerializeObjectArray<GBARRR_Mode7Object>(Mode7_Objects, 141, name: nameof(Mode7_Objects)));
                });
                if (s.GetR1Settings().GameModeSelection == GameModeSelection.RaymanRavingRabbidsGBAEU) {
                    s.DoAt(Mode7_CollisionMapDataPointers[s.GetR1Settings().Level], () => {
                        Mode7_CollisionMapData = s.SerializeArray<ushort>(Mode7_CollisionMapData, 256 * 256, name: nameof(Mode7_CollisionMapData));
                    });
                } else {
                    s.DoAt(Mode7_CollisionMapDataPointers[s.GetR1Settings().Level], () => {
                        s.DoEncoded(new RNC2Encoder(hasHeader: false), () =>
                            Mode7_CollisionMapData = s.SerializeArray<ushort>(Mode7_CollisionMapData, 256 * 256, name: nameof(Mode7_CollisionMapData)));
                    });
                }
                s.DoAt(Mode7_CollisionTypesPointers[s.GetR1Settings().Level], () => {
                    s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Mode7_CollisionTypes = s.SerializeArray<byte>(Mode7_CollisionTypes, s.CurrentLength, name: nameof(Mode7_CollisionTypes)));
                });
                s.DoAt(Mode7_WaypointsPointers[s.GetR1Settings().Level], () => {
                    Mode7_Waypoints = s.SerializeObjectArray<GBARRR_Mode7Waypoint>(Mode7_Waypoints, Mode7_WaypointsCount[s.GetR1Settings().Level], name: nameof(Mode7_Waypoints));
                });

                // Serialize palettes
                Mode7_MapPalette = s.DoAt(Mode7_MapPalettePointers[s.GetR1Settings().Level], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_MapPalette, 16 * 16, name: nameof(Mode7_MapPalette)));
                Mode7_BG1Palette = s.DoAt(Mode7_BG1PalettePointers[s.GetR1Settings().Level], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_BG1Palette, 16, name: nameof(Mode7_BG1Palette)));
                Mode7_BG0Palette = s.DoAt(Mode7_BG0PalettePointers[s.GetR1Settings().Level], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_BG0Palette, 16, name: nameof(Mode7_BG0Palette)));

                // Fill in full tilemap palette
                Mode7_TilemapPalette = new RGBA5551Color[16 * 16];

                for (int i = 0; i < 12 * 16; i++)
                    Mode7_TilemapPalette[i] = Mode7_MapPalette[i];
                for (int i = 0; i < 16; i++)
                    Mode7_TilemapPalette[12 * 16 + i] = Mode7_BG0Palette[i];
                for (int i = 0; i < 16; i++)
                    Mode7_TilemapPalette[14 * 16 + i] = Mode7_BG1Palette[i];
            }
            else if (gameMode == GBA_RRR_Manager.GameMode.Mode7Unused)
            {
                OffsetTable.DoAtBlock(s.Context, 1180, size =>
                {
                    ObjectArray = s.SerializeObject<GBARRR_ObjectArray>(ObjectArray, onPreSerialize: x => x.IsUnusedMode7 = true, name: nameof(ObjectArray));
                    Mode7_MapData = s.SerializeObjectArray<MapTile>(Mode7_MapData, 256 * 256, onPreSerialize: x => x.GBARRRType = GBARRR_MapBlock.MapType.Foreground, name: nameof(Mode7_MapData));
                });
                OffsetTable.DoAtBlock(s.Context, 1181, size =>
                    CollisionMap = s.SerializeObject<GBARRR_MapBlock>(CollisionMap, name: nameof(CollisionMap),
                        onPreSerialize: x => x.Type = GBARRR_MapBlock.MapType.Collision));

                OffsetTable.DoAtBlock(s.Context, 1177, size => BG0TileSet = s.SerializeObject<GBARRR_Tileset>(BG0TileSet, onPreSerialize: x => x.BlockSize = size, name: nameof(BG0TileSet)));
                OffsetTable.DoAtBlock(s.Context, 1178, size => BG1TileSet = s.SerializeObject<GBARRR_Tileset>(BG1TileSet, onPreSerialize: x => x.BlockSize = size, name: nameof(BG1TileSet)));
                OffsetTable.DoAtBlock(s.Context, 1179, size => LevelTileset = s.SerializeObject<GBARRR_Tileset>(LevelTileset, onPreSerialize: x => x.BlockSize = size, name: nameof(LevelTileset)));
            }
            else if (gameMode == GBA_RRR_Manager.GameMode.Menu)
            {
                Menu_Pointers = s.DoAt(pointerTable[DefinedPointer.MenuArray], () => s.SerializePointerArray(Menu_Pointers, 15 * 3, name: nameof(Menu_Pointers)));

                var manager = (GBA_RRR_Manager)s.GetR1Settings().GetGameManager;
                var menuLevels = manager.GetMenuLevels(s.GetR1Settings().Level);

                Menu_Tiles = new byte[menuLevels.Length][];
                Menu_MapData = new MapTile[menuLevels.Length][];
                Menu_Palette = new RGBA5551Color[menuLevels.Length][];

                for (int i = 0; i < menuLevels.Length; i++)
                {
                    var lvl = menuLevels[i];
                    var size = manager.GetMenuSize(lvl);
                    var isCompressed = manager.IsMenuCompressed(lvl);
                    var mapType = manager.HasMenuAlphaBlending(lvl) ? GBARRR_MapBlock.MapType.Menu : GBARRR_MapBlock.MapType.Foreground;

                    if (isCompressed)
                    {
                        s.DoAt(Menu_Pointers[lvl * 3 + 0], () => {
                            s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Menu_Tiles[i] = s.SerializeArray<byte>(Menu_Tiles[i], s.CurrentLength, name: $"{nameof(Menu_Tiles)}[{i}]"));
                        });
                        s.DoAt(Menu_Pointers[lvl * 3 + 1], () =>
                        {
                            s.DoEncoded(new RNC2Encoder(hasHeader: false), () => Menu_MapData[i] = s.SerializeObjectArray<MapTile>(Menu_MapData[i], size.Width * size.Height, onPreSerialize: x => x.GBARRRType = mapType, name: $"{nameof(Menu_MapData)}[{i}]"));
                        });
                    }
                    else
                    {
                        s.DoAt(Menu_Pointers[lvl * 3 + 0], () =>
                            Menu_Tiles[i] = s.SerializeArray<byte>(Menu_Tiles[i], 0x4B00*2, name: $"{nameof(Menu_Tiles)}[{i}]"));
                        s.DoAt(Menu_Pointers[lvl * 3 + 1], () =>
                            Menu_MapData[i] = s.SerializeObjectArray<MapTile>(Menu_MapData[i], size.Width * size.Height, onPreSerialize: x => x.GBARRRType = mapType, name: $"{nameof(Menu_MapData)}[{i}]"));
                    }

                    Menu_Palette[i] = s.DoAt(Menu_Pointers[lvl * 3 + 2], () => s.SerializeObjectArray<RGBA5551Color>(Menu_Palette[i], 16 * 16, name: $"{nameof(Menu_Palette)}[{i}]"));
                }
            }
        }

        public GBARRR_LevelInfo GetLevelInfo(GameSettings settings)
        {
            var mode = GBA_RRR_Manager.GetCurrentGameMode(settings);

            switch (mode)
            {
                case GBA_RRR_Manager.GameMode.Game:
                    return LevelInfo[settings.Level];

                case GBA_RRR_Manager.GameMode.Village:
                    return VillageLevelInfo[settings.Level];

                default:
                    throw new Exception($"{mode} maps do not use level info");
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