namespace R1Engine
{
    public class GBAVV_LevelData : R1Serializable
    {
        public bool SerializeAll { get; set; }
        public GBAVV_BaseManager.LevInfo LevInfo { get; set; } // Set before serializing if it's the current level

        public uint MapsCount { get; set; }
        public Pointer MapsPointer { get; set; }
        public Pointer BonusMapPointer { get; set; }
        public Pointer ChallengeMapPointer { get; set; }

        // Serialized from pointers

        public Pointer[] MapsPointers { get; set; }

        public GBAVV_MapInfo[] Maps { get; set; }
        public GBAVV_MapInfo BonusMap { get; set; }
        public GBAVV_MapInfo ChallengeMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapsCount = s.Serialize<uint>(MapsCount, name: nameof(MapsCount));
            MapsPointer = s.SerializePointer(MapsPointer, name: nameof(MapsPointer));
            BonusMapPointer = s.SerializePointer(BonusMapPointer, name: nameof(BonusMapPointer));
            ChallengeMapPointer = s.SerializePointer(ChallengeMapPointer, name: nameof(ChallengeMapPointer));

            MapsPointers = s.DoAt(MapsPointer, () => s.SerializePointerArray(MapsPointers, MapsCount, name: nameof(MapsPointers)));

            if (Maps == null)
                Maps = new GBAVV_MapInfo[MapsCount];

            for (int i = 0; i < Maps.Length; i++)
            {
                if (SerializeAll || (LevInfo?.MapIndex == i && LevInfo?.MapType == GBAVV_BaseManager.LevInfo.Type.Normal))
                    Maps[i] = s.DoAt(MapsPointers[i], () => s.SerializeObject<GBAVV_MapInfo>(Maps[i], name: $"{nameof(Maps)}[{i}]"));
            }

            if (SerializeAll || LevInfo?.MapType == GBAVV_BaseManager.LevInfo.Type.Bonus)
                BonusMap = s.DoAt(BonusMapPointer, () => s.SerializeObject<GBAVV_MapInfo>(BonusMap, name: nameof(BonusMap)));

            if (SerializeAll || LevInfo?.MapType == GBAVV_BaseManager.LevInfo.Type.Challenge)
                ChallengeMap = s.DoAt(ChallengeMapPointer, () => s.SerializeObject<GBAVV_MapInfo>(ChallengeMap, name: nameof(ChallengeMap)));
        }
    }
}