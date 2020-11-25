namespace R1Engine
{
    public class GBC_TileKit : GBC_BaseBlock 
    {
        // Palm OS & Pocket PC
        public uint TilesCount { get; set; }
        public byte[] TileData { get; set; }

        // GBC
        public byte PaletteCount { get; set; }
        public ushort PaletteOffset { get; set; }
        public ushort TileDataOffset { get; set; }
        public UInt24 TileDataLength { get; set; }
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            // Serialize data
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1) {
                var pointer = s.CurrentPointer;
                PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
                PaletteOffset = s.Serialize<ushort>(PaletteOffset, name: nameof(PaletteOffset));
                TileDataOffset = s.Serialize<ushort>(TileDataOffset, name: nameof(TileDataOffset));
                TileDataLength = s.Serialize<UInt24>(TileDataLength, name: nameof(TileDataLength));
                s.DoAt(pointer + PaletteOffset, () => {
                    Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, PaletteCount * 4, name: nameof(Palette));
                });
                s.DoAt(pointer + TileDataOffset, () => {
                    TileData = s.SerializeArray<byte>(TileData, TileDataLength, name: nameof(TileData));
                });
            } else {
                TilesCount = s.Serialize<uint>(TilesCount, name: nameof(TilesCount));
                TileData = s.SerializeArray<byte>(TileData, TilesCount * 0x40, name: nameof(TileData));
            }
        }
    }
}