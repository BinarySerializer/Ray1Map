namespace R1Engine
{
    public class R1_SaturnJP_Manager : R1_Saturn_Manager
    {
        public override uint GetPalOffset => 0x791C4;

        public override uint? TypeZDCOffset => 0x7EFD2;
        public override uint? ZDCDataOffset => 0x7DFD2;
        public override uint? EventFlagsOffset => 0x7D7D0;
    }
}