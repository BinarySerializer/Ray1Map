namespace R1Engine
{
    public class GBA_Cluster : GBA_BaseBlock
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public byte Byte_04 { get; set; }
        public byte Byte_05 { get; set; }
        public byte Byte_06 { get; set; }
        public byte Byte_07 { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public byte Byte_0C { get; set; }
        public byte Byte_0D { get; set; }
        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
        }
    }
}