namespace R1Engine
{
    public abstract class GBAVV_BrotherBear_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 55;
    }
    public class GBAVV_BrotherBearEU_Manager : GBAVV_BrotherBear_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x080800EC,
            0x08085BFC,
            0x08087244,
            0x080899FC,
            0x0808C7C8,
            0x08097990,
            0x080990C0,
            0x080993EC,
            0x0809C1E4,
            0x080A16F4,
            0x080A5D04,
        };
    }
    public class GBAVV_BrotherBearUS_Manager : GBAVV_BrotherBear_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0808019C,
            0x08085CAC,
            0x080872F4,
            0x08089AAC,
            0x0808C878,
            0x08097A40,
            0x08099170,
            0x0809949C,
            0x0809C294,
            0x080A17A4,
            0x080A5DB4,
        };
    }
}