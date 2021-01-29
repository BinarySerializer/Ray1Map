namespace R1Engine
{
    public class GBAVV_Map2D_ObjPal : R1Serializable
    {
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 16, name: nameof(Palette));
        }
    }
}