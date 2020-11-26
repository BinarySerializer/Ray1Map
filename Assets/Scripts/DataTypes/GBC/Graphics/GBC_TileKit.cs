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
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // Serialize data
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1) {
                PaletteCount = s.Serialize<byte>(PaletteCount, name: nameof(PaletteCount));
                PaletteOffset = s.Serialize<ushort>(PaletteOffset, name: nameof(PaletteOffset));
                TileDataOffset = s.Serialize<ushort>(TileDataOffset, name: nameof(TileDataOffset));
                TilesCount = s.Serialize<UInt24>((UInt24)TilesCount, name: nameof(TilesCount));
                s.DoAt(BlockStartPointer + PaletteOffset, () => {
                    Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, PaletteCount * 4, name: nameof(Palette));
                });
                s.DoAt(BlockStartPointer + TileDataOffset, () => {
                    TileData = s.SerializeArray<byte>(TileData, TilesCount * 0x10, name: nameof(TileData));
                });
                // Go to end of block
                s.Goto(BlockStartPointer + TileDataOffset + TilesCount * 0x10);
            } else {
                bool greyScale = s.GameSettings.GameModeSelection == GameModeSelection.RaymanGBCPalmOSGreyscale;
                TilesCount = s.Serialize<uint>(TilesCount, name: nameof(TilesCount));
                TileData = s.SerializeArray<byte>(TileData, TilesCount * (greyScale ? 0x20 : 0x40), name: nameof(TileData));
            }
        }
    }
}