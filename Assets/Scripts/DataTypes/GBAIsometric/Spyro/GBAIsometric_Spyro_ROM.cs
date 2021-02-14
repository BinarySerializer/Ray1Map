using System;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro_ROM : GBA_ROMBase
    {
        public GBAIsometric_Spyro_DataTable DataTable { get; set; }
        public GBAIsometric_Spyro_Localization Localization { get; set; }

        public GBAIsometric_Spyro_LevelDataArray LevelData { get; set; }
        public GBAIsometric_Spyro_LevelDataArray LevelData_Spyro2_Agent9 { get; set; }
        public GBAIsometric_Spyro_LevelDataArray LevelData_Spyro3_Agent9 { get; set; }
        public GBAIsometric_Spyro_LevelDataArray LevelData_Spyro3_SgtByrd { get; set; }
        public GBAIsometric_Spyro_LevelObjects[] LevelObjects { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex[] LevelObjectIndices_Spyro2_Agent9 { get; set; }
        public GBAIsometric_Spyro2_LevelObjects2D[] LevelObjects_Spyro2_Agent9 { get; set; }
        public GBAIsometric_Spyro_LevelObjects[] LevelObjects_Spyro3_Agent9 { get; set; }
        public GBAIsometric_Spyro_SgtByrdInfo[] LevelObjects_Spyro3_SgtByrd { get; set; }
        public GBAIsometric_Spyro_ByrdRescueInfo[] LevelObjects_Spyro3_ByrdRescue { get; set; }
        public GBAIsometric_Spyro_LevelMap[] LevelMaps { get; set; }

        public GBAIsometric_ObjectType[] ObjectTypes { get; set; }
        public GBAIsometric_Spyro_AnimSet[] AnimSets { get; set; }

        public GBAIsometric_Spyro_PortraitSprite[] PortraitSprites { get; set; }
        public GBAIsometric_Spyro_Dialog[] DialogEntries { get; set; }

        public GBAIsometric_Spyro_LevelNameInfo[] LevelNameInfos { get; set; }
        public GBAIsometric_LocIndex[] LevelNames { get; set; }
        public byte[] LevelIndices { get; set; } // Level index for every map
        public ushort[] GemCounts { get; set; } // The gem count for every level
        public GBAIsometric_Spyro_MenuPage[] MenuPages { get; set; }

        public GBAIsometric_Spyro_UnkStruct[] UnkStructs { get; set; }
        public GBAIsometric_Spyro_CutsceneMap[] CutsceneMaps { get; set; }

        public RGBA5551Color[] Spyro2_CommonPalette { get; set; }
        public RGBA5551Color[][][] Spyro2_AnimSetPalettes { get; set; }

        public GBAIsometric_Spyro3_State_NPC[] States_Spyro3_NPC { get; set; }
        public GBAIsometric_Spyro3_State_DoorTypes[] States_Spyro3_DoorTypes { get; set; }
        public GBAIsometric_Spyro3_State_DoorGraphics[] States_Spyro3_DoorGraphics { get; set; }
        public GBAIsometric_Spyro2_State_LevelObjective[] States_Spyro2_LevelObjectives { get; set; }
        public GBAIsometric_Spyro2_State_Portal[] States_Spyro2_Portals { get; set; }
        public GBAIsometric_Spyro2_State_ChallengePortal[] States_Spyro2_ChallengePortals { get; set; }
        public GBAIsometric_Spyro3_QuestItem[] QuestItems { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var pointerTable = PointerTables.GBAIsometric_Spyro_PointerTable(s.GameSettings.GameModeSelection, Offset.file);
            var manager = (GBAIsometric_Spyro_Manager)s.GameSettings.GetGameManager;

            // Serialize primary data table and store it so we can get the data blocks
            DataTable = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.DataTable], () => s.SerializeObject<GBAIsometric_Spyro_DataTable>(DataTable, name: nameof(DataTable)));
            s.Context.StoreObject(nameof(DataTable), DataTable);

            // Serialize the localization data
            if (s.GameSettings.EngineVersion != EngineVersion.GBAIsometric_Tron2)
                Localization = s.SerializeObject<GBAIsometric_Spyro_Localization>(Localization, name: nameof(Localization));

            var id = GetLevelDataID(s.GameSettings);

            // Serialize level data
            LevelData = s.DoAt<GBAIsometric_Spyro_LevelDataArray>(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LevelData), () => s.SerializeObject(LevelData, x =>
            {
                x.Length = manager.LevelDataCount;
                x.UsesPointerArray = true;
                x.Is2D = false;
                x.SerializeDataForID = s.GameSettings.World != 0 ? -1 : id;
            }, name: nameof(LevelData)));
            LevelObjects = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LevelObjects), () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelObjects>(LevelObjects, LevelData.Length, name: nameof(LevelObjects)));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
            {
                // Agent 9
                LevelData_Spyro2_Agent9 = s.DoAt<GBAIsometric_Spyro_LevelDataArray>(pointerTable[GBAIsometric_Spyro_Pointer.LevelData_Spyro2_Agent9], () => s.SerializeObject(LevelData_Spyro2_Agent9, x =>
                {
                    x.Length = 4;
                    x.UsesPointerArray = false;
                    x.Is2D = true;
                    x.SerializeDataForID = s.GameSettings.World != 1 ? -1 : id;
                    x.AssignIDAsIndex = true;
                }, name: nameof(LevelData_Spyro2_Agent9)));

                ushort[] objIndices;

                if (s.GameSettings.GameModeSelection == GameModeSelection.SpyroSeasonFlameEU)
                    objIndices = new ushort[] { 0x4E3, 0x4F2, 0x4D4, 0x502 };
                else
                    objIndices = new ushort[] { 0x4DE, 0x4ED, 0x4CF, 0x4FD };

                // The game hard-codes these indices
                LevelObjectIndices_Spyro2_Agent9 = objIndices.Select(x => GBAIsometric_Spyro_DataBlockIndex.FromIndex(s, x)).ToArray();

                if (LevelObjects_Spyro2_Agent9 == null)
                    LevelObjects_Spyro2_Agent9 = new GBAIsometric_Spyro2_LevelObjects2D[LevelObjectIndices_Spyro2_Agent9.Length];

                for (int i = 0; i < LevelObjects_Spyro2_Agent9.Length; i++)
                    LevelObjects_Spyro2_Agent9[i] = LevelObjectIndices_Spyro2_Agent9[i].DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro2_LevelObjects2D>(LevelObjects_Spyro2_Agent9[i], name: $"{nameof(LevelObjects_Spyro2_Agent9)}[{i}]"));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
            {
                // Agent 9

                LevelData_Spyro3_Agent9 = s.DoAt<GBAIsometric_Spyro_LevelDataArray>(pointerTable[GBAIsometric_Spyro_Pointer.LevelData_Spyro3_Agent9], () => s.SerializeObject(LevelData_Spyro3_Agent9, x =>
                {
                    x.Length = 4;
                    x.UsesPointerArray = true;
                    x.Is2D = true;
                    x.SerializeDataForID = s.GameSettings.World != 1 ? -1 : id;
                }, name: nameof(LevelData_Spyro3_Agent9)));
                LevelObjects_Spyro3_Agent9 = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_Agent9], () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelObjects>(LevelObjects_Spyro3_Agent9, LevelData_Spyro3_Agent9.Length, name: nameof(LevelObjects_Spyro3_Agent9)));

                // Sgt. Byrd
                LevelObjects_Spyro3_SgtByrd = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_SgtByrd], () => s.SerializeObjectArray<GBAIsometric_Spyro_SgtByrdInfo>(LevelObjects_Spyro3_SgtByrd, 4, name: nameof(LevelObjects_Spyro3_SgtByrd)));
                LevelObjects_Spyro3_ByrdRescue = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.LevelObjects_Spyro3_ByrdRescue], () => s.SerializeObjectArray<GBAIsometric_Spyro_ByrdRescueInfo>(LevelObjects_Spyro3_ByrdRescue, 22, name: nameof(LevelObjects_Spyro3_ByrdRescue)));

                id = GetLevelDataID(s.GameSettings);

                LevelData_Spyro3_SgtByrd = s.DoAt<GBAIsometric_Spyro_LevelDataArray>(pointerTable[GBAIsometric_Spyro_Pointer.LevelData_Spyro3_SgtByrd], () => s.SerializeObject(LevelData_Spyro3_SgtByrd, x =>
                {
                    x.Length = 13;
                    x.UsesPointerArray = true;
                    x.Is2D = true;
                    x.SerializeDataForID = s.GameSettings.World != 2 && s.GameSettings.World != 3 ? -1 : id;
                }, name: nameof(LevelData_Spyro3_SgtByrd)));
            }

            LevelMaps = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LevelMaps), () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelMap>(LevelMaps, manager.LevelMapsCount, name: nameof(LevelMaps)));

            // Serialize object data
            ObjectTypes = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.ObjectTypes), () => s.SerializeObjectArray<GBAIsometric_ObjectType>(ObjectTypes, manager.ObjectTypesCount, name: nameof(ObjectTypes)));
            AnimSets = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.AnimSets), () => s.SerializeObjectArray<GBAIsometric_Spyro_AnimSet>(AnimSets, manager.AnimSetsCount, name: nameof(AnimSets)));

            // Serialize portraits
            PortraitSprites = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.PortraitSprites), () => s.SerializeObjectArray<GBAIsometric_Spyro_PortraitSprite>(PortraitSprites, manager.PortraitsCount, name: nameof(PortraitSprites)));

            // Serialize dialog entries
            DialogEntries = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.DialogEntries), () => s.SerializeObjectArray<GBAIsometric_Spyro_Dialog>(DialogEntries, manager.DialogCount, name: nameof(DialogEntries)));

            // Serialize cutscene maps
            CutsceneMaps = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.CutsceneMaps), () => s.SerializeObjectArray<GBAIsometric_Spyro_CutsceneMap>(CutsceneMaps, 11, name: nameof(CutsceneMaps)));

            // Serialize level properties
            GemCounts = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.GemCounts), () => s.SerializeArray<ushort>(GemCounts, manager.PrimaryLevelCount, name: nameof(GemCounts)));
            LevelIndices = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LevelIndices), () => s.SerializeArray<byte>(LevelIndices, manager.TotalLevelsCount, name: nameof(LevelIndices)));
            LevelNameInfos = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LevelNameInfos), () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelNameInfo>(LevelNameInfos, manager.TotalLevelsCount, name: nameof(LevelNameInfos)));
            LevelNames = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LevelNames), () => s.SerializeObjectArray<GBAIsometric_LocIndex>(LevelNames, manager.TotalLevelsCount, name: nameof(LevelNames)));
            MenuPages = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.MenuPages), () => s.SerializeObjectArray<GBAIsometric_Spyro_MenuPage>(MenuPages, manager.MenuPageCount, name: nameof(MenuPages)));

            // Serialize palettes for Spyro 2
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
            {
                Spyro2_CommonPalette = GBAIsometric_Spyro_DataBlockIndex.FromIndex(s, (ushort)(s.GameSettings.GameModeSelection == GameModeSelection.SpyroSeasonFlameEU ? 332 : 321)).DoAtBlock(size => s.SerializeObjectArray<RGBA5551Color>(Spyro2_CommonPalette, 256, name: nameof(Spyro2_CommonPalette)));

                var palInfo = s.GameSettings.GameModeSelection == GameModeSelection.SpyroSeasonFlameUS ? Spyro2_PalInfoUS : Spyro2_PalInfoEU;

                if (Spyro2_AnimSetPalettes == null)
                    Spyro2_AnimSetPalettes = new RGBA5551Color[palInfo.Length][][];

                for (int i = 0; i < Spyro2_AnimSetPalettes.Length; i++)
                {
                    var p = palInfo[i];

                    if (p.BlockIndices != null)
                    {
                        if (Spyro2_AnimSetPalettes[i] == null)
                            Spyro2_AnimSetPalettes[i] = new RGBA5551Color[p.BlockIndices.Length][];

                        for (int j = 0; j < p.BlockIndices.Length; j++)
                        {
                            Spyro2_AnimSetPalettes[i][j] = GBAIsometric_Spyro_DataBlockIndex.FromIndex(s, (ushort)p.BlockIndices[j]).DoAtBlock(size => s.SerializeObjectArray<RGBA5551Color>(Spyro2_AnimSetPalettes[i][j], size / 2, name: $"{nameof(Spyro2_AnimSetPalettes)}[{i}][{j}]"));
                        }
                    }
                }
            }

            // Serialize object states
            States_Spyro3_NPC = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.States_Spyro3_NPC), () => s.SerializeObjectArray<GBAIsometric_Spyro3_State_NPC>(States_Spyro3_NPC, 49, name: nameof(States_Spyro3_NPC)));
            States_Spyro3_DoorTypes = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.States_Spyro3_DoorTypes), () => s.SerializeObjectArray<GBAIsometric_Spyro3_State_DoorTypes>(States_Spyro3_DoorTypes, 49, name: nameof(States_Spyro3_DoorTypes)));
            States_Spyro3_DoorGraphics = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.States_Spyro3_DoorGraphics), () => s.SerializeObjectArray<GBAIsometric_Spyro3_State_DoorGraphics>(States_Spyro3_DoorGraphics, 22, name: nameof(States_Spyro3_DoorGraphics)));
            States_Spyro2_LevelObjectives = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.States_Spyro2_LevelObjectives), () => s.SerializeObjectArray<GBAIsometric_Spyro2_State_LevelObjective>(States_Spyro2_LevelObjectives, 14, name: nameof(States_Spyro2_LevelObjectives)));
            States_Spyro2_Portals = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.States_Spyro2_Portals), () => s.SerializeObjectArray<GBAIsometric_Spyro2_State_Portal>(States_Spyro2_Portals, 25, name: nameof(States_Spyro2_Portals)));
            States_Spyro2_ChallengePortals = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.States_Spyro2_ChallengePortals), () => s.SerializeObjectArray<GBAIsometric_Spyro2_State_ChallengePortal>(States_Spyro2_ChallengePortals, 10, name: nameof(States_Spyro2_ChallengePortals)));

            QuestItems = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.QuestItems), () => s.SerializeObjectArray<GBAIsometric_Spyro3_QuestItem>(QuestItems, 104, name: nameof(QuestItems)));

            // Serialize unknown struct
            if (s.GameSettings.GameModeSelection == GameModeSelection.SpyroAdventureUS)
            {
                UnkStructs = s.DoAt(Offset + 0x1c009c, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct>(UnkStructs, 104, name: nameof(UnkStructs)));
                // 0x1bfa10, same as UnkStruct
            }
        }

        public GBAIsometric_Spyro_LevelData GetLevelData(GameSettings settings)
        {
            GBAIsometric_Spyro_LevelDataArray array = null;

            if (settings.World == 0)
            {
                array = LevelData;
            }
            else
            {
                if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                {
                    if (settings.World == 1)
                        array = LevelData_Spyro2_Agent9;
                }
                else if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                {
                    if (settings.World == 1)
                        array = LevelData_Spyro3_Agent9;
                    else if (settings.World == 2 || settings.World == 3)
                        array = LevelData_Spyro3_SgtByrd;
                }
            }

            if (array == null)
                throw new Exception("Invalid game settings");

            var id = GetLevelDataID(settings);

            return array.LevelData.First(x => x.ID == id);
        }
        public GBAIsometric_Spyro_ObjectTable GetObjectTable(GameSettings settings)
        {
            GBAIsometric_Spyro_LevelObjects[] array = null;

            if (settings.World == 0)
            {
                array = LevelObjects;
            }
            else
            {
                if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                {
                    if (settings.World == 1)
                        return null;
                }
                else if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                {
                    if (settings.World == 1)
                        array = LevelObjects_Spyro3_Agent9;
                    else if (settings.World == 2)
                        return LevelObjects_Spyro3_SgtByrd.First(x => x.LevelID == settings.Level).ObjectTable;
                    else if (settings.World == 3)
                        return LevelObjects_Spyro3_ByrdRescue.First(x => x.ID == settings.Level).ObjectTable;
                }
            }

            var id = GetLevelDataID(settings);

            return array?.First(x => x.LevelID == id).ObjectTable;
        }
        public int GetLevelDataID(GameSettings settings)
        {
            if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3 && settings.World == 2)
                return LevelObjects_Spyro3_SgtByrd?.First(x => x.LevelID == settings.Level).LevelDataID ?? -1;
            else if (settings.EngineVersion == EngineVersion.GBAIsometric_Spyro3 && settings.World == 3)
                return LevelObjects_Spyro3_ByrdRescue?.First(x => x.ID == settings.Level).LevelDataID ?? -1;
            else
                return settings.Level;
        }

        public class PalInfo
        {
            public PalInfo(bool usesCommonPalette) : this(-1, usesCommonPalette) { }
            public PalInfo(bool usesCommonPalette, params long[] blockIndices)
            {
                UsesCommonPalette = usesCommonPalette;
                BlockIndices = blockIndices;
            }
            public PalInfo(long blockIndex = -1, bool usesCommonPalette = true)
            {
                if (blockIndex != -1)
                    BlockIndices = new long[]
                    {
                        blockIndex
                    };
                UsesCommonPalette = usesCommonPalette;
            }

            public long[] BlockIndices { get; }
            public bool UsesCommonPalette { get; }
        }

        public PalInfo[] Spyro2_PalInfoUS => new PalInfo[]
        {
            // HUD/Menu
            new PalInfo(true, new long[]
            {
                1211,
                315,
                315,
                315,
                315,
                1475,
            }), // AnimSet 0
            new PalInfo(78, false), // AnimSet 1
            new PalInfo(175, false), // AnimSet 2
            new PalInfo(226, false), // AnimSet 3
            new PalInfo(225, false), // AnimSet 4
            new PalInfo(225, false), // AnimSet 5
            new PalInfo(227, false), // AnimSet 6

            // Game
            new PalInfo(315), // AnimSet 7
            new PalInfo(310), // AnimSet 8
            new PalInfo(333), // AnimSet 9
            new PalInfo(317), // AnimSet 10
            new PalInfo(339), // AnimSet 11
            new PalInfo(335), // AnimSet 12
            new PalInfo(315), // AnimSet 13
            new PalInfo(327), // AnimSet 14
            new PalInfo(320), // AnimSet 15
            new PalInfo(339), // AnimSet 16
            new PalInfo(333), // AnimSet 17
            new PalInfo(328), // AnimSet 18
            new PalInfo(328), // AnimSet 19
            new PalInfo(328), // AnimSet 20
            new PalInfo(), // AnimSet 21 // NOTE: Unused (not in eu version)
            new PalInfo(310), // AnimSet 22
            new PalInfo(327), // AnimSet 23
            new PalInfo(344), // AnimSet 24
            new PalInfo(313), // AnimSet 25
            new PalInfo(337), // AnimSet 26
            new PalInfo(328), // AnimSet 27
            new PalInfo(328), // AnimSet 28
            new PalInfo(320), // AnimSet 29
            new PalInfo(310), // AnimSet 30
            new PalInfo(320), // AnimSet 31
            new PalInfo(318), // AnimSet 32
            new PalInfo(339), // AnimSet 33
            new PalInfo(328), // AnimSet 34
            new PalInfo(328), // AnimSet 35
            new PalInfo(true, new long[]
            {
                310, 310, // Candy Lane
                323, 323, // Moon Fondue
                312, 312, 312, // Country Farms
                315, 315, // Temple of Dune
                317, 317, // Dreamy Castle
                325, 325, // Celestial Plains
                335, 335, // Starry Plains
                337, 337, // Sunny Plains
                320, 320, // Tiki Tropics
                323, 323, // NOTE: Unused
                327, 327, // Haunted Hills
                330, 330, // Shamrock Isle
                339, 339, // Watertopia
                342, 342, // Winter Mesa
                344, 344, // Gypsy Road
            }), // AnimSet 36
            new PalInfo(344), // AnimSet 37
            new PalInfo(310), // AnimSet 38
            new PalInfo(327), // AnimSet 39
            new PalInfo(327), // AnimSet 40
            new PalInfo(341), // AnimSet 41
            new PalInfo(341), // AnimSet 42
            new PalInfo(310), // AnimSet 43
            new PalInfo(318), // AnimSet 44
            new PalInfo(333), // AnimSet 45
            new PalInfo(332), // AnimSet 46
            new PalInfo(312), // AnimSet 47
            new PalInfo(330), // AnimSet 48
            new PalInfo(true, new long[]
            {
                315, // Temple of Dune
                310, // Candy Lane
                344, // Gypsy Road
                342, // Winter Mesa
                339, // Watertopia
                330, // Shamrock Isle
                315, // NOTE: Unused (US only)
                318, // Rocket icon
                323, // Moon Fondue
                332, // Arctic Outback
                331, // Lunar Ledges
                333, // Ripto's Mondo Volcano
                332, // Canyon Hop
                320, // Tiki Tropics
                327, // Haunted Hills
            }), // AnimSet 49
            new PalInfo(335), // AnimSet 50
            new PalInfo(328), // AnimSet 51
            new PalInfo(328), // AnimSet 52
            new PalInfo(332), // AnimSet 53
            new PalInfo(339), // AnimSet 54
            new PalInfo(335), // AnimSet 55
            new PalInfo(339), // AnimSet 56
            new PalInfo(339), // AnimSet 57
            new PalInfo(327), // AnimSet 58 // NOTE: Uses different palette in Gypsy Road
            new PalInfo(333), // AnimSet 59
            new PalInfo(339), // AnimSet 60
            new PalInfo(339), // AnimSet 61
            new PalInfo(344), // AnimSet 62
            new PalInfo(310), // AnimSet 63
            new PalInfo(339), // AnimSet 64
            new PalInfo(331), // AnimSet 65
            new PalInfo(333), // AnimSet 66
            new PalInfo(333), // AnimSet 67
            new PalInfo(330), // AnimSet 68
            new PalInfo(330), // AnimSet 69
            new PalInfo(320), // AnimSet 70
            new PalInfo(342), // AnimSet 71
            new PalInfo(342), // AnimSet 72
            new PalInfo(312), // AnimSet 73
            new PalInfo(327), // AnimSet 74
            new PalInfo(321, false), // AnimSet 75
            new PalInfo(339), // AnimSet 76
            new PalInfo(339), // AnimSet 77
            new PalInfo(342), // AnimSet 78
            new PalInfo(312), // AnimSet 79
            new PalInfo(333), // AnimSet 80
            new PalInfo(335), // AnimSet 81
            new PalInfo(327), // AnimSet 82
            new PalInfo(310), // AnimSet 83 // NOTE: Uses different palette in Dreamy Castle
            new PalInfo(344), // AnimSet 84
            new PalInfo(333), // AnimSet 85
            new PalInfo(333), // AnimSet 86
            new PalInfo(339), // AnimSet 87
            new PalInfo(332), // AnimSet 88
            new PalInfo(317), // AnimSet 89
            new PalInfo(341), // AnimSet 90
            new PalInfo(323), // AnimSet 91
            new PalInfo(328), // AnimSet 92
            new PalInfo(323), // AnimSet 93
            new PalInfo(323), // AnimSet 94
            new PalInfo(339), // AnimSet 95
            new PalInfo(344), // AnimSet 96
            new PalInfo(342), // AnimSet 97
            new PalInfo(323), // AnimSet 98
            new PalInfo(312), // AnimSet 99
            new PalInfo(310), // AnimSet 100
            new PalInfo(310), // AnimSet 101
            new PalInfo(312), // AnimSet 102
            new PalInfo(323), // AnimSet 103
            new PalInfo(333), // AnimSet 104
            new PalInfo(332), // AnimSet 105
            new PalInfo(333), // AnimSet 106
            new PalInfo(317), // AnimSet 107
            new PalInfo(342), // AnimSet 108
            new PalInfo(342), // AnimSet 109
            new PalInfo(344), // AnimSet 110
            new PalInfo(323), // AnimSet 111
            new PalInfo(323), // AnimSet 112
            new PalInfo(333), // AnimSet 113
            new PalInfo(339), // AnimSet 114
            new PalInfo(339), // AnimSet 115
            new PalInfo(320), // AnimSet 116
            new PalInfo(320), // AnimSet 117
            new PalInfo(320), // AnimSet 118
            new PalInfo(339), // AnimSet 119
            new PalInfo(339), // AnimSet 120
            new PalInfo(327), // AnimSet 121

            // Agent 9
            new PalInfo(1216, false), // AnimSet 122
            new PalInfo(1215, false), // AnimSet 123
            new PalInfo(1216, false), // AnimSet 124
            new PalInfo(1216, false), // AnimSet 125
            new PalInfo(1216, false), // AnimSet 126
            new PalInfo(1209, false), // AnimSet 127
            new PalInfo(1216, false), // AnimSet 128
            new PalInfo(1216, false), // AnimSet 129
            new PalInfo(1216, false), // AnimSet 130
            new PalInfo(1216, false), // AnimSet 131
            new PalInfo(1216, false), // AnimSet 132
            new PalInfo(1216, false), // AnimSet 133
            new PalInfo(false, new long[]
            {
                1209, 1209, // Alpine Adobe
                1211, 1211, // Rumble Jungle
                1215, 1215, // Minty Mines
                1216, 1216, // Volcano Vaults
            }), // AnimSet 134
            new PalInfo(1216, false), // AnimSet 135
            new PalInfo(1216, false), // AnimSet 136
            new PalInfo(1216, false), // AnimSet 137
            new PalInfo(1216, false), // AnimSet 138
            new PalInfo(1209, false), // AnimSet 139 // NOTE: Unused (?)
            new PalInfo(1216, false), // AnimSet 140
            new PalInfo(1211, false), // AnimSet 141
            new PalInfo(1216, false), // AnimSet 142
            new PalInfo(1215, false), // AnimSet 143
            new PalInfo(1216, false), // AnimSet 144
            new PalInfo(1216, false), // AnimSet 145
            new PalInfo(1216, false), // AnimSet 146
            new PalInfo(1211, false), // AnimSet 147
            new PalInfo(1216, false), // AnimSet 148
            new PalInfo(1216, false), // AnimSet 149
            new PalInfo(1215, false), // AnimSet 150

            // Dragon Draughts
            new PalInfo(1436, false), // AnimSet 151
            new PalInfo(1436, false), // AnimSet 152
            
            // Sparx Panic
            new PalInfo(1450), // AnimSet 153
            new PalInfo(1475, false), // AnimSet 154
            new PalInfo(1475, false), // AnimSet 155 // TODO: Transparency glitch - why?
            new PalInfo(1475, false), // AnimSet 156
            new PalInfo(1475, false), // AnimSet 157
            new PalInfo(1475, false), // AnimSet 158
            new PalInfo(1475, false), // AnimSet 159
            new PalInfo(1475, false), // AnimSet 160
            new PalInfo(1475, false), // AnimSet 161
        };

        // Generated from the us table
        public PalInfo[] Spyro2_PalInfoEU => new PalInfo[]
        {
            new PalInfo(true, 1216, 326, 326, 326, 326, 1480), // AnimSet 0 (0)
            new PalInfo(false, 78), // AnimSet 1 (1)
            new PalInfo(false, 173), // AnimSet 2 (2)
            new PalInfo(false, 239), // AnimSet 3
            new PalInfo(false, 239), // AnimSet 4 (3)
            new PalInfo(false, 239), // AnimSet 5
            new PalInfo(false, 239), // AnimSet 6
            new PalInfo(false, 239), // AnimSet 7
            new PalInfo(false, 239), // AnimSet 8
            new PalInfo(false, 238), // AnimSet 9 (4)
            new PalInfo(false, 238), // AnimSet 10 (5)
            new PalInfo(false, 240), // AnimSet 11 (6)
            new PalInfo(true, 326), // AnimSet 12 (7)
            new PalInfo(true, 321), // AnimSet 13 (8)
            new PalInfo(true, 344), // AnimSet 14 (9)
            new PalInfo(true, 328), // AnimSet 15 (10)
            new PalInfo(true, 350), // AnimSet 16 (11)
            new PalInfo(true, 346), // AnimSet 17 (12)
            new PalInfo(true, 326), // AnimSet 18 (13)
            new PalInfo(true, 338), // AnimSet 19 (14)
            new PalInfo(true, 331), // AnimSet 20 (15)
            new PalInfo(true, 350), // AnimSet 21 (16)
            new PalInfo(true, 344), // AnimSet 22 (17)
            new PalInfo(true, 339), // AnimSet 23 (18)
            new PalInfo(true, 339), // AnimSet 24 (19)
            new PalInfo(true, 339), // AnimSet 25 (20)
            new PalInfo(true, 321), // AnimSet 26 (22)
            new PalInfo(true, 338), // AnimSet 27 (23)
            new PalInfo(true, 355), // AnimSet 28 (24)
            new PalInfo(true, 324), // AnimSet 29 (25)
            new PalInfo(true, 348), // AnimSet 30 (26)
            new PalInfo(true, 339), // AnimSet 31 (27)
            new PalInfo(true, 339), // AnimSet 32 (28)
            new PalInfo(true, 331), // AnimSet 33 (29)
            new PalInfo(true, 321), // AnimSet 34 (30)
            new PalInfo(true, 331), // AnimSet 35 (31)
            new PalInfo(true, 329), // AnimSet 36 (32)
            new PalInfo(true, 350), // AnimSet 37 (33)
            new PalInfo(true, 339), // AnimSet 38 (34)
            new PalInfo(true, 339), // AnimSet 39 (35)
            new PalInfo(true, 321, 321, 334, 334, 323, 323, 323, 326, 326, 328, 328, 336, 336, 346, 346, 348, 348, 331, 331, 334, 334, 338, 338, 341, 341, 350, 350, 353, 353, 355, 355), // AnimSet 40 (36)
            new PalInfo(true, 355), // AnimSet 41 (37)
            new PalInfo(true, 321), // AnimSet 42 (38)
            new PalInfo(true, 338), // AnimSet 43 (39)
            new PalInfo(true, 338), // AnimSet 44 (40)
            new PalInfo(true, 352), // AnimSet 45 (41)
            new PalInfo(true, 352), // AnimSet 46 (42)
            new PalInfo(true, 321), // AnimSet 47 (43)
            new PalInfo(true, 329), // AnimSet 48 (44)
            new PalInfo(true, 344), // AnimSet 49 (45)
            new PalInfo(true, 343), // AnimSet 50 (46)
            new PalInfo(true, 323), // AnimSet 51 (47)
            new PalInfo(true, 341), // AnimSet 52 (48)
            new PalInfo(true, 326, 321, 355, 353, 350, 341, 329, 334, 343, 342, 344, 343, 331, 338), // AnimSet 53 (49)
            new PalInfo(true, 346), // AnimSet 54 (50)
            new PalInfo(true, 339), // AnimSet 55 (51)
            new PalInfo(true, 339), // AnimSet 56 (52)
            new PalInfo(true, 343), // AnimSet 57 (53)
            new PalInfo(true, 350), // AnimSet 58 (54)
            new PalInfo(true, 346), // AnimSet 59 (55)
            new PalInfo(true, 350), // AnimSet 60 (56)
            new PalInfo(true, 350), // AnimSet 61 (57)
            new PalInfo(true, 338), // AnimSet 62 (58)
            new PalInfo(true, 344), // AnimSet 63 (59)
            new PalInfo(true, 350), // AnimSet 64 (60)
            new PalInfo(true, 350), // AnimSet 65 (61)
            new PalInfo(true, 355), // AnimSet 66 (62)
            new PalInfo(true, 321), // AnimSet 67 (63)
            new PalInfo(true, 350), // AnimSet 68 (64)
            new PalInfo(true, 342), // AnimSet 69 (65)
            new PalInfo(true, 344), // AnimSet 70 (66)
            new PalInfo(true, 344), // AnimSet 71 (67)
            new PalInfo(true, 341), // AnimSet 72 (68)
            new PalInfo(true, 341), // AnimSet 73 (69)
            new PalInfo(true, 331), // AnimSet 74 (70)
            new PalInfo(true, 353), // AnimSet 75 (71)
            new PalInfo(true, 353), // AnimSet 76 (72)
            new PalInfo(true, 323), // AnimSet 77 (73)
            new PalInfo(true, 338), // AnimSet 78 (74)
            new PalInfo(false, 332), // AnimSet 79 (75)
            new PalInfo(true, 350), // AnimSet 80 (76)
            new PalInfo(true, 353), // AnimSet 81 (78)
            new PalInfo(true, 323), // AnimSet 82 (79)
            new PalInfo(true, 344), // AnimSet 83 (80)
            new PalInfo(true, 346), // AnimSet 84 (81)
            new PalInfo(true, 338), // AnimSet 85 (82)
            new PalInfo(true, 321), // AnimSet 86 (83)
            new PalInfo(true, 355), // AnimSet 87 (84)
            new PalInfo(true, 344), // AnimSet 88 (85)
            new PalInfo(true, 344), // AnimSet 89 (86)
            new PalInfo(true, 350), // AnimSet 90 (87)
            new PalInfo(true, 343), // AnimSet 91 (88)
            new PalInfo(true, 328), // AnimSet 92 (89)
            new PalInfo(true, 352), // AnimSet 93 (90)
            new PalInfo(true, 334), // AnimSet 94 (91)
            new PalInfo(true, 339), // AnimSet 95 (92)
            new PalInfo(true, 334), // AnimSet 96 (93)
            new PalInfo(true, 334), // AnimSet 97 (94)
            new PalInfo(true, 350), // AnimSet 98 (95)
            new PalInfo(true, 355), // AnimSet 99 (96)
            new PalInfo(true, 353), // AnimSet 100 (97)
            new PalInfo(true, 334), // AnimSet 101 (98)
            new PalInfo(true, 323), // AnimSet 102 (99)
            new PalInfo(true, 321), // AnimSet 103 (100)
            new PalInfo(true, 321), // AnimSet 104 (101)
            new PalInfo(true, 323), // AnimSet 105 (102)
            new PalInfo(true, 334), // AnimSet 106 (103)
            new PalInfo(true, 344), // AnimSet 107 (104)
            new PalInfo(true, 343), // AnimSet 108 (105)
            new PalInfo(true, 344), // AnimSet 109 (106)
            new PalInfo(true, 328), // AnimSet 110 (107)
            new PalInfo(true, 353), // AnimSet 111 (108)
            new PalInfo(true, 353), // AnimSet 112 (109)
            new PalInfo(true, 355), // AnimSet 113 (110)
            new PalInfo(true, 334), // AnimSet 114 (111)
            new PalInfo(true, 334), // AnimSet 115 (112)
            new PalInfo(true, 344), // AnimSet 116 (113)
            new PalInfo(true, 350), // AnimSet 117 (114)
            new PalInfo(true, 350), // AnimSet 118 (115)
            new PalInfo(true, 331), // AnimSet 119 (116)
            new PalInfo(true, 331), // AnimSet 120 (117)
            new PalInfo(true, 331), // AnimSet 121 (118)
            new PalInfo(true, 350), // AnimSet 122 (119)
            new PalInfo(true, 350), // AnimSet 123 (120)
            new PalInfo(true, 338), // AnimSet 124 (121)
            new PalInfo(false, 1221), // AnimSet 125 (122)
            new PalInfo(false, 1220), // AnimSet 126 (123)
            new PalInfo(false, 1221), // AnimSet 127 (124)
            new PalInfo(false, 1221), // AnimSet 128 (125)
            new PalInfo(false, 1221), // AnimSet 129 (126)
            new PalInfo(false, 1214), // AnimSet 130 (127)
            new PalInfo(false, 1221), // AnimSet 131 (128)
            new PalInfo(false, 1221), // AnimSet 132 (129)
            new PalInfo(false, 1221), // AnimSet 133 (130)
            new PalInfo(false, 1221), // AnimSet 134 (131)
            new PalInfo(false, 1221), // AnimSet 135 (132)
            new PalInfo(false, 1221), // AnimSet 136 (133)
            new PalInfo(false, 1214, 1214, 1216, 1216, 1220, 1220, 1221, 1221), // AnimSet 137 (134)
            new PalInfo(false, 1221), // AnimSet 138 (135)
            new PalInfo(false, 1221), // AnimSet 139 (136)
            new PalInfo(false, 1221), // AnimSet 140 (137)
            new PalInfo(false, 1221), // AnimSet 141 (138)
            new PalInfo(false, 1214), // AnimSet 142 (139)
            new PalInfo(false, 1221), // AnimSet 143 (140)
            new PalInfo(false, 1216), // AnimSet 144 (141)
            new PalInfo(false, 1221), // AnimSet 145 (142)
            new PalInfo(false, 1220), // AnimSet 146 (143)
            new PalInfo(false, 1221), // AnimSet 147 (144)
            new PalInfo(false, 1221), // AnimSet 148 (145)
            new PalInfo(false, 1221), // AnimSet 149 (146)
            new PalInfo(false, 1216), // AnimSet 150 (147)
            new PalInfo(false, 1221), // AnimSet 151 (148)
            new PalInfo(false, 1221), // AnimSet 152 (149)
            new PalInfo(false, 1220), // AnimSet 153 (150)
            new PalInfo(false, 1441), // AnimSet 154 (151)
            new PalInfo(false, 1441), // AnimSet 155 (152)
            new PalInfo(true, 1455), // AnimSet 156 (153)
            new PalInfo(false, 1480), // AnimSet 157 (154)
            new PalInfo(false, 1480), // AnimSet 158 (155)
            new PalInfo(false, 1480), // AnimSet 159 (156)
            new PalInfo(false, 1480), // AnimSet 160 (157)
            new PalInfo(false, 1480), // AnimSet 161 (158)
        };
    }
}