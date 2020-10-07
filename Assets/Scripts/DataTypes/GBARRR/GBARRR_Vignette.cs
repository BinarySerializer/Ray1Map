namespace R1Engine
{
    public class GBARRR_Vignette : R1Serializable
    {
        public ARGB1555Color[] Palette { get; set; }
        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, 0x100, name: nameof(Palette));
            ImgData = s.SerializeArray<byte>(ImgData, 240 * 160, name: nameof(ImgData));
        }
    }
}