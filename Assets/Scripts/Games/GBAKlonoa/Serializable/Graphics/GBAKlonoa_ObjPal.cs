using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_ObjPal : BinarySerializable
    {
        public RGBA5551Color[] Colors { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Colors = s.SerializeObjectArray<RGBA5551Color>(Colors, 16, name: nameof(Colors));
        }
    }
}