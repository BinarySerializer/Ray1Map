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

        public ARGB1555Color[] CommonPalette { get; set; }

        public GBAIsometric_Spyro_State_NPC[] States_NPC { get; set; }

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
            Localization = s.SerializeObject<GBAIsometric_Spyro_Localization>(Localization, name: nameof(Localization));

            var id = GetLevelDataID(s.GameSettings);

            // Serialize level data
            LevelData = s.DoAt<GBAIsometric_Spyro_LevelDataArray>(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LevelData), () => s.SerializeObject(LevelData, x =>
            {
                x.Length = manager.LevelDataCount;
                x.UesPointerArray = true;
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
                    x.UesPointerArray = false;
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
                    x.UesPointerArray = true;
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
                    x.UesPointerArray = true;
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

            // Serialize common palette for Spyro 2
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                CommonPalette = GBAIsometric_Spyro_DataBlockIndex.FromIndex(s, (ushort)(s.GameSettings.GameModeSelection == GameModeSelection.SpyroSeasonFlameEU ? 332 : 321)).DoAtBlock(size => s.SerializeObjectArray<ARGB1555Color>(CommonPalette, 256, name: nameof(CommonPalette)));

            // Serialize object states
            States_NPC = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.States_NPC), () => s.SerializeObjectArray<GBAIsometric_Spyro_State_NPC>(States_NPC, 49, name: nameof(States_NPC)));

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
    }
}