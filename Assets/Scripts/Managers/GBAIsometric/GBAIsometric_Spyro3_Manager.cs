namespace R1Engine
{
    public class GBAIsometric_Spyro3_Manager : GBAIsometric_Spyro_Manager
    {
        protected override int LevelCount => 84; // Is the count correct?

        public override int DataTableCount => 2180;
        public override int LevelInfoCount => 80;
        public override int LevelInfo2DCount => 4;
    }
}