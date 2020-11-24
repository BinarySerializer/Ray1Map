namespace R1Engine
{
    public class GBC_PalmOS_Map : GBC_PalmOS_Block 
    {
        public byte[] Bytes_00 { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public MapTile[] MapTiles { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 12, name: nameof(Bytes_00));
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            MapTiles = s.SerializeObjectArray<MapTile>(MapTiles, Width * Height, name: nameof(MapTiles));
        }
    }
}