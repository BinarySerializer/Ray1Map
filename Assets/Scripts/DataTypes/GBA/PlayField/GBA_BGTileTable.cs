namespace R1Engine
{
    public class GBA_BGTileTable : GBA_BaseBlock
    {
        public ushort IndicesCount8bpp { get; set; }
        public ushort IndicesCount4bpp { get; set; }
        public ushort[] Indices8bpp { get; set; }
        public ushort[] Indices4bpp { get; set; }

        // PoP
        public bool HasExtraData { get; set; }
        public byte[] ExtraData { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            IndicesCount8bpp = s.Serialize<ushort>(IndicesCount8bpp, name: nameof(IndicesCount8bpp));
            IndicesCount4bpp = s.Serialize<ushort>(IndicesCount4bpp, name: nameof(IndicesCount4bpp));
            if (HasExtraData) {
                ExtraData = s.SerializeArray<byte>(ExtraData, 4, name: nameof(ExtraData));
            }
            Indices8bpp = s.SerializeArray<ushort>(Indices8bpp, IndicesCount8bpp, name: nameof(Indices8bpp));
            Indices4bpp = s.SerializeArray<ushort>(Indices4bpp, IndicesCount4bpp, name: nameof(Indices4bpp));
        }
    }
}