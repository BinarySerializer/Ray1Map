namespace R1Engine
{
    public class SNES_Proto_ImageDescriptor : R1Serializable
    {

        // See https://wiki.superfamicom.org/snes-sprites (Sprite Table 2)
        public byte Padding0 { get; set; }
        public bool IsEmpty { get; set; } // true = no sprite?
        public byte Padding1 { get; set; }
        public bool IsLarge { get; set; } // true = 16x16, false = 8x8
        // Same link (Sprite Table 1)
        public int TileIndex { get; set; }
        public int Palette { get; set; }
        public int Priority { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<byte>(bitFunc => {
                Padding0 = (byte)bitFunc(Padding0, 3, name: nameof(Padding0));
                IsEmpty = bitFunc(IsEmpty ? 1 : 0, 1, name: nameof(IsEmpty)) == 1;
                Padding1 = (byte)bitFunc(Padding1, 3, name: nameof(Padding1));
                IsLarge = bitFunc(IsLarge ? 1 : 0, 1, name: nameof(IsLarge)) == 1;
            });
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                TileIndex = (ushort)bitFunc(TileIndex, 9, name: nameof(TileIndex));
                Palette = bitFunc(Palette, 3, name: nameof(Palette));
                Priority = bitFunc(Priority, 2, name: nameof(Priority));
                FlipX = bitFunc(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
                FlipY = bitFunc(FlipY ? 1 : 0, 1, name: nameof(FlipY)) == 1;
            });
        }
    }
}