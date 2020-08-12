namespace R1Engine
{
    /// <summary>
    /// Tile map data for sprites on GBA
    /// </summary>
    public class GBA_SpriteTileMap : GBA_BaseBlock
    {
        public ushort TileMapLength { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }

        public byte[] TileMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileMapLength = s.Serialize<ushort>(TileMapLength, name: nameof(TileMapLength));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            TileMap = s.SerializeArray<byte>(TileMap, TileMapLength * 32, name: nameof(TileMap));
        }
    }
}