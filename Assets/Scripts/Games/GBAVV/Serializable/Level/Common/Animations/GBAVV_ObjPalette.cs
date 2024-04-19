using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_ObjPalette : BinarySerializable
    {
        public SerializableColor[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeIntoArray<SerializableColor>(Palette, 16, BitwiseColor.RGBA5551, name: nameof(Palette));
        }
    }
}