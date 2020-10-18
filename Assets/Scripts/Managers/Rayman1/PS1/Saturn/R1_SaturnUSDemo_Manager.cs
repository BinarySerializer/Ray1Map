namespace R1Engine
{
    public class R1_SaturnUSDemo_Manager : R1_Saturn_Manager
    {
        public override uint GetPalOffset => 0x78E48;
        public override uint GetFndFileTableOffset => 0x8156C;
        public override uint GetFndSPFileTableOffset => 0x81963;
        public override uint GetFndIndexTableOffset => 0x8189B;

        public override uint? TypeZDCOffset => 0x7EC56;
        public override uint? ZDCDataOffset => 0x7DC56;
        public override uint? EventFlagsOffset => 0x7D454;
    }
}