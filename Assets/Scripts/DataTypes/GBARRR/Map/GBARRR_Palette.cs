namespace R1Engine
{
    public class GBARRR_Palette : R1Serializable
    {
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, 0x100, name: nameof(Palette));
        }
    }
}