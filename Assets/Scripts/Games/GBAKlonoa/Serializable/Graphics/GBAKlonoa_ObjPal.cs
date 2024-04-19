using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_ObjPal : BinarySerializable
    {
        public SerializableColor[] Colors { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Colors = s.SerializeIntoArray<SerializableColor>(Colors, 16, BitwiseColor.RGBA5551, name: nameof(Colors));
        }
    }
}