namespace R1Engine
{
    public class GBACrash_Mode7_ObjFrame : R1Serializable
    {
        public byte Width { get; set; }
        public byte Height { get; set; }
        public ushort Flags { get; set; }
        public byte[] TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            TileSet = s.SerializeArray<byte>(TileSet, Width * Height * 0x20, name: nameof(TileSet));
        }
    }
}