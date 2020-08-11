namespace R1Engine
{
    /// <summary>
    /// A map block for GBA
    /// </summary>
    public class GBA_TileMap : GBA_BaseBlock {
        public ushort TileMapSize { get; set; }
        public ushort BGMapSize { get; set; }
        public byte[] UnkData { get; set; }
        public byte TilePaletteIndex { get; set; } // Not used. Always 0 in R3GBA, but not in N-Gage (but tile block is always offset 0).
        public sbyte UnknownIndex { get; set; }

        public byte[] TileMap4bpp { get; set; }
        public byte[] TileMap8bpp { get; set; }

		#region Parsed
        public GBA_Palette TilePalette { get; set; }
        #endregion

        public override void SerializeBlock(SerializerObject s) {
            TileMapSize = s.Serialize<ushort>(TileMapSize, name: nameof(TileMapSize));
            BGMapSize = s.Serialize<ushort>(BGMapSize, name: nameof(BGMapSize));
            TilePaletteIndex = s.Serialize<byte>(TilePaletteIndex, name: nameof(TilePaletteIndex));
            UnknownIndex = s.Serialize<sbyte>(UnknownIndex, name: nameof(UnknownIndex)); // Can be 0xFF which means this block doesn't exist
            UnkData = s.SerializeArray<byte>(UnkData, 6, name: nameof(UnkData));

            // Serialize tilemap data
            TileMap4bpp = s.SerializeArray<byte>(TileMap4bpp, TileMapSize * 0x20, name: nameof(TileMap4bpp));
            TileMap8bpp = s.SerializeArray<byte>(TileMap8bpp, BGMapSize * 0x40, name: nameof(TileMap8bpp));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            // Serialize tile palette
            TilePalette = s.DoAt(OffsetTable.GetPointer(0, true), () => s.SerializeObject<GBA_Palette>(TilePalette, name: nameof(TilePalette)));
        }
    }
}