namespace R1Engine
{
    /// <summary>
    /// Tile map data for sprites on GBA
    /// </summary>
    public class GBA_SpriteTileSet : GBA_BaseBlock
    {
        public bool? IsDataCompressed { get; set; }

        public ushort TileSetLength { get; set; }
        public bool IsCompressed { get; set; }
        public byte Byte_03 { get; set; }

        public byte[] TileSet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion <= EngineVersion.GBA_BatmanVengeance) {
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03)); // Indicates if the sprites are 8-bit or not
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                TileSetLength = s.Serialize<ushort>(TileSetLength, name: nameof(TileSetLength));
            } else {
                TileSetLength = s.Serialize<ushort>(TileSetLength, name: nameof(TileSetLength));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            }

            if (IsDataCompressed ?? IsCompressed) {
                s.DoEncoded(new GBA_LZSSEncoder(), () => TileSet = s.SerializeArray<byte>(TileSet, TileSetLength * 32, name: nameof(TileSet)));
                s.Align();
            } else {
                TileSet = s.SerializeArray<byte>(TileSet, TileSetLength * 32, name: nameof(TileSet));
            }
        }
    }
}