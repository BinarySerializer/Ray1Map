using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_Font : BinarySerializable
    {
        public byte Byte_00 { get; set; }
        public bool Is8Bit { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public Pointer<GBAIsometric_RHR_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer Pointer1 { get; set; } // LookupBufferPositions?
        public Pointer Pointer2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_RHR_GraphicsData>(GraphicsDataPointer, name: nameof(GraphicsDataPointer))?.ResolveObject(s);
            Pointer1 = s.SerializePointer(Pointer1, name: nameof(Pointer1));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));
        }
    }
}