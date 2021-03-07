namespace R1Engine
{
    public class GBAVV_Crash1_CutsceneStringFrame : R1Serializable
    {
        public Pointer StringPointer { get; set; }
        public int Int_04 { get; set; }

        // Serialized from pointers
        public GBAVV_LocalizedStringItem String { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StringPointer = s.SerializePointer(StringPointer, name: nameof(StringPointer));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));

            String = s.DoAt(StringPointer, () => s.SerializeObject<GBAVV_LocalizedStringItem>(String, name: nameof(String)));
        }
    }
}