using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_ObjectType : BinarySerializable
    {
        public Pointer<Sparx_AnimSet> AnimSet { get; set; }
        public int AnimIndex { get; set; }
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int PaletteIndex { get; set; }
        public int Int_18 { get; set; }
        public Pointer FunctionPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimSet = s.SerializePointer<Sparx_AnimSet>(AnimSet, name: nameof(AnimSet))?.ResolveObject(s);
            AnimIndex = s.Serialize<int>(AnimIndex, name: nameof(AnimIndex));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            PaletteIndex = s.Serialize<int>(PaletteIndex, name: nameof(PaletteIndex));
            Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
            FunctionPointer = s.SerializePointer(FunctionPointer, name: nameof(FunctionPointer));
        }
    }
}