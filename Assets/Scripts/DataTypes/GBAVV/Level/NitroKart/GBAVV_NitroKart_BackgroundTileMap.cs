namespace R1Engine
{
    public class GBAVV_NitroKart_BackgroundTileMap : GBAVV_BaseBlock
    {
        public MapTile[] MapTiles { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, BlockLength / 2, name: nameof(MapTiles));
        }
    }
}