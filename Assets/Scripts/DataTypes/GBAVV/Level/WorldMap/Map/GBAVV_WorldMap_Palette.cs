namespace R1Engine
{
    public class GBAVV_WorldMap_Palette : GBAVV_BaseBlock
    {
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, BlockLength / 2, name: nameof(Palette));
        }
    }
}