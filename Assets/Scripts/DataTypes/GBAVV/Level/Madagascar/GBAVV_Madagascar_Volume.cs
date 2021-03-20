namespace R1Engine
{
    public class GBAVV_Madagascar_Volume : R1Serializable
    {
        public GBAVV_Madagascar_Manager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        public Pointer VolumeNamePointer { get; set; }
        public Pointer LevelInfosPointer { get; set; }
        public int LevelsCount { get; set; }
        public Pointer Pointer_0C { get; set; }

        // Serialized from pointers
        public GBAVV_LocalizedString VolumeName { get; set; }
        public GBAVV_Madagascar_LevelInfo[] LevelInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            VolumeNamePointer = s.SerializePointer(VolumeNamePointer, name: nameof(VolumeNamePointer));
            LevelInfosPointer = s.SerializePointer(LevelInfosPointer, name: nameof(LevelInfosPointer));
            LevelsCount = s.Serialize<int>(LevelsCount, name: nameof(LevelsCount));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));

            VolumeName = s.DoAt(VolumeNamePointer, () => s.SerializeObject<GBAVV_LocalizedString>(VolumeName, name: nameof(VolumeName)));

            s.DoAt(LevelInfosPointer, () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_Madagascar_LevelInfo[LevelsCount];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_Madagascar_LevelInfo>(LevelInfos[i], x => x.CurrentLevInfo = i == CurrentLevInfo?.Level ? CurrentLevInfo : null, name: $"{nameof(LevelInfos)}[{i}]");
            });
        }
    }
}