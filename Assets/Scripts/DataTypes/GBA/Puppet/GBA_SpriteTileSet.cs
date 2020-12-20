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
        public bool Is8Bit { get; set; } // TODO: Double check this property for all games and use it when parsing the data

        public byte[] TileSet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion <= EngineVersion.GBA_BatmanVengeance) {
                Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                TileSetLength = s.Serialize<ushort>(TileSetLength, name: nameof(TileSetLength));
            } else {
                TileSetLength = s.Serialize<ushort>(TileSetLength, name: nameof(TileSetLength));
                IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
                Is8Bit = s.Serialize<bool>(Is8Bit, name: nameof(Is8Bit));
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