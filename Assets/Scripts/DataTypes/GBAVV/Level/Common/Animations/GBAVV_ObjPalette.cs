using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_ObjPalette : BinarySerializable
    {
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 16, name: nameof(Palette));
        }
    }
}