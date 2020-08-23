namespace R1Engine
{
    public class GBA_Cluster : GBA_BaseBlock
    {
        public int ScrollX { get; set; }
        public int ScrollY { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public byte Byte_0C { get; set; }

        // Seems to be 1 when a special effect should be used (like scrolling sky, floating skulls etc. - the type of effect is determined from the game info level table)
        public byte Byte_0D { get; set; }

        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }

        public float ScrollXFloat => ScrollX / (float)0x10000;
        public float ScrollYFloat => ScrollY / (float)0x10000;

        public override void SerializeBlock(SerializerObject s)
        {
            ScrollX = s.Serialize<int>(ScrollX, name: nameof(ScrollX));
            ScrollY = s.Serialize<int>(ScrollY, name: nameof(ScrollY));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
        }
    }
}