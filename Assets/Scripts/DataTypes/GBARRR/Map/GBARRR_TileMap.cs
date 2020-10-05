namespace R1Engine
{
    public class GBARRR_TileMap : R1Serializable
    {
        public ARGB1555Color[] Palette { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, 0x100, name: nameof(Palette));
            Data = s.SerializeArray<byte>(Data, 0x40040, name: nameof(Data));
        }
    }
}