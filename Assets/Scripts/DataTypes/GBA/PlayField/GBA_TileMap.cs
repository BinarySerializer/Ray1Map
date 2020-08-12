namespace R1Engine
{
    /// <summary>
    /// A map block for GBA
    /// </summary>
    public class GBA_TileMap : GBA_BaseBlock {
        public ushort TileMap4bppSize { get; set; }
        public ushort TileMap8bppSize { get; set; }
        public byte[] UnkData { get; set; }
        public byte TilePaletteIndex { get; set; } // Not used. Always 0 in R3GBA, but not in N-Gage (but tile block is always offset 0).
        public byte UnknownIndex { get; set; }

        public byte[] TileMap4bpp { get; set; }
        public byte[] TileMap8bpp { get; set; }

        // Batman
        public bool Is8bpp { get; set; }
        public bool IsCompressed { get; set; }

		#region Parsed
        public GBA_Palette TilePalette { get; set; }
        #endregion

        public override void SerializeImpl(SerializerObject s) {
            if (s.GameSettings.EngineVersion == EngineVersion.BatmanVengeanceGBA) {
                Is8bpp = s.Serialize<bool>(Is8bpp, name: nameof(Is8bpp));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                if (Is8bpp) {
                    TileMap8bppSize = s.Serialize<ushort>(TileMap8bppSize, name: nameof(TileMap8bppSize));
                } else {
                    TileMap4bppSize = s.Serialize<ushort>(TileMap4bppSize, name: nameof(TileMap4bppSize));
                }
            } else {
                TileMap4bppSize = s.Serialize<ushort>(TileMap4bppSize, name: nameof(TileMap4bppSize));
                TileMap8bppSize = s.Serialize<ushort>(TileMap8bppSize, name: nameof(TileMap8bppSize));
                TilePaletteIndex = s.Serialize<byte>(TilePaletteIndex, name: nameof(TilePaletteIndex));
                UnknownIndex = s.Serialize<byte>(UnknownIndex, name: nameof(UnknownIndex)); // Can be 0xFF which means this block doesn't exist
                UnkData = s.SerializeArray<byte>(UnkData, 6, name: nameof(UnkData));
            }

            // Serialize tilemap data
            if (IsCompressed) {
                s.DoEncoded(new LZSSEncoder(), () => {
                    TileMap4bpp = s.SerializeArray<byte>(TileMap4bpp, TileMap4bppSize * 0x20, name: nameof(TileMap4bpp));
                    TileMap8bpp = s.SerializeArray<byte>(TileMap8bpp, TileMap8bppSize * 0x40, name: nameof(TileMap8bpp));
                });
                s.Align();
            } else {
                TileMap4bpp = s.SerializeArray<byte>(TileMap4bpp, TileMap4bppSize * 0x20, name: nameof(TileMap4bpp));
                TileMap8bpp = s.SerializeArray<byte>(TileMap8bpp, TileMap8bppSize * 0x40, name: nameof(TileMap8bpp));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion != EngineVersion.BatmanVengeanceGBA) {
                // Serialize tile palette
                TilePalette = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Palette>(TilePalette, name: nameof(TilePalette)));
            }
        }
    }
}