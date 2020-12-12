namespace R1Engine
{
    public class SNES_Proto_ImageDescriptor : R1Serializable
    {
        public bool IsLarge { get; set; }
        public byte Unknown { get; set; }
        public int TileIndex { get; set; }
        public int Palette { get; set; }
        public int Priority { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<byte>(bitFunc => {
                Unknown = (byte)bitFunc(Unknown, 7, name: nameof(Unknown));
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