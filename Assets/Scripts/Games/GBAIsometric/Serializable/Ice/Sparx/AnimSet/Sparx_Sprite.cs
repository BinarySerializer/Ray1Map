using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_Sprite : BinarySerializable
    {
        public byte Byte_00 { get; set; } // Always 0
        public byte Byte_01 { get; set; } // Always 0
        public short XPos { get; set; }
        public short YPos { get; set; }
        public byte[] Bytes_06 { get; set; } // Always 0
        public Pointer<Sparx_SpriteGraphics> GraphicsPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Bytes_06 = s.SerializeArray<byte>(Bytes_06, 14, name: nameof(Bytes_06));
            GraphicsPointer = s.SerializePointer<Sparx_SpriteGraphics>(GraphicsPointer, resolve: true, name: nameof(GraphicsPointer));
        }
    }
}