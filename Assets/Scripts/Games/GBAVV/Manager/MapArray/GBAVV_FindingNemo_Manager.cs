namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_FindingNemo_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 24;
    }
    public class GBAVV_FindingNemoEUUS_Manager : GBAVV_FindingNemo_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0803BABC,
            0x08042958,
            0x080437B0,
            0x08044B00,
            0x08044E4C,
            0x08044EE8,
            0x0804B1A4,
            0x0804B3F0,
            0x0804DEF8,
            0x08051380,
            0x080517D8,
            0x08056D34,
            0x08057EEC,
            0x0805E354,
            0x0805E854,
        };
    }
    public class GBAVV_FindingNemoJP_Manager : GBAVV_FindingNemo_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0805678C,
            0x0805D628,
            0x0805E480,
            0x0805F7D0,
            0x0805FB1C,
            0x0805FBB8,
            0x08065E74,
            0x080660C0,
            0x08068BC8,
            0x0806C050,
            0x0806C4A8,
            0x08071A04,
            0x08078700,
            0x08078C00,
        };
    }
}