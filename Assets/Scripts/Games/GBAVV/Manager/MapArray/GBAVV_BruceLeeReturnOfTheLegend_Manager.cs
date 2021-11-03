namespace Ray1Map.GBAVV
{
    public abstract class GBAVV_BruceLeeReturnOfTheLegend_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 36;
    }
    public class GBAVV_BruceLeeReturnOfTheLegendEU_Manager : GBAVV_BruceLeeReturnOfTheLegend_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0803DA60,
            0x08049B78,
            0x0806D56C,
        };
    }
    public class GBAVV_BruceLeeReturnOfTheLegendUS_Manager : GBAVV_BruceLeeReturnOfTheLegend_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0802EE84,
            0x0803AF9C,
            0x0805E990,
        };
    }
}