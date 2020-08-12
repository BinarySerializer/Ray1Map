namespace R1Engine
{
    /// <summary>
    /// Tile map data for sprites on GBA
    /// </summary>
    public class GBA_SpriteTileMap : GBA_BaseBlock
    {
        public uint TileMapLength { get; set; }
        public byte[] TileMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileMapLength = s.Serialize<uint>(TileMapLength, name: nameof(TileMapLength));
            TileMap = s.SerializeArray<byte>(TileMap, TileMapLength * 32, name: nameof(TileMap));
        }
    }
}