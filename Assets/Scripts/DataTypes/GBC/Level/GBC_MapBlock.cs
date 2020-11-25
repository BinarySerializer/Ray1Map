namespace R1Engine
{
    public class GBC_MapBlock : GBC_BaseBlock 
    {
        public byte Width { get; set; }
        public byte Height { get; set; }
        public MapTile.GBC_TileType Type { get; set; }
        public MapTile[] MapTiles { get; set; }


        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);
            MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, Width * Height, onPreSerialize: t => t.GBCTileType = Type, name: nameof(MapTiles));

        }
    }
}