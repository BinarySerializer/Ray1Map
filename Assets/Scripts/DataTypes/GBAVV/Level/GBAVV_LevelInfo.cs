namespace R1Engine
{
    public class GBAVV_LevelInfo : R1Serializable
    {
        public GBAVV_BaseManager.LevInfo LevInfo { get; set; } // Set before serializing if it's the current level

        public uint LocIndex_LevelName { get; set; }
        public uint LevelTheme { get; set; } // Used to set the music, warp room preview & tile palette modifications
        public GBAVV_Time Time_Sapphire { get; set; }
        public GBAVV_Time Time_Gold { get; set; }
        public GBAVV_Time Time_Platinum { get; set; }
        public uint Uint_14 { get; set; }
        public uint Uint_18 { get; set; }
        public bool IsBossFight { get; set; }
        public Pointer LevelDataPointer { get; set; }

        // Fusion
        public Pointer Fusion_LevelNamePointer { get; set; }
        public Pointer Fusion_MapDataPointer { get; set; }
        public Pointer Fusion_Pointer_08 { get; set; }
        public uint Fusion_Uint_0C { get; set; }

        // Serialized from pointers
        public GBAVV_LevelData LevelData { get; set; }

        // Fusion
        public GBAVV_Fusion_LocalizedString Fusion_LevelName { get; set; }
        public GBAVV_WorldMap_Data Fusion_MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion != EngineVersion.GBAVV_Fusion)
            {
                LocIndex_LevelName = s.Serialize<uint>(LocIndex_LevelName, name: nameof(LocIndex_LevelName));
                s.Log($"LevelName: {s.Context.GetStoredObject<GBAVV_LocTable>(GBAVV_BaseManager.LocTableID)?.Strings[LocIndex_LevelName]}");
                LevelTheme = s.Serialize<uint>(LevelTheme, name: nameof(LevelTheme));

                Time_Sapphire = s.SerializeObject<GBAVV_Time>(Time_Sapphire, name: nameof(Time_Sapphire));
                Time_Gold = s.SerializeObject<GBAVV_Time>(Time_Gold, name: nameof(Time_Gold));
                Time_Platinum = s.SerializeObject<GBAVV_Time>(Time_Platinum, name: nameof(Time_Platinum));

                Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
                Uint_18 = s.Serialize<uint>(Uint_18, name: nameof(Uint_18));
                IsBossFight = s.Serialize<bool>(IsBossFight, name: nameof(IsBossFight));
                s.SerializeArray<byte>(new byte[3], 3, name: "Padding");
                LevelDataPointer = s.SerializePointer(LevelDataPointer, name: nameof(LevelDataPointer));

                LevelData = s.DoAt(LevelDataPointer, () => s.SerializeObject<GBAVV_LevelData>(LevelData, x => x.LevInfo = LevInfo, name: nameof(LevelData)));
            }
            else
            {
                Fusion_LevelNamePointer = s.SerializePointer(Fusion_LevelNamePointer, name: nameof(Fusion_LevelNamePointer));
                Fusion_MapDataPointer = s.SerializePointer(Fusion_MapDataPointer, name: nameof(Fusion_MapDataPointer));
                Fusion_Pointer_08 = s.SerializePointer(Fusion_Pointer_08, name: nameof(Fusion_Pointer_08));
                Fusion_Uint_0C = s.Serialize<uint>(Fusion_Uint_0C, name: nameof(Fusion_Uint_0C));

                Fusion_LevelName = s.DoAt(Fusion_LevelNamePointer, () => s.SerializeObject<GBAVV_Fusion_LocalizedString>(Fusion_LevelName, name: nameof(Fusion_LevelName)));

                if (LevInfo != null)
                    Fusion_MapData = s.DoAt(Fusion_MapDataPointer, () => s.SerializeObject<GBAVV_WorldMap_Data>(Fusion_MapData, name: nameof(Fusion_MapData)));
            }
        }
    }
}