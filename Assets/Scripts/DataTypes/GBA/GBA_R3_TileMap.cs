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

        public byte[] TileMapData { get; set; }
        public byte[] BGMapData { get; set; }

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
            TileMapData = s.SerializeArray<byte>(TileMapData, TileMapSize * 0x20, name: nameof(TileMapData));
            BGMapData = s.SerializeArray<byte>(BGMapData, BGMapSize * 0x40, name: nameof(BGMapData));

            s.Align();

            // Serialize offset table
            OffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(OffsetTable, name: nameof(OffsetTable));

            // Serialize tile palette
            TilePalette = s.DoAt(OffsetTable.GetPointer(TilePaletteIndex, true), () => s.SerializeObject<GBA_R3_Palette>(TilePalette, name: nameof(TilePalette)));
        }

    }
}