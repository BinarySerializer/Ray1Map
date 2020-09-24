namespace R1Engine
{
    public class SNES_Proto_TileDescriptor : R1Serializable
    {
        public int TileIndex { get; set; }
        public int Palette { get; set; }
        public bool IsForeground { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                TileIndex = (ushort)bitFunc(TileIndex, 10, name: nameof(TileIndex));
                Palette = bitFunc(Palette, 3, name: nameof(Palette));
                IsForeground = bitFunc(IsForeground ? 1 : 0, 1, name: nameof(IsForeground)) == 1;
                FlipX = bitFunc(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
                FlipY = bitFunc(FlipY ? 1 : 0, 1, name: nameof(FlipY)) == 1;
            });
        }
    }
}