using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_Sprite : BinarySerializable
    {
        public byte Byte_00 { get; set; } // Always 0
        public byte Byte_01 { get; set; } // Always 0
        public short Short_02 { get; set; }
        public short Short_04 { get; set; }
        public byte[] Bytes_06 { get; set; } // Always 0
        public Pointer<Sparx_SpriteGraphics> GraphicsPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
            Short_04 = s.Serialize<short>(Short_04, name: nameof(Short_04));
            Bytes_06 = s.SerializeArray<byte>(Bytes_06, 14, name: nameof(Bytes_06));
            GraphicsPointer = s.SerializePointer<Sparx_SpriteGraphics>(GraphicsPointer, resolve: true, name: nameof(GraphicsPointer));
        }
    }
}