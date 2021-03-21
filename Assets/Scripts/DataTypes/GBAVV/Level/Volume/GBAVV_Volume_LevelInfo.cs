namespace R1Engine
{
    public class GBAVV_Volume_LevelInfo : R1Serializable
    {
        public GBAVV_Volume_BaseManager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        public int Int_00 { get; set; }
        public Pointer LevelNamePointer { get; set; }
        public Pointer MapInfosPointer { get; set; }
        public int MapsCount { get; set; }
        public int[] Ints_0C { get; set; }

        // Serialized from pointers
        public GBAVV_LocalizedString LevelName { get; set; }
        public string LevelNameString { get; set; }
        public string GetLevelName => LevelName?.DefaultString ?? LevelNameString;
        public GBAVV_Volume_MapInfo[] MapInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_MadagascarOperationPenguin)
                Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Madagascar || 
                s.GameSettings.EngineVersion == EngineVersion.GBAVV_MadagascarOperationPenguin || 
                s.GameSettings.EngineVersion == EngineVersion.GBAVV_OverTheHedge)
                LevelNamePointer = s.SerializePointer(LevelNamePointer, name: nameof(LevelNamePointer));

            MapInfosPointer = s.SerializePointer(MapInfosPointer, name: nameof(MapInfosPointer));
            MapsCount = s.Serialize<int>(MapsCount, name: nameof(MapsCount));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_OverTheHedge)
            {
                Ints_0C = s.SerializeArray<int>(Ints_0C, 11, name: nameof(Ints_0C));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Madagascar || 
                     s.GameSettings.EngineVersion == EngineVersion.GBAVV_MadagascarOperationPenguin)
            {
                Ints_0C = s.SerializeArray<int>(Ints_0C, 3, name: nameof(Ints_0C));
            }
            else
            {
                Ints_0C = s.SerializeArray<int>(Ints_0C, 2, name: nameof(Ints_0C));
            }

            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_OverTheHedge)
                LevelNameString = s.DoAt(LevelNamePointer, () => s.SerializeString(LevelNameString, name: nameof(LevelNameString)));
            else
                LevelName = s.DoAt(LevelNamePointer, () => s.SerializeObject<GBAVV_LocalizedString>(LevelName, name: nameof(LevelName)));

            s.DoAt(MapInfosPointer, () =>
            {
                if (MapInfos == null)
                    MapInfos = new GBAVV_Volume_MapInfo[MapsCount];

                for (int i = 0; i < MapInfos.Length; i++)
                    MapInfos[i] = s.SerializeObject<GBAVV_Volume_MapInfo>(MapInfos[i], x => x.SerializeData = i == CurrentLevInfo?.Map, name: $"{nameof(MapInfos)}[{i}]");
            });
        }
    }
}