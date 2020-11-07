namespace R1Engine
{
    public class GBAIsometric_Spyro_ROM : GBA_ROMBase
    {
        public GBAIsometric_Spyro_DataTable DataTable { get; set; }
        public GBAIsometric_Spyro_Localization Localization { get; set; }

        public GBAIsometric_Spyro_LevelData[][] LevelData { get; set; }
        public GBAIsometric_Spyro_LevelMap[] LevelMaps { get; set; }
        public GBAIsometric_Spyro_LevelObjects[] LevelObjects { get; set; }

        public GBAIsometric_ObjectType[] ObjectTypes { get; set; }
        public GBAIsometric_Spyro_AnimSet[] AnimSets { get; set; }

        public GBAIsometric_Spyro_PortraitSprite[] PortraitSprites { get; set; }
        public GBAIsometric_Spyro_Dialog[] DialogEntries { get; set; }

        public GBAIsometric_Spyro_LevelNameInfo[] LevelNameInfos { get; set; }
        public GBAIsometric_Spyro_UnkStruct[] UnkStructs { get; set; }
        public byte[] LevelIndices { get; set; } // Level index for every map
        public ushort[] GemCounts { get; set; } // The gem count for every level


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

            // Serialize level data
            var levelInfo = manager.LevelInfos;

            if (LevelData == null)
                LevelData = new GBAIsometric_Spyro_LevelData[levelInfo.Length][];

            for (int world = 0; world < LevelData.Length; world++)
            {
                if (levelInfo[world].UsesPointerArray)
                {
                    var pointers = s.DoAt(new Pointer(levelInfo[world].Offsets[s.GameSettings.GameModeSelection], Offset.file), () => s.SerializePointerArray(default, levelInfo[world].Length, name: $"{nameof(LevelData)}Pointers[{world}]"));

                    if (LevelData[world] == null)
                        LevelData[world] = new GBAIsometric_Spyro_LevelData[levelInfo[world].Length];

                    for (int levelIndex = 0; levelIndex < LevelData[world].Length; levelIndex++)
                    {
                        LevelData[world][levelIndex] = s.DoAt(pointers[levelIndex], () => s.SerializeObject<GBAIsometric_Spyro_LevelData>(LevelData[world][levelIndex], x =>
                        {
                            x.Is2D = levelInfo[world].Is2D;
                            x.SerializeData = s.GameSettings.World == world;
                            x.ID = (uint)levelIndex; // Default to use the index of the entry - this will get replaced with an actual index if it exists
                        }, name: $"{nameof(LevelData)}[{world}][{levelIndex}]"));
                    }
                }
                else
                {
                    s.DoAt(new Pointer(levelInfo[world].Offsets[s.GameSettings.GameModeSelection], Offset.file), () =>
                    {
                        if (LevelData[world] == null)
                            LevelData[world] = new GBAIsometric_Spyro_LevelData[levelInfo[world].Length];

                        for (int levelIndex = 0; levelIndex < LevelData[world].Length; levelIndex++)
                        {
                            LevelData[world][levelIndex] = s.SerializeObject<GBAIsometric_Spyro_LevelData>(LevelData[world][levelIndex], x =>
                            {
                                x.Is2D = levelInfo[world].Is2D;
                                x.SerializeData = s.GameSettings.World == world;
                                x.ID = (uint)levelIndex; // Default to use the index of the entry - this will get replaced with an actual index if it exists
                            }, name: $"{nameof(LevelData)}[{world}][{levelIndex}]");
                        }
                    });
                }
            }
            LevelMaps = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.LevelMaps], () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelMap>(LevelMaps, manager.LevelMapsCount, name: nameof(LevelMaps)));
            LevelObjects = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.LevelObjects], () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelObjects>(LevelObjects, levelInfo[0].Length, name: nameof(LevelObjects)));

            // Serialize object data
            ObjectTypes = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.ObjectTypes], () => s.SerializeObjectArray<GBAIsometric_ObjectType>(ObjectTypes, manager.ObjectTypesCount, name: nameof(ObjectTypes)));
            AnimSets = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.AnimSets], () => s.SerializeObjectArray<GBAIsometric_Spyro_AnimSet>(AnimSets, manager.AnimSetsCount, name: nameof(AnimSets)));

            // Serialize portraits
            PortraitSprites = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.PortraitSprites], () => s.SerializeObjectArray<GBAIsometric_Spyro_PortraitSprite>(PortraitSprites, manager.PortraitsCount, name: nameof(PortraitSprites)));

            // Serialize dialog entries
            DialogEntries = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.DialogEntries], () => s.SerializeObjectArray<GBAIsometric_Spyro_Dialog>(DialogEntries, manager.DialogCount, name: nameof(DialogEntries)));

            // Serialize level properties
            GemCounts = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.GemCounts], () => s.SerializeArray<ushort>(GemCounts, manager.PrimaryLevelCount, name: nameof(GemCounts)));
            LevelIndices = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.LevelIndices], () => s.SerializeArray<byte>(LevelIndices, manager.TotalLevelsCount, name: nameof(LevelIndices)));
            LevelNameInfos = s.DoAt(pointerTable[GBAIsometric_Spyro_Pointer.LevelNameInfos], () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelNameInfo>(LevelNameInfos, manager.TotalLevelsCount, name: nameof(LevelNameInfos)));


            // Serialize unknown struct
            if (s.GameSettings.GameModeSelection == GameModeSelection.SpyroAdventureUS)
            {
                UnkStructs = s.DoAt(Offset + 0x1c009c, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct>(UnkStructs, 104, name: nameof(UnkStructs)));
                // 0x1bfa10, same as UnkStruct
            }
        }
    }
}