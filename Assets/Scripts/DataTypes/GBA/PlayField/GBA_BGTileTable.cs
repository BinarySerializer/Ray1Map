namespace R1Engine
{
    public class GBA_BGTileTable : GBA_BaseBlock
    {
        public ushort IndicesCount8bpp { get; set; }
        public ushort IndicesCount4bpp { get; set; }
        public ushort[] Indices8bpp { get; set; }
        public ushort[] Indices4bpp { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            IndicesCount8bpp = s.Serialize<ushort>(IndicesCount8bpp, name: nameof(IndicesCount8bpp));
            IndicesCount4bpp = s.Serialize<ushort>(IndicesCount4bpp, name: nameof(IndicesCount4bpp));
            Indices8bpp = s.SerializeArray<ushort>(Indices8bpp, IndicesCount8bpp, name: nameof(Indices8bpp));
            Indices4bpp = s.SerializeArray<ushort>(Indices4bpp, IndicesCount4bpp, name: nameof(Indices4bpp));
        }
    }
}