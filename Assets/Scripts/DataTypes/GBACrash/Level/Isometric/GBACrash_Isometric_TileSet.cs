namespace R1Engine
{
    public class GBACrash_Isometric_TileSet : R1Serializable
    {
        public ushort TileSetCount_Total { get; set; } // Set before serializing
        public ushort TileSetCount_4bpp { get; set; } // Set before serializing

        public byte[] TileSet_4bpp { get; set; }
        public byte[] TileSet_8bpp { get; set; }

        public ushort[] TileSet_4bpp_Attributes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSet_4bpp = s.SerializeArray<byte>(TileSet_4bpp, TileSetCount_4bpp * 0x20, name: nameof(TileSet_4bpp));
            TileSet_8bpp = s.SerializeArray<byte>(TileSet_8bpp, (TileSetCount_Total - TileSetCount_4bpp) * 0x40, name: nameof(TileSet_8bpp));
            TileSet_4bpp_Attributes = s.SerializeArray<ushort>(TileSet_4bpp_Attributes, TileSetCount_4bpp, name: nameof(TileSet_4bpp_Attributes));
            // TODO: More data?
        }
    }
}