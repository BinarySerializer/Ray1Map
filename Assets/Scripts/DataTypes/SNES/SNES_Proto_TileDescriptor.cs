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
            ushort value = 0;

            value = (ushort)BitHelpers.SetBits(value, TileIndex, 10, 0);
            value = (ushort)BitHelpers.SetBits(value, Palette, 3, 10);
            value = (ushort)BitHelpers.SetBits(value, IsForeground ? 1 : 0, 1, 13);
            value = (ushort)BitHelpers.SetBits(value, FlipX ? 1 : 0, 1, 14);
            value = (ushort)BitHelpers.SetBits(value, FlipY ? 1 : 0, 1, 15);

            value = s.Serialize<ushort>(value, name: "TileDescriptor");

            TileIndex = BitHelpers.ExtractBits(value, 10, 0);
            Palette = BitHelpers.ExtractBits(value, 3, 10);
            IsForeground = BitHelpers.ExtractBits(value, 1, 13) == 1;
            FlipX = BitHelpers.ExtractBits(value, 1, 14) == 1;
            FlipY = BitHelpers.ExtractBits(value, 1, 15) == 1;

            s.Log($"{nameof(TileIndex)}: {TileIndex}");
            s.Log($"{nameof(Palette)}: {Palette}");
            s.Log($"{nameof(IsForeground)}: {IsForeground}");
            s.Log($"{nameof(FlipX)}: {FlipX}");
            s.Log($"{nameof(FlipY)}: {FlipY}");
        }
    }
}