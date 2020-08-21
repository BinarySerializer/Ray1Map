namespace R1Engine
{
    /// <summary>
    /// Tile map data for sprites on GBA
    /// </summary>
    public class GBA_SpriteTileMap : GBA_BaseBlock
    {
        public bool? IsDataCompressed { get; set; }

        public ushort TileMapLength { get; set; }
        public bool IsCompressed { get; set; }
        public byte Byte_03 { get; set; }

        public byte[] TileMap { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            TileMapLength = s.Serialize<ushort>(TileMapLength, name: nameof(TileMapLength));
            IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));

            if (IsDataCompressed ?? IsCompressed) {
                s.DoEncoded(new GBA_LZSSEncoder(), () => TileMap = s.SerializeArray<byte>(TileMap, TileMapLength * 32, name: nameof(TileMap)));
                s.Align();
            } else {
                TileMap = s.SerializeArray<byte>(TileMap, TileMapLength * 32, name: nameof(TileMap));
            }
        }
    }
}