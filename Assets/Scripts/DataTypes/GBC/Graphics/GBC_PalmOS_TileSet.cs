namespace R1Engine
{
    public class GBC_PalmOS_TileSet : GBC_PalmOS_Block 
    {
        public uint Uint_00 { get; set; } // Specified if it's 4bpp or 8bpp?
        public uint TilesCount { get; set; }
        public byte[] TileData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
            TilesCount = s.Serialize<uint>(TilesCount, name: nameof(TilesCount));
            TileData = s.SerializeArray<byte>(TileData, TilesCount * 0x40, name: nameof(TileData));
        }
    }
}