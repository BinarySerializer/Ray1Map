using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Volume_LevelInfo : BinarySerializable
    {
        public GBAVV_Volume_BaseManager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        public int Int_00 { get; set; }
        public Pointer LevelNamePointer { get; set; }
        public Pointer InternalLevelNamePointer { get; set; }
        public Pointer MapInfosPointer { get; set; }
        public int MapsCount { get; set; }
        public int[] Ints_0C { get; set; }

        // Serialized from pointers
        public GBAVV_LocalizedString LevelName { get; set; }
        public string InternalLevelName { get; set; }
        public GBAVV_Volume_MapInfo[] MapInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_MadagascarOperationPenguin)
                Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedge ||
                s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts)
                InternalLevelNamePointer = s.SerializePointer(InternalLevelNamePointer, name: nameof(InternalLevelNamePointer));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Madagascar || 
                s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_MadagascarOperationPenguin ||
                s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts)
                LevelNamePointer = s.SerializePointer(LevelNamePointer, name: nameof(LevelNamePointer));

            MapInfosPointer = s.SerializePointer(MapInfosPointer, name: nameof(MapInfosPointer));
            MapsCount = s.Serialize<int>(MapsCount, name: nameof(MapsCount));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedge ||
                s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_OverTheHedgeHammyGoesNuts)
            {
                Ints_0C = s.SerializeArray<int>(Ints_0C, 11, name: nameof(Ints_0C)); // One of the values is a pointer
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Madagascar || 
                     s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_MadagascarOperationPenguin)
            {
                Ints_0C = s.SerializeArray<int>(Ints_0C, 3, name: nameof(Ints_0C));
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpiderMan3)
            {
                Ints_0C = s.SerializeArray<int>(Ints_0C, 1, name: nameof(Ints_0C));
                LevelNamePointer = s.SerializePointer(LevelNamePointer, name: nameof(LevelNamePointer));
            }
            else
            {
                Ints_0C = s.SerializeArray<int>(Ints_0C, 2, name: nameof(Ints_0C));
            }

            InternalLevelName = s.DoAt(InternalLevelNamePointer, () => s.SerializeString(InternalLevelName, name: nameof(InternalLevelName)));
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