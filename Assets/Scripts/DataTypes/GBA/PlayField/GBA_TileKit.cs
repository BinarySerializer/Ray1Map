namespace R1Engine
{
    /// <summary>
    /// A map block for GBA
    /// </summary>
    public class GBA_TileKit : GBA_BaseBlock {
        public ushort TileSet4bppSize { get; set; }
        public ushort TileSet8bppSize { get; set; }
        public byte[] UnkData { get; set; }
        public byte TilePaletteIndex { get; set; } // Not used. Always 0 in R3GBA, but not in N-Gage (but tile block is always offset 0).
        public byte UnknownIndex { get; set; }

        public byte[] TileSet4bpp { get; set; }
        public byte[] TileSet8bpp { get; set; }

        // Batman
        public bool Is8bpp { get; set; }
        public bool IsCompressed { get; set; }

		#region Parsed
        public GBA_Palette TilePalette { get; set; }
        #endregion

        public override void SerializeImpl(SerializerObject s) {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Is8bpp = s.Serialize<bool>(Is8bpp, name: nameof(Is8bpp));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                if (Is8bpp) {
                    TileSet8bppSize = s.Serialize<ushort>(TileSet8bppSize, name: nameof(TileSet8bppSize));
                } else {
                    TileSet4bppSize = s.Serialize<ushort>(TileSet4bppSize, name: nameof(TileSet4bppSize));
                }
            } else {
                TileSet4bppSize = s.Serialize<ushort>(TileSet4bppSize, name: nameof(TileSet4bppSize));
                TileSet8bppSize = s.Serialize<ushort>(TileSet8bppSize, name: nameof(TileSet8bppSize));
                TilePaletteIndex = s.Serialize<byte>(TilePaletteIndex, name: nameof(TilePaletteIndex));
                UnknownIndex = s.Serialize<byte>(UnknownIndex, name: nameof(UnknownIndex)); // Can be 0xFF which means this block doesn't exist
                UnkData = s.SerializeArray<byte>(UnkData, 6, name: nameof(UnkData));
            }

            // Serialize tilemap data
            if (IsCompressed) {
                s.DoEncoded(new GBA_LZSSEncoder(), () => {
                    TileSet4bpp = s.SerializeArray<byte>(TileSet4bpp, TileSet4bppSize * 0x20, name: nameof(TileSet4bpp));
                    TileSet8bpp = s.SerializeArray<byte>(TileSet8bpp, TileSet8bppSize * 0x40, name: nameof(TileSet8bpp));
                });
                s.Align();
            } else {
                TileSet4bpp = s.SerializeArray<byte>(TileSet4bpp, TileSet4bppSize * 0x20, name: nameof(TileSet4bpp));
                TileSet8bpp = s.SerializeArray<byte>(TileSet8bpp, TileSet8bppSize * 0x40, name: nameof(TileSet8bpp));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion != EngineVersion.GBA_BatmanVengeance) {
                // Serialize tile palette
                TilePalette = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Palette>(TilePalette, name: nameof(TilePalette)));
            }
        }
    }
}