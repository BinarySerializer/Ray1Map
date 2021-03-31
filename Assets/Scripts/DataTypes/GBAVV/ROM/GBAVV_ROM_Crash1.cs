using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_ROM_Crash1 : GBAVV_ROM_Crash
    {
        // Cutscenes
        public Pointer[] CutsceneStringPointers { get; set; }
        public GBAVV_Crash1_CutsceneStrings[] CutsceneStrings { get; set; }
        public GBAVV_Crash1_CutsceneEntry[] CutsceneTable { get; set; }

        // WorldMap
        public GBAVV_WorldMap_Crash1_LevelIcon[] WorldMapLevelIcons { get; set; }

        public override void SerializeAdditionalData(SerializerObject s, Dictionary<GBAVV_Pointer, Pointer> pointerTable)
        {
            CutsceneTable = s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Crash1_CutsceneTable), () => s.SerializeObjectArray<GBAVV_Crash1_CutsceneEntry>(CutsceneTable, 11, name: nameof(CutsceneTable)));

            CutsceneStringPointers = s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Crash1_CutsceneStrings), () => s.SerializePointerArray(CutsceneStringPointers, 6, name: nameof(CutsceneStringPointers)));

            if (CutsceneStrings == null)
                CutsceneStrings = new GBAVV_Crash1_CutsceneStrings[CutsceneStringPointers.Length];

            for (int i = 0; i < CutsceneStrings.Length; i++)
                CutsceneStrings[i] = s.DoAt(CutsceneStringPointers[i], () => s.SerializeObject<GBAVV_Crash1_CutsceneStrings>(CutsceneStrings[i], x => x.CutsceneTable = CutsceneTable, name: $"{nameof(CutsceneStrings)}[{i}]"));

            if (CurrentLevInfo.IsWorldMap)
                WorldMapLevelIcons = s.DoAt(pointerTable[GBAVV_Pointer.Crash1_WorldMapLevelIcons], () => s.SerializeObjectArray<GBAVV_WorldMap_Crash1_LevelIcon>(WorldMapLevelIcons, 10, name: nameof(WorldMapLevelIcons)));
        }
    }
}