namespace R1Engine
{
    public class GBA_MadTraxPlayField : GBA_BaseBlock
    {
        public uint Uint_00 { get; set; }
        public ushort Ushort_04 { get; set; }
        public ushort Ushort_06 { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte Byte_0C { get; set; }
        public byte Byte_0D { get; set; }
        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }
        public ushort Ushort_10 { get; set; }
        public ushort Ushort_12 { get; set; }
        public ushort Ushort_14 { get; set; }
        public ushort Ushort_16 { get; set; }
        public ushort Ushort_18 { get; set; }
        public ushort Ushort_1A { get; set; }

        // Parsed

        public GBA_TileLayer TileLayer { get; set; }
        public GBA_TileKit TileKit { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));
            Ushort_10 = s.Serialize<ushort>(Ushort_10, name: nameof(Ushort_10));
            Ushort_12 = s.Serialize<ushort>(Ushort_12, name: nameof(Ushort_12));
            Ushort_14 = s.Serialize<ushort>(Ushort_14, name: nameof(Ushort_14));
            Ushort_16 = s.Serialize<ushort>(Ushort_16, name: nameof(Ushort_16));
            Ushort_18 = s.Serialize<ushort>(Ushort_18, name: nameof(Ushort_18));
            Ushort_1A = s.Serialize<ushort>(Ushort_1A, name: nameof(Ushort_1A));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            // TODO: Don't hard-code indexes!
            TileLayer = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_TileLayer>(TileLayer, name: nameof(TileLayer)));
            TileKit = s.DoAt(OffsetTable.GetPointer(3), () => s.SerializeObject<GBA_TileKit>(TileKit, name: nameof(TileKit)));
        }
    }
}