namespace R1Engine
{
    public class GBACrash_LevelData : R1Serializable
    {
        public bool SerializeAll { get; set; }
        public GBACrash_Crash2_Manager.LevInfo LevInfo { get; set; } // Set before serializing if it's the current level

        public uint MapsCount { get; set; }
        public Pointer MapsPointer { get; set; }
        public Pointer BonusMapPointer { get; set; }
        public Pointer ChallengeMapPointer { get; set; }

        // Serialized from pointers

        public Pointer[] MapsPointers { get; set; }

        public GBACrash_MapInfo[] Maps { get; set; }
        public GBACrash_MapInfo BonusMap { get; set; }
        public GBACrash_MapInfo ChallengeMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapsCount = s.Serialize<uint>(MapsCount, name: nameof(MapsCount));
            MapsPointer = s.SerializePointer(MapsPointer, name: nameof(MapsPointer));
            BonusMapPointer = s.SerializePointer(BonusMapPointer, name: nameof(BonusMapPointer));
            ChallengeMapPointer = s.SerializePointer(ChallengeMapPointer, name: nameof(ChallengeMapPointer));

            MapsPointers = s.DoAt(MapsPointer, () => s.SerializePointerArray(MapsPointers, MapsCount, name: nameof(MapsPointers)));

            if (Maps == null)
                Maps = new GBACrash_MapInfo[MapsCount];

            for (int i = 0; i < Maps.Length; i++)
            {
                if (SerializeAll || (LevInfo?.MapIndex == i && LevInfo?.MapType == GBACrash_Crash2_Manager.LevInfo.Type.Normal))
                    Maps[i] = s.DoAt(MapsPointers[i], () => s.SerializeObject<GBACrash_MapInfo>(Maps[i], name: $"{nameof(Maps)}[{i}]"));
            }

            if (SerializeAll || LevInfo?.MapType == GBACrash_Crash2_Manager.LevInfo.Type.Bonus)
                BonusMap = s.DoAt(BonusMapPointer, () => s.SerializeObject<GBACrash_MapInfo>(BonusMap, name: nameof(BonusMap)));

            if (SerializeAll || LevInfo?.MapType == GBACrash_Crash2_Manager.LevInfo.Type.Challenge)
                ChallengeMap = s.DoAt(ChallengeMapPointer, () => s.SerializeObject<GBACrash_MapInfo>(ChallengeMap, name: nameof(ChallengeMap)));
        }
    }
}