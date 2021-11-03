using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_ROM_Frogger : GBAVV_ROM_Generic
    {
        // Helpers
        public override GBAVV_Generic_MapInfo CurrentMapInfo
        {
            get
            {
                if (Context.GetR1Settings().World == 0)
                    return MapInfos[Context.GetR1Settings().Level];
                
                var lvl = AdditionalLevelInfos[Context.GetR1Settings().Level / 2];

                return Context.GetR1Settings().Level % 2 == 0 ? lvl.LevelData.MapInfo1 : lvl.LevelData.MapInfo2;
            }
        }

        // Common
        public Pointer[] MapInfoPointers { get; set; }
        public GBAVV_Generic_MapInfo[] MapInfos { get; set; }
        public GBAVV_Frogger_AdditionalLevelInfo[] AdditionalLevelInfos { get; set; }

        public override void SerializeLevelInfo(SerializerObject s, Dictionary<GBAVV_Pointer, Pointer> pointerTable)
        {
            // Serialize level infos
            if (s.GetR1Settings().World == 0)
            {
                MapInfoPointers = s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.LevelInfo), () => s.SerializePointerArray(MapInfoPointers, 17, name: nameof(MapInfoPointers)));

                if (MapInfos == null)
                    MapInfos = new GBAVV_Generic_MapInfo[MapInfoPointers.Length];

                for (int i = 0; i < MapInfos.Length; i++)
                    MapInfos[i] = s.DoAt(MapInfoPointers[i], () => s.SerializeObject<GBAVV_Generic_MapInfo>(MapInfos[i], x => x.SerializeData = i == s.GetR1Settings().Level, name: $"{nameof(MapInfos)}[{i}]"));
            }
            else
            {
                s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Frogger_AdditionalLevels), () =>
                {
                    if (AdditionalLevelInfos == null)
                        AdditionalLevelInfos = new GBAVV_Frogger_AdditionalLevelInfo[5];

                    for (int i = 0; i < AdditionalLevelInfos.Length; i++)
                        AdditionalLevelInfos[i] = s.SerializeObject<GBAVV_Frogger_AdditionalLevelInfo>(AdditionalLevelInfos[i], x =>
                        {
                            x.SerializeData1 = i == s.GetR1Settings().Level / 2f;
                            x.SerializeData2 = i == (s.GetR1Settings().Level - 1) / 2f;
                        }, name: $"{nameof(AdditionalLevelInfos)}[{i}]");
                });
            }
        }
    }
}