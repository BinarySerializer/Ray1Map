using System;
using System.Collections.Generic;

namespace R1Engine
{
    public class GBAIsometric_Spyro1_Manager : GBAIsometric_Spyro_Manager
    {
        public override int DataTableCount => 83;
        public override int PortraitsCount => throw new NotImplementedException();
        public override int DialogCount => throw new NotImplementedException();
        public override int PrimaryLevelCount => throw new NotImplementedException();
        public override int LevelMapsCount => throw new NotImplementedException();
        public override int TotalLevelsCount => throw new NotImplementedException();
        public override int ObjectTypesCount => throw new NotImplementedException();
        public override int AnimSetsCount => throw new NotImplementedException();

        public override IEnumerable<string> GetLanguages
        {
            get
            {
                yield return "English";
            }
        }

        public override LevelInfo[] LevelInfos => new LevelInfo[0];
    }
}