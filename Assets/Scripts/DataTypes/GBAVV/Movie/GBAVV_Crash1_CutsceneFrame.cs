using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Crash1_CutsceneFrame : BinarySerializable
    {
        public Pointer GraphicsPointer { get; set; }
        public int Int_04 { get; set; }
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }
        public int Int_18 { get; set; }

        // Serialized from pointers
        public GBAVV_Crash1_CutsceneFrameGraphics Graphics { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsPointer = s.SerializePointer(GraphicsPointer, name: nameof(GraphicsPointer));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
            Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));

            Graphics = s.DoAt(GraphicsPointer, () => s.SerializeObject<GBAVV_Crash1_CutsceneFrameGraphics>(Graphics, name: nameof(Graphics)));
        }
    }
}