using System;

namespace R1Engine
{
    /// <summary>
    /// A map block for Rayman 3 (GBA)
    /// </summary>
    public class GBA_R3_TileMap : GBA_R3_BaseBlock {
        public ushort TileMapSize { get; set; }
        public ushort BGMapSize { get; set; }
        public byte[] UnkData { get; set; }
        public byte TilePaletteIndex { get; set; }
        public byte UnknownIndex { get; set; }

        public byte[] TileMap4bpp { get; set; }
        public byte[] TileMap8bpp { get; set; }

		#region Parsed
		public GBA_R3_OffsetTable OffsetTable { get; set; }
        public GBA_R3_Palette TilePalette { get; set; }
        #endregion

        public override void SerializeImpl(SerializerObject s) {
            // Serialize block header
            base.SerializeImpl(s);

            TileMapSize = s.Serialize<ushort>(TileMapSize, name: nameof(TileMapSize));
            BGMapSize = s.Serialize<ushort>(BGMapSize, name: nameof(BGMapSize));
            TilePaletteIndex = s.Serialize<byte>(TilePaletteIndex, name: nameof(TilePaletteIndex));
            UnknownIndex = s.Serialize<byte>(UnknownIndex, name: nameof(UnknownIndex)); // Can be 0xFF which means this block doesn't exist
            UnkData = s.SerializeArray<byte>(UnkData, 6, name: nameof(UnkData));

            // Serialize tilemap data
            TileMap4bpp = s.SerializeArray<byte>(TileMap4bpp, TileMapSize * 0x20, name: nameof(TileMap4bpp));
            TileMap8bpp = s.SerializeArray<byte>(TileMap8bpp, BGMapSize * 0x40, name: nameof(TileMap8bpp));

            s.Align();

            // Serialize offset table
            OffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(OffsetTable, name: nameof(OffsetTable));

            // Serialize tile palette
            TilePalette = s.DoAt(OffsetTable.GetPointer(TilePaletteIndex, true), () => s.SerializeObject<GBA_R3_Palette>(TilePalette, name: nameof(TilePalette)));
        }

    }
}