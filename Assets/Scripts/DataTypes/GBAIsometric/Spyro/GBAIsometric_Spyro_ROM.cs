namespace R1Engine
{
    public class GBAIsometric_Spyro_ROM : GBA_ROMBase
    {
        public GBAIsometric_Spyro_DataTable DataTable { get; set; }
        
        public Pointer[] LevelInfoPointers { get; set; }
        public Pointer[] LevelInfo2DPointers { get; set; }
        public GBAIsometric_Spyro_LevelInfo[] LevelInfos { get; set; }
        public GBAIsometric_Spyro_LevelInfo[] LevelInfos2D { get; set; }

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

            // Serialize level infos
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
            {
                LevelInfo2DPointers = s.DoAt(pointerTabe[GBAIsometric_Spyro_Pointer.LevelInfo2D], () => s.SerializePointerArray(LevelInfo2DPointers, manager.LevelInfo2DCount, name: nameof(LevelInfo2DPointers)));

                if (LevelInfos2D == null)
                    LevelInfos2D = new GBAIsometric_Spyro_LevelInfo[LevelInfo2DPointers.Length];

                for (int i = 0; i < LevelInfos2D.Length; i++)
                    LevelInfos2D[i] = s.DoAt(LevelInfo2DPointers[i], () => s.SerializeObject<GBAIsometric_Spyro_LevelInfo>(LevelInfos2D[i], x => x.Is2D = true, name: $"{nameof(LevelInfos2D)}[{i}]"));
            }
            else
            {
                LevelInfos2D = s.DoAt(pointerTabe[GBAIsometric_Spyro_Pointer.LevelInfo2D], () => s.SerializeObjectArray<GBAIsometric_Spyro_LevelInfo>(LevelInfos2D, manager.LevelInfo2DCount, x => x.Is2D = true, name: nameof(LevelInfos2D)));
            }

            LevelInfoPointers = s.DoAt(pointerTabe[GBAIsometric_Spyro_Pointer.LevelInfo], () => s.SerializePointerArray(LevelInfoPointers, manager.LevelInfoCount, name: nameof(LevelInfoPointers)));

            if (LevelInfos == null)
                LevelInfos = new GBAIsometric_Spyro_LevelInfo[LevelInfoPointers.Length];

            for (int i = 0; i < LevelInfos.Length; i++)
                LevelInfos[i] = s.DoAt(LevelInfoPointers[i], () => s.SerializeObject<GBAIsometric_Spyro_LevelInfo>(LevelInfos[i], name: $"{nameof(LevelInfos)}[{i}]"));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
            {
                // Serialize unknown structs
                UnkStructs1 = s.DoAt(Offset + 0x1c8024, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct1>(UnkStructs1, 196, name: nameof(UnkStructs1)));
                UnkStructs2 = s.DoAt(Offset + 0x1bf644, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct2>(UnkStructs2, 38, name: nameof(UnkStructs2)));
            }
        }
    }
}