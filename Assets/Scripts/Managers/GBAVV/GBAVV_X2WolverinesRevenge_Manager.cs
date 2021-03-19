namespace R1Engine
{
    public class GBAVV_X2WolverinesRevenge_Manager : GBAVV_MapArray_BaseManager
    {
        public override int LevelsCount => 40;

        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x080446BC,
            0x0805CF40,
            0x0805E76C,
            0x0805FD88,
            0x0806DD90,
            0x08081FA8,
        };
    }
}