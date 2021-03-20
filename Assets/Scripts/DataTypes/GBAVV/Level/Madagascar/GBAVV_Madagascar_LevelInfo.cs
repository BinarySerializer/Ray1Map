namespace R1Engine
{
    public class GBAVV_Madagascar_LevelInfo : R1Serializable
    {
        public GBAVV_Madagascar_Manager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        public Pointer LevelNamePointer { get; set; }
        public Pointer MapInfosPointer { get; set; }
        public int MapsCount { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }

        // Serialized from pointers
        public GBAVV_LocalizedString LevelName { get; set; }
        public GBAVV_Madagascar_MapInfo[] MapInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelNamePointer = s.SerializePointer(LevelNamePointer, name: nameof(LevelNamePointer));
            MapInfosPointer = s.SerializePointer(MapInfosPointer, name: nameof(MapInfosPointer));
            MapsCount = s.Serialize<int>(MapsCount, name: nameof(MapsCount));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));

            LevelName = s.DoAt(LevelNamePointer, () => s.SerializeObject<GBAVV_LocalizedString>(LevelName, name: nameof(LevelName)));

            s.DoAt(MapInfosPointer, () =>
            {
                if (MapInfos == null)
                    MapInfos = new GBAVV_Madagascar_MapInfo[MapsCount];

                for (int i = 0; i < MapInfos.Length; i++)
                    MapInfos[i] = s.SerializeObject<GBAVV_Madagascar_MapInfo>(MapInfos[i], x => x.SerializeData = i == CurrentLevInfo?.Map, name: $"{nameof(MapInfos)}[{i}]");
            });
        }
    }
}