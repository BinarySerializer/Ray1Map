using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_Palette : BinarySerializable
    {
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 0x100, name: nameof(Palette));
        }
    }
}