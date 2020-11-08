using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro2_Manager : GBAIsometric_Spyro_Manager
    {
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, ValueRange.EnumerateRanges(new ValueRange(0, 34), new ValueRange(39, 42)).ToArray()), // 3D
            new GameInfo_World(1, ValueRange.EnumerateRanges(new ValueRange(0, 3)).ToArray()), // Agent 9
        });

        public override int DataTableCount => 1509;
        public override int PortraitsCount => throw new NotImplementedException();
        public override int DialogCount => throw new NotImplementedException();
        public override int PrimaryLevelCount => throw new NotImplementedException();
        public override int LevelMapsCount => throw new NotImplementedException();
        public override int TotalLevelsCount => throw new NotImplementedException();
        public override int ObjectTypesCount => throw new NotImplementedException();
        public override int AnimSetsCount => 162;
        public override int LevelDataCount => 39;
        public override int MenuPageCount => throw new NotImplementedException();

        public override IEnumerable<string> GetLanguages
        {
            get
            {
                yield return "English";
            }
        }
    }
}