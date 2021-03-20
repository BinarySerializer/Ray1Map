namespace R1Engine
{
    public class GBAVV_Crash_LevelInfo : R1Serializable
    {
        public GBAVV_Crash_BaseManager.CrashLevInfo LevInfo { get; set; } // Set before serializing if it's the current level

        public uint LocIndex_LevelName { get; set; }
        public int LevelTheme { get; set; } // Used to set the music, warp room preview & tile palette modifications
        public GBAVV_Time Time_Sapphire { get; set; }
        public GBAVV_Time Time_Gold { get; set; }
        public GBAVV_Time Time_Platinum { get; set; }
        public uint Uint_14 { get; set; }
        public uint Uint_18 { get; set; }
        public bool IsBossFight { get; set; }
        public Pointer LevelDataPointer { get; set; }

        // Serialized from pointers
        public GBAVV_Crash_LevelData LevelData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LocIndex_LevelName = s.Serialize<uint>(LocIndex_LevelName, name: nameof(LocIndex_LevelName));
            s.Log($"LevelName: {s.Context.GetStoredObject<GBAVV_Crash_LocTable>(GBAVV_Crash_BaseManager.LocTableID)?.Strings[LocIndex_LevelName]}");
            LevelTheme = s.Serialize<int>(LevelTheme, name: nameof(LevelTheme));

            Time_Sapphire = s.SerializeObject<GBAVV_Time>(Time_Sapphire, name: nameof(Time_Sapphire));
            Time_Gold = s.SerializeObject<GBAVV_Time>(Time_Gold, name: nameof(Time_Gold));
            Time_Platinum = s.SerializeObject<GBAVV_Time>(Time_Platinum, name: nameof(Time_Platinum));

            Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
            Uint_18 = s.Serialize<uint>(Uint_18, name: nameof(Uint_18));
            IsBossFight = s.Serialize<bool>(IsBossFight, name: nameof(IsBossFight));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");
            LevelDataPointer = s.SerializePointer(LevelDataPointer, name: nameof(LevelDataPointer));

            LevelData = s.DoAt(LevelDataPointer, () => s.SerializeObject<GBAVV_Crash_LevelData>(LevelData, x => x.LevInfo = LevInfo, name: nameof(LevelData)));
        }
    }
}