namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_PAL : R1Serializable
    {
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, s.CurrentLength / 2, name: nameof(Palette));
        }
    }
}