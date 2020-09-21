namespace R1Engine
{
    public class R1_SaturnProto_Manager : R1_Saturn_Manager
    {
        public override uint GetPalOffset => 0x87754;

        public override uint? TypeZDCOffset => 0x832B2;
        public override uint? ZDCDataOffset => 0x822B2;
        public override uint? EventFlagsOffset => 0x81AB0;
    }
}