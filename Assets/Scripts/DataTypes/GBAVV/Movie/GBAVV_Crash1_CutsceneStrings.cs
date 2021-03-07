namespace R1Engine
{
    public class GBAVV_Crash1_CutsceneStrings : R1Serializable
    {
        public GBAVV_Crash1_CutsceneEntry[] CutsceneTable { get; set; } // Set before serializing

        public Pointer[] CutscenePointers { get; set; }

        // Serialized from pointers
        public GBAVV_Crash1_CutsceneStringFrame[][] Cutscenes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            CutscenePointers = s.SerializePointerArray(CutscenePointers, CutsceneTable.Length, name: nameof(CutscenePointers));

            if (Cutscenes == null)
                Cutscenes = new GBAVV_Crash1_CutsceneStringFrame[CutsceneTable.Length][];

            for (int i = 0; i < Cutscenes.Length; i++)
                Cutscenes[i] = s.DoAt(CutscenePointers[i], () => s.SerializeObjectArray<GBAVV_Crash1_CutsceneStringFrame>(Cutscenes[i], CutsceneTable[i].FramesCount, name: $"{nameof(Cutscenes)}[{i}]"));
        }
    }
}