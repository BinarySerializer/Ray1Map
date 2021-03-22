using System.Collections.Generic;

namespace R1Engine
{
    public class GBAVV_ROM_SpongeBobRevengeOfTheFlyingDutchman : GBAVV_ROM_Generic
    {
        public bool ForceMode7 { get; set; } // Set before serializing

        // Helpers
        public override GBAVV_Generic_MapInfo CurrentMapInfo => LevelInfos[Context.Settings.World].Maps[Context.Settings.Level];
        public override GenericLevelType GetGenericLevelType => CurrentMapInfo.SpongeBob_IsMode7 || ForceMode7 ? GenericLevelType.Mode7 : GenericLevelType.Map2D;

        // Common
        public GBAVV_SpongeBobRevengeOfTheFlyingDutchman_LevelInfo[] LevelInfos { get; set; } // Note: This is actually two arrays, but we treat is as one

        public override void SerializeLevelInfo(SerializerObject s, Dictionary<GBAVV_Pointer, Pointer> pointerTable)
        {
            s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.LevelInfo), () =>
            {
                // Serialize level infos
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_SpongeBobRevengeOfTheFlyingDutchman_LevelInfo[31];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_SpongeBobRevengeOfTheFlyingDutchman_LevelInfo>(LevelInfos[i], x => x.SerializeDataForMap = i == s.GameSettings.World ? s.GameSettings.Level : -1, name: $"{nameof(LevelInfos)}[{i}]");
            });
        }
    }
}