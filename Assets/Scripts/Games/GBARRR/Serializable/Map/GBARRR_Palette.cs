using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_Palette : BinarySerializable
    {
        public SerializableColor[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeIntoArray<SerializableColor>(Palette, 0x100, BitwiseColor.RGBA5551, name: nameof(Palette));
        }
    }
}