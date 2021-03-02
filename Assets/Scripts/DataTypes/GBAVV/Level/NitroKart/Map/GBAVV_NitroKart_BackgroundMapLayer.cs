namespace R1Engine
{
    public class GBAVV_NitroKart_BackgroundMapLayer : R1Serializable
    {
        public Pointer TileMapPointer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_BackgroundTileMap TileMap { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileMapPointer = s.SerializePointer(TileMapPointer, name: nameof(TileMapPointer));
            Width = s.Serialize<int>(Width, name: nameof(Width));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
            Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));

            TileMap = s.DoAt(TileMapPointer, () => s.SerializeObject<GBAVV_NitroKart_BackgroundTileMap>(TileMap, name: nameof(TileMap)));
        }
    }
}