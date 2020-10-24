namespace R1Engine
{
    public class R1_SaturnJP_Manager : R1_Saturn_Manager
    {
        public override uint GetPalOffset => 0x791C4;
        public override uint GetFndFileTableOffset => 0x818E8;
        public override uint GetFndSPFileTableOffset => 0x81CDF;
        public override uint GetFndIndexTableOffset => 0x81C17;

        public override uint? TypeZDCOffset => 0x7EFD2;
        public override uint? ZDCDataOffset => 0x7DFD2;
        public override uint? EventFlagsOffset => 0x7D7D0;
        public override uint? WorldInfoOffset => 0x7F8A0;
    }
}