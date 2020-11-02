namespace R1Engine
{
    public class GBAIsometric_Spyro_ROM : GBA_ROMBase
    {
        public GBAIsometric_Spyro_DataTable DataTable { get; set; }
        
        public GBAIsometric_Spyro_LevelInfo[][] LevelInfos { get; set; }

        public GBAIsometric_Spyro_UnkStruct1[] UnkStructs1 { get; set; }
        public GBAIsometric_Spyro_UnkStruct2[] UnkStructs2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var pointerTabe = PointerTables.GBAIsometric_Spyro_PointerTable(s.GameSettings.GameModeSelection, Offset.file);
            var manager = (GBAIsometric_Spyro_Manager)s.GameSettings.GetGameManager;

            // Serialize primary data table and store it so we can get the data blocks
            DataTable = s.DoAt(pointerTabe[GBAIsometric_Spyro_Pointer.DataTable], () => s.SerializeObject<GBAIsometric_Spyro_DataTable>(DataTable, name: nameof(DataTable)));
            s.Context.StoreObject(nameof(DataTable), DataTable);

            var levelInfo = manager.LevelInfos;

            if (LevelInfos == null)
                LevelInfos = new GBAIsometric_Spyro_LevelInfo[levelInfo.Length][];

            for (int world = 0; world < LevelInfos.Length; world++)
            {
                if (levelInfo[world].UsesPointerArray)
                {
                    var pointers = s.DoAt(new Pointer(levelInfo[world].Offsets[s.GameSettings.GameModeSelection], Offset.file), () => s.SerializePointerArray(default, levelInfo[world].Length, name: $"{nameof(LevelInfos)}Pointers[{world}]"));

                    if (LevelInfos[world] == null)
                        LevelInfos[world] = new GBAIsometric_Spyro_LevelInfo[levelInfo[world].Length];

                    for (int levelIndex = 0; levelIndex < LevelInfos[world].Length; levelIndex++)
                    {
                        LevelInfos[world][levelIndex] = s.DoAt(pointers[levelIndex], () => s.SerializeObject<GBAIsometric_Spyro_LevelInfo>(LevelInfos[world][levelIndex], x =>
                        {
                            x.Is2D = levelInfo[world].Is2D;
                            x.SerializeData = s.GameSettings.World == world;
                            x.ID = (uint)levelIndex; // Default to use the index of the entry - this will get replaced with an actual index if it exists
                        }, name: $"{nameof(LevelInfos)}[{world}][{levelIndex}]"));
                    }
                }
                else
                {
                    s.DoAt(new Pointer(levelInfo[world].Offsets[s.GameSettings.GameModeSelection], Offset.file), () =>
                    {
                        if (LevelInfos[world] == null)
                            LevelInfos[world] = new GBAIsometric_Spyro_LevelInfo[levelInfo[world].Length];

                        for (int levelIndex = 0; levelIndex < LevelInfos[world].Length; levelIndex++)
                        {
                            LevelInfos[world][levelIndex] = s.SerializeObject<GBAIsometric_Spyro_LevelInfo>(LevelInfos[world][levelIndex], x =>
                            {
                                x.Is2D = levelInfo[world].Is2D;
                                x.SerializeData = s.GameSettings.World == world;
                                x.ID = (uint)levelIndex; // Default to use the index of the entry - this will get replaced with an actual index if it exists
                            }, name: $"{nameof(LevelInfos)}[{world}][{levelIndex}]");
                        }
                    });
                }
            }

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
            {
                // Serialize unknown structs
                UnkStructs1 = s.DoAt(Offset + 0x1c8024, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct1>(UnkStructs1, 196, name: nameof(UnkStructs1)));
                UnkStructs2 = s.DoAt(Offset + 0x1bf644, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct2>(UnkStructs2, 38, name: nameof(UnkStructs2)));
            }
        }
    }
}