namespace R1Engine
{
    public class GBA_Cluster : GBA_BaseBlock
    {
        public int OriginX { get; set; }
        public int OriginY { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public byte Byte_0C { get; set; }
        public byte Byte_0D { get; set; }
        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            OriginX = s.Serialize<int>(OriginX, name: nameof(OriginX));
            OriginY = s.Serialize<int>(OriginY, name: nameof(OriginY));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
        }
    }
}