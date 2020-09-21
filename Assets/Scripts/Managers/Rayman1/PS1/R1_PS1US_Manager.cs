using System.Collections.Generic;

namespace R1Engine
{
    public class R1_PS1US_Manager : R1_PS1_Manager
    {
        public override string GetExeFilePath => "SLUS-000.05";

        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => PS1FileInfo.fileInfoUS;

        public override uint? TypeZDCOffset => 0x9E294;
        public override uint? ZDCDataOffset => 0x9D294;
        public override uint? EventFlagsOffset => 0x9CA94;
    }
}