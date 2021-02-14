using System.Collections.Generic;

namespace R1Engine
{
    public class GBAIsometric_Tron2_Manager : GBAIsometric_Spyro_Manager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, new int[]
            {
                1,3,7,9,10,14,20,22,23,24,26,28,33,36,37,39,40,46,47,49
            })
        });

        public override int DataTableCount => 2148;
        public override int PortraitsCount => 57 - 1;
        public override int DialogCount => 0;
        public override int PrimaryLevelCount => 0;
        public override int LevelMapsCount => 0;
        public override int TotalLevelsCount => 0;
        public override int ObjectTypesCount => 352;
        public override int AnimSetsCount => 128;
        public override int LevelDataCount => 20;
        public override int MenuPageCount => 0;
        public override IEnumerable<string> GetLanguages => new string[0];
    }
}