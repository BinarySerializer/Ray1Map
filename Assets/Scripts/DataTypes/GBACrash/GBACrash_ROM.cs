using System.Linq;

namespace R1Engine
{
    public class GBACrash_ROM : GBA_ROMBase
    {
        public bool SerializeAll { get; set; } // Set before serializing

        public GBACrash_LocTable LocTable { get; set; }
        public GBACrash_LevelInfo[] LevelInfos { get; set; }

        public GBACrash_MapInfo CurrentMapInfo
        {
            get
            {
                GBACrash_MapInfo map;

                var manager = (GBACrash_BaseManager)Context.Settings.GetGameManager;
                var levInfo = manager.LevInfos[Context.Settings.Level];

                if (levInfo.LevelIndex == -1)
                    map = new GBACrash_MapInfo
                    {
                        MapType = levInfo.SpecialMapType,
                        Index3D = (ushort)levInfo.Index3D
                    };
                else if (levInfo.MapType == GBACrash_BaseManager.LevInfo.Type.Normal)
                    map = LevelInfos[levInfo.LevelIndex].LevelData.Maps[levInfo.MapIndex];
                else if (levInfo.MapType == GBACrash_BaseManager.LevInfo.Type.Bonus)
                    map = LevelInfos[levInfo.LevelIndex].LevelData.BonusMap;
                else
                    map = LevelInfos[levInfo.LevelIndex].LevelData.ChallengeMap;

                return map;
            }
        }
        public GBACrash_Mode7_LevelInfo CurrentMode7LevelInfo => Mode7_LevelInfos[CurrentMapInfo.Index3D];

        // 2D
        public GBACrash_AnimSet[] AnimSets { get; set; }
        public byte[] ObjTileSet { get; set; }
        public GBACrash_ObjPal[] ObjPalettes { get; set; }

        // Mode7
        public GBACrash_Mode7_LevelInfo[] Mode7_LevelInfos { get; set; }
        public RGBA5551Color[] Mode7_TilePalette { get; set; }
        public byte[] Mode7_Crash2_Type0_BG1 { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileMapsPointers { get; set; }
        public MapTile[][] Mode7_Crash2_Type1_FlamesTileMaps { get; set; }
        public uint[] Mode7_Crash2_Type1_FlamesTileSetLengths { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileSetsPointers { get; set; }
        public byte[][] Mode7_Crash2_Type1_FlamesTileSets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBACrash_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Get the current lev info
            var manager = (GBACrash_BaseManager)s.GameSettings.GetGameManager;
            var levInfo = manager.LevInfos[s.GameSettings.Level];

            LocTable = s.DoAt(pointerTable[GBACrash_Pointer.Localization], () => s.SerializeObject<GBACrash_LocTable>(LocTable, name: nameof(LocTable)));
            s.Context.StoreObject(GBACrash_BaseManager.LocTableID, LocTable);

            s.DoAt(pointerTable[GBACrash_Pointer.LevelInfo], () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBACrash_LevelInfo[manager.LevInfos.Max(x => x.LevelIndex) + 1];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBACrash_LevelInfo>(LevelInfos[i], x => x.LevInfo = i == levInfo.LevelIndex ? levInfo : null, name: $"{nameof(LevelInfos)}[{i}]");
            });

            if (CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Normal ||
                CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Normal_Vehicle_0 ||
                CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Normal_Vehicle_1 ||
                SerializeAll)
            {
                AnimSets = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_AnimSets], () => s.SerializeObjectArray<GBACrash_AnimSet>(AnimSets, manager.AnimSetsCount, name: nameof(AnimSets)));

                var tileSetLength = (long)AnimSets.SelectMany(x => x.AnimationFrames).Select(x =>
                    x.TileOffset + (x.TileShapes.Select(t => (manager.TileShapes[t.ShapeIndex].x * manager.TileShapes[t.ShapeIndex].y) / 2).Sum())).Max();
                ObjTileSet = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_ObjTileSet], () => s.SerializeArray<byte>(ObjTileSet, tileSetLength, name: nameof(ObjTileSet)));
                ObjPalettes = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_ObjPalettes], () => s.SerializeObjectArray<GBACrash_ObjPal>(ObjPalettes, AnimSets.SelectMany(x => x.Animations).Max(x => x.PaletteIndex) + 1, name: nameof(ObjPalettes)));
            }

            if (CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Mode7 || SerializeAll)
            {
                s.DoAt(pointerTable[GBACrash_Pointer.Mode7_LevelInfo], () =>
                {
                    if (Mode7_LevelInfos == null)
                        Mode7_LevelInfos = new GBACrash_Mode7_LevelInfo[7];

                    var index3D = CurrentMapInfo.Index3D;

                    for (int i = 0; i < Mode7_LevelInfos.Length; i++)
                        Mode7_LevelInfos[i] = s.SerializeObject<GBACrash_Mode7_LevelInfo>(Mode7_LevelInfos[i], x => x.SerializeData = i == index3D || SerializeAll, name: $"{nameof(Mode7_LevelInfos)}[{i}]");
                });

                if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash2)
                {
                    GBACrash_Pointer palPointer = CurrentMode7LevelInfo.LevelType == 0 ? GBACrash_Pointer.Mode7_TilePalette_Type0 : GBACrash_Pointer.Mode7_TilePalette_Type1_Flames;

                    Mode7_TilePalette = s.DoAt(pointerTable[palPointer], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_TilePalette, CurrentMode7LevelInfo.LevelType == 0 ? 256 : 16, name: nameof(Mode7_TilePalette)));
                }

                if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash2 && CurrentMode7LevelInfo.LevelType == 0)
                {
                    Mode7_Crash2_Type0_BG1 = s.DoAt(pointerTable[GBACrash_Pointer.Mode7_Crash2_Type0_BG1], () => s.SerializeArray<byte>(Mode7_Crash2_Type0_BG1, 38 * 9 * 32, name: nameof(Mode7_Crash2_Type0_BG1)));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash2 && CurrentMode7LevelInfo.LevelType == 1)
                {
                    Mode7_Crash2_Type1_FlamesTileMapsPointers = s.DoAt(pointerTable[GBACrash_Pointer.Mode7_Crash2_Type1_FlamesTileMaps], () => s.SerializePointerArray(Mode7_Crash2_Type1_FlamesTileMapsPointers, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileMapsPointers)));

                    if (Mode7_Crash2_Type1_FlamesTileMaps == null)
                        Mode7_Crash2_Type1_FlamesTileMaps = new MapTile[20][];

                    for (int i = 0; i < Mode7_Crash2_Type1_FlamesTileMaps.Length; i++)
                        Mode7_Crash2_Type1_FlamesTileMaps[i] = s.DoAt(Mode7_Crash2_Type1_FlamesTileMapsPointers[i], () => s.SerializeObjectArray<MapTile>(Mode7_Crash2_Type1_FlamesTileMaps[i], 0x1E * 0x14, name: $"{nameof(Mode7_Crash2_Type1_FlamesTileMaps)}[{i}]"));

                    Mode7_Crash2_Type1_FlamesTileSetLengths = s.DoAt(pointerTable[GBACrash_Pointer.Mode7_Crash2_Type1_FlamesTileSetLengths], () => s.SerializeArray<uint>(Mode7_Crash2_Type1_FlamesTileSetLengths, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileSetLengths)));

                    Mode7_Crash2_Type1_FlamesTileSetsPointers = s.DoAt(pointerTable[GBACrash_Pointer.Mode7_Crash2_Type1_FlamesTileSets], () => s.SerializePointerArray(Mode7_Crash2_Type1_FlamesTileSetsPointers, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileSetsPointers)));

                    if (Mode7_Crash2_Type1_FlamesTileSets == null)
                        Mode7_Crash2_Type1_FlamesTileSets = new byte[20][];

                    for (int i = 0; i < Mode7_Crash2_Type1_FlamesTileSets.Length; i++)
                        Mode7_Crash2_Type1_FlamesTileSets[i] = s.DoAt(Mode7_Crash2_Type1_FlamesTileSetsPointers[i], () => s.SerializeArray<byte>(Mode7_Crash2_Type1_FlamesTileSets[i], Mode7_Crash2_Type1_FlamesTileSetLengths[i], name: $"{nameof(Mode7_Crash2_Type1_FlamesTileSets)}[{i}]"));
                }
            }

            if (CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Isometric || SerializeAll)
            {

            }
        }
    }
}