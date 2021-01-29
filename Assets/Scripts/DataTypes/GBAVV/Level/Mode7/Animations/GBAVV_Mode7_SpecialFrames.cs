namespace R1Engine
{
    public class GBAVV_Mode7_SpecialFrames : R1Serializable
    {
        public byte FramesCount { get; set; } // Set before serializing

        public RGBA5551Color[] Palette { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public Frame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Frames = s.SerializeObjectArray<Frame>(Frames, FramesCount, x => x.TileMapLength = Width * Height, name: nameof(Frames));
        }

        public class Frame : R1Serializable
        {
            public int TileMapLength { get; set; } // Set before serializing

            public uint TileSetLength { get; set; }
            public MapTile[] TileMap { get; set; }
            public byte[] TileSet { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TileSetLength = s.Serialize<uint>(TileSetLength, name: nameof(TileSetLength));
                TileMap = s.SerializeObjectArray<MapTile>(TileMap, TileMapLength, name: nameof(TileMap));
                TileSet = s.SerializeArray<byte>(TileSet, TileSetLength * 0x20, name: nameof(TileSet));
            }
        }
    }
}