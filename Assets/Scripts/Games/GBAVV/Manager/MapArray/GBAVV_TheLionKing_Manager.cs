namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_TheLionKing_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 30;
    }
    public class GBAVV_TheLionKingEU_Manager : GBAVV_TheLionKing_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x080457E4,
            0x0806CA64,
            0x080725A4,
            0x08073F04,
            0x08076604,
            0x0807996C,
            0x08083C84,
            0x0808D830,
            0x08092A88,
        };
    }
    public class GBAVV_TheLionKingUS_Manager : GBAVV_TheLionKing_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x080384DC,
            0x0805F75C,
            0x0806529C,
            0x08066B70,
            0x08069270,
            0x0806C5D8,
            0x080768F0,
            0x0808049C,
            0x080856F4,
        };
    }
}