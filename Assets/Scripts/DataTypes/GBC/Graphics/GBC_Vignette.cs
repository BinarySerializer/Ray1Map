namespace R1Engine
{
    public class GBC_Vignette : GBC_BaseBlock
    {
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte[] UnkData { get; set; }
        public ARGB1555Color[] Palette { get; set; }
        public byte[] RemainingData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize data
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            UnkData = s.SerializeArray<byte>(UnkData, 14, name: nameof(UnkData));
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, 4 * 8, name: nameof(Palette));
            RemainingData = s.SerializeArray<byte>(RemainingData, BlockSize - (s.CurrentPointer - BlockStartPointer), name: nameof(RemainingData));
        }
    }
}