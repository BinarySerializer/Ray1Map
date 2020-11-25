namespace R1Engine
{
    public class GBC_Map : GBC_BaseBlock 
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public MapTile[] MapTiles { get; set; }

        // Parsed from offset table
        public GBC_TileKit TileKit { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, Width * Height, name: nameof(MapTiles));

            TileKit = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_TileKit>(TileKit, name: nameof(TileKit)));

        }
    }
}