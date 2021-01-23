using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBACrash_ROM : GBA_ROMBase
    {
        public GBACrash_BaseManager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        public Pointer[] LocTablePointers { get; set; }
        public GBACrash_LocTable[] LocTables { get; set; }
        public GBACrash_LevelInfo[] LevelInfos { get; set; }

        public GBACrash_MapInfo CurrentMapInfo
        {
            get
            {
                GBACrash_MapInfo map;

                if (CurrentLevInfo.LevelIndex == -1)
                    map = new GBACrash_MapInfo
                    {
                        MapType = CurrentLevInfo.SpecialMapType,
                        Index3D = CurrentLevInfo.Index3D
                    };
                else if (CurrentLevInfo.MapType == GBACrash_BaseManager.LevInfo.Type.Normal)
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.Maps[CurrentLevInfo.MapIndex];
                else if (CurrentLevInfo.MapType == GBACrash_BaseManager.LevInfo.Type.Bonus)
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.BonusMap;
                else
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.ChallengeMap;

                return map;
            }
        }
        public GBACrash_Mode7_LevelInfo CurrentMode7LevelInfo => Mode7_LevelInfos[CurrentMapInfo.Index3D];
        public GBACrash_Isometric_LevelInfo CurrentIsometricLevelInfo => Isometric_LevelInfos[CurrentMapInfo.Index3D + 4];
        public GBACrash_Isometric_ObjectData CurrentIsometricObjData => Isometric_ObjectDatas[CurrentMapInfo.Index3D + 4];

        // 2D
        public GBACrash_AnimSet[] AnimSets { get; set; }
        public byte[] ObjTileSet { get; set; }
        public GBACrash_ObjPal[] ObjPalettes { get; set; }

        // Mode7
        public GBACrash_Mode7_LevelInfo[] Mode7_LevelInfos { get; set; }
        public RGBA5551Color[] Mode7_TilePalette { get; set; }
        public RGBA5551Color[] Mode7_Crash1_Type0_TilePalette_0F { get; set; }
        public byte[] Mode7_Crash2_Type0_BG1 { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileMapsPointers { get; set; }
        public MapTile[][] Mode7_Crash2_Type1_FlamesTileMaps { get; set; }
        public uint[] Mode7_Crash2_Type1_FlamesTileSetLengths { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileSetsPointers { get; set; }
        public byte[][] Mode7_Crash2_Type1_FlamesTileSets { get; set; }
        public RGBA5551Color[] Mode7_GetTilePal(GBACrash_Mode7_LevelInfo levInfo)
        {
            var tilePal = Context.Settings.EngineVersion == EngineVersion.GBACrash_Crash1 ? levInfo.TileSetFrames.Palette : Mode7_TilePalette;

            if (Context.Settings.EngineVersion == EngineVersion.GBACrash_Crash1 && levInfo.LevelType == 0)
                tilePal = tilePal.Take(256 - 16).Concat(Mode7_Crash1_Type0_TilePalette_0F).ToArray(); // Over last palette

            return tilePal;
        }

        // Isometric
        public GBACrash_Isometric_LevelInfo[] Isometric_LevelInfos { get; set; }
        public GBACrash_Isometric_ObjectData[] Isometric_ObjectDatas { get; set; }
        public GBACrash_Isometric_CharacterInfo[] Isometric_CharacterInfos { get; set; }
        public GBACrash_Isometric_CharacterIcon[] Isometric_CharacterIcons { get; set; }
        public GBACrash_Isometric_Animation[] Isometric_ObjAnimations { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_0 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_1 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_2 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_4 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_11 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_12 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_13 { get; set; }
        public RGBA5551Color[] Isometric_GetObjPalette =>
            Isometric_ObjPalette_0.
                Concat(Isometric_ObjPalette_1).
                Concat(Isometric_ObjPalette_2).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16* 1)).
                Concat(Isometric_ObjPalette_4).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16 * 6)).
                Concat(Isometric_ObjPalette_11).
                Concat(Isometric_ObjPalette_12).
                Concat(Isometric_ObjPalette_13).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16 * 2)).
                ToArray();
        public GBACrash_Isometric_Animation[] Isometric_AdditionalAnimations { get; set; }
        public IEnumerable<GBACrash_Isometric_Animation> Isometric_GetAnimations => Isometric_ObjAnimations.Concat(Isometric_AdditionalAnimations);

        // WorldMap
        public GBACrash_WorldMap_Data WorldMap { get; set; }
        public GBACrash_WorldMap_Crash1_LevelIcon[] WorldMap_Crash1_LevelIcons { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBACrash_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Get the current lev info
            var manager = (GBACrash_BaseManager)s.GameSettings.GetGameManager;

            if (pointerTable.ContainsKey(GBACrash_Pointer.Localization))
            {
                var multipleLanguages = s.GameSettings.GameModeSelection == GameModeSelection.Crash1GBAEU || s.GameSettings.GameModeSelection == GameModeSelection.Crash2GBAEU;

                if (multipleLanguages)
                    LocTablePointers = s.DoAt(pointerTable[GBACrash_Pointer.Localization], () => s.SerializePointerArray(LocTablePointers, 6, name: nameof(LocTablePointers)));
                else
                    LocTablePointers = new Pointer[]
                    {
                        pointerTable[GBACrash_Pointer.Localization]
                    };

                if (LocTables == null)
                    LocTables = new GBACrash_LocTable[LocTablePointers.Length];

                for (int i = 0; i < LocTables.Length; i++)
                    LocTables[i] = s.DoAt(LocTablePointers[i], () => s.SerializeObject<GBACrash_LocTable>(LocTables[i], name: $"{nameof(LocTables)}[{i}]"));
            }

            s.Context.StoreObject(GBACrash_BaseManager.LocTableID, LocTables?.FirstOrDefault());

            s.DoAt(pointerTable[GBACrash_Pointer.LevelInfo], () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBACrash_LevelInfo[manager.LevInfos.Max(x => x.LevelIndex) + 1];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBACrash_LevelInfo>(LevelInfos[i], x => x.LevInfo = i == CurrentLevInfo.LevelIndex ? CurrentLevInfo : null, name: $"{nameof(LevelInfos)}[{i}]");
            });

            if (CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Normal ||
                CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Normal_Vehicle_0 ||
                CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Normal_Vehicle_1 ||
                CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.WorldMap)
            {
                AnimSets = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_AnimSets], () => s.SerializeObjectArray<GBACrash_AnimSet>(AnimSets, manager.AnimSetsCount, name: nameof(AnimSets)));

                var tileSetLength = (long)AnimSets.SelectMany(x => x.AnimationFrames).Select(x =>
                    x.TileOffset + (x.TileShapes.Select(t => (manager.TileShapes[t.ShapeIndex].x * manager.TileShapes[t.ShapeIndex].y) / 2).Sum())).Max();
                ObjTileSet = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_ObjTileSet], () => s.SerializeArray<byte>(ObjTileSet, tileSetLength, name: nameof(ObjTileSet)));
                ObjPalettes = s.DoAt(pointerTable[GBACrash_Pointer.Map2D_ObjPalettes], () => s.SerializeObjectArray<GBACrash_ObjPal>(ObjPalettes, AnimSets.SelectMany(x => x.Animations).Max(x => x.PaletteIndex) + 1, name: nameof(ObjPalettes)));
            }

            if (CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Mode7)
            {
                s.DoAt(pointerTable[GBACrash_Pointer.Mode7_LevelInfo], () =>
                {
                    if (Mode7_LevelInfos == null)
                        Mode7_LevelInfos = new GBACrash_Mode7_LevelInfo[7];

                    var index3D = CurrentMapInfo.Index3D;

                    for (int i = 0; i < Mode7_LevelInfos.Length; i++)
                        Mode7_LevelInfos[i] = s.SerializeObject<GBACrash_Mode7_LevelInfo>(Mode7_LevelInfos[i], x => x.SerializeData = i == index3D, name: $"{nameof(Mode7_LevelInfos)}[{i}]");
                });

                if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash2)
                {
                    GBACrash_Pointer palPointer = CurrentMode7LevelInfo.LevelType == 0 ? GBACrash_Pointer.Mode7_TilePalette_Type0 : GBACrash_Pointer.Mode7_TilePalette_Type1_Flames;

                    Mode7_TilePalette = s.DoAt(pointerTable[palPointer], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_TilePalette, CurrentMode7LevelInfo.LevelType == 0 ? 256 : 16, name: nameof(Mode7_TilePalette)));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1 && CurrentMode7LevelInfo.LevelType == 0)
                {
                    Mode7_Crash1_Type0_TilePalette_0F = s.DoAt(pointerTable[GBACrash_Pointer.Mode7_Crash1_Type0_TilePalette_0F], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_Crash1_Type0_TilePalette_0F, 16, name: nameof(Mode7_Crash1_Type0_TilePalette_0F)));
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

            if (CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.Isometric)
            {
                var index3D = CurrentMapInfo.Index3D;

                s.DoAt(pointerTable[GBACrash_Pointer.Isometric_LevelInfo], () =>
                {
                    if (Isometric_LevelInfos == null)
                        Isometric_LevelInfos = new GBACrash_Isometric_LevelInfo[7];


                    for (int i = 0; i < Isometric_LevelInfos.Length; i++)
                        Isometric_LevelInfos[i] = s.SerializeObject<GBACrash_Isometric_LevelInfo>(Isometric_LevelInfos[i], x => x.SerializeData = i == index3D + 4, name: $"{nameof(Isometric_LevelInfos)}[{i}]");
                });

                s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjectDatas], () =>
                {
                    if (Isometric_ObjectDatas == null)
                        Isometric_ObjectDatas = new GBACrash_Isometric_ObjectData[7];

                    for (int i = 0; i < Isometric_ObjectDatas.Length; i++)
                        Isometric_ObjectDatas[i] = s.SerializeObject<GBACrash_Isometric_ObjectData>(Isometric_ObjectDatas[i], x =>
                        {
                            x.SerializeData = i == index3D + 4;
                            x.IsMultiplayer = i < 4;
                        }, name: $"{nameof(Isometric_ObjectDatas)}[{i}]");
                });

                Isometric_CharacterInfos = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_Characters], () => s.SerializeObjectArray<GBACrash_Isometric_CharacterInfo>(Isometric_CharacterInfos, 12, name: nameof(Isometric_CharacterInfos)));
                Isometric_CharacterIcons = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_CharacterIcons], () => s.SerializeObjectArray<GBACrash_Isometric_CharacterIcon>(Isometric_CharacterIcons, 11, name: nameof(Isometric_CharacterIcons)));
                Isometric_ObjAnimations = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjAnimations], () => s.SerializeObjectArray<GBACrash_Isometric_Animation>(Isometric_ObjAnimations, 22, name: nameof(Isometric_ObjAnimations)));

                Isometric_ObjPalette_0 = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjPalette_0], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_0, 16, name: nameof(Isometric_ObjPalette_0)));
                Isometric_ObjPalette_1 = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjPalette_1], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_1, 16, name: nameof(Isometric_ObjPalette_1)));
                Isometric_ObjPalette_2 = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjPalette_2], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_2, 16, name: nameof(Isometric_ObjPalette_2)));
                Isometric_ObjPalette_4 = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjPalette_4], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_4, 16, name: nameof(Isometric_ObjPalette_4)));
                Isometric_ObjPalette_11 = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjPalette_11], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_11, 16, name: nameof(Isometric_ObjPalette_11)));
                Isometric_ObjPalette_12 = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjPalette_12], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_12, 16, name: nameof(Isometric_ObjPalette_12)));
                Isometric_ObjPalette_13 = s.DoAt(pointerTable[GBACrash_Pointer.Isometric_ObjPalette_13], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_13, 16, name: nameof(Isometric_ObjPalette_13)));

                // These animations are all hard-coded from functions:
                Isometric_AdditionalAnimations = new GBACrash_Isometric_Animation[]
                {
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim0_Frames], 0x03, 4, 4, 2), // Green barrel
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim1_Frames], 0x03, 4, 4, 2), // Laser beam
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim2_Frames], 0x06, 4, 4, 1), // Crate breaks
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim3_Frames], 0x07, 4, 4, 1), // Checkpoint breaks
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim4_Frames], 0x18, 8, 4, 0), // Checkpoint text
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim5_Frames], 0x08, 4, 4, 2), // Nitro explosion
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim6_Frames], 0x08, 4, 4, 2), // Nitro switch
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim7_Frames], 0x0E, 4, 4, 0), // Wumpa HUD
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim8_Frames], 0x0A, 8, 8, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim8_Palette]), // Crystal collected
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim9_Frames], 0x03, 4, 4, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim9_Palette]), // Multiplayer base
                    GBACrash_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim10_Frames], 0x0A, 2, 2, pointerTable[GBACrash_Pointer.Isometric_AdditionalAnim10_Palette]), // Multiplayer item
                };
            }

            if (CurrentMapInfo.MapType == GBACrash_MapInfo.GBACrash_MapType.WorldMap)
            {
                if (pointerTable.ContainsKey(GBACrash_Pointer.WorldMap))
                    WorldMap = s.DoAt(pointerTable[GBACrash_Pointer.WorldMap], () => s.SerializeObject(WorldMap, name: nameof(WorldMap)));

                if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1)
                    WorldMap_Crash1_LevelIcons = s.DoAt(pointerTable[GBACrash_Pointer.WorldMap_Crash1_LevelIcons], () => s.SerializeObjectArray<GBACrash_WorldMap_Crash1_LevelIcon>(WorldMap_Crash1_LevelIcons, 10, name: nameof(WorldMap_Crash1_LevelIcons)));
            }
        }
    }
}