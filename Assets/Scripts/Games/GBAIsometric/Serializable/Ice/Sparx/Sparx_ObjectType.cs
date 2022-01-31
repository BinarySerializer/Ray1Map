using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_ObjectType : BinarySerializable
    {
        public Pointer<Sparx_AnimSet> AnimSet { get; set; }
        public int Int_04 { get; set; }
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }
        public int Int_18 { get; set; }
        public Pointer FunctionPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimSet = s.SerializePointer<Sparx_AnimSet>(AnimSet, resolve: true, name: nameof(AnimSet));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
            Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
            FunctionPointer = s.SerializePointer(FunctionPointer, name: nameof(FunctionPointer));
        }
    }
}