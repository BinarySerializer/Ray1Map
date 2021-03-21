namespace R1Engine
{
    public class GBAVV_Volume : R1Serializable
    {
        public GBAVV_Volume_BaseManager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        public Pointer VolumeNamePointer { get; set; }
        public Pointer PrimaryLevelInfoPointer { get; set; }
        public Pointer LevelInfosPointer { get; set; }
        public int LevelsCount { get; set; }
        public Pointer Pointer_0C { get; set; } // Compressed data

        // Serialized from pointers
        public GBAVV_LocalizedString VolumeName { get; set; }
        public GBAVV_Volume_LevelInfo PrimaryLevelInfo { get; set; }
        public GBAVV_Volume_LevelInfo[] LevelInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion != EngineVersion.GBAVV_OverTheHedge && 
                s.GameSettings.EngineVersion != EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts)
            {
                VolumeNamePointer = s.SerializePointer(VolumeNamePointer, name: nameof(VolumeNamePointer));

                if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_MadagascarOperationPenguin)
                    PrimaryLevelInfoPointer = s.SerializePointer(PrimaryLevelInfoPointer, name: nameof(PrimaryLevelInfoPointer));
            }

            LevelInfosPointer = s.SerializePointer(LevelInfosPointer, name: nameof(LevelInfosPointer));

            if (s.GameSettings.EngineVersion != EngineVersion.GBAVV_OverTheHedge &&
                s.GameSettings.EngineVersion != EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts)
            {
                LevelsCount = s.Serialize<int>(LevelsCount, name: nameof(LevelsCount));

                if (s.GameSettings.EngineVersion != EngineVersion.GBAVV_MadagascarOperationPenguin)
                    Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            }

            VolumeName = s.DoAt(VolumeNamePointer, () => s.SerializeObject<GBAVV_LocalizedString>(VolumeName, name: nameof(VolumeName)));

            PrimaryLevelInfo = s.DoAt(PrimaryLevelInfoPointer, () => s.SerializeObject<GBAVV_Volume_LevelInfo>(PrimaryLevelInfo, x => x.CurrentLevInfo = CurrentLevInfo?.Level == -1 ? CurrentLevInfo : null, name: nameof(PrimaryLevelInfo)));

            s.DoAt(LevelInfosPointer, () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_Volume_LevelInfo[LevelsCount];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_Volume_LevelInfo>(LevelInfos[i], x => x.CurrentLevInfo = i == CurrentLevInfo?.Level ? CurrentLevInfo : null, name: $"{nameof(LevelInfos)}[{i}]");
            });
        }
    }
}