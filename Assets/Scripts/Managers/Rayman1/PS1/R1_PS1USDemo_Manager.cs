using System.Collections.Generic;

namespace R1Engine
{
    public class R1_PS1USDemo_Manager : R1_PS1_Manager
    {
        public override string GetExeFilePath => "SLUS-900.01";

        protected override Dictionary<string, PS1FileInfo> GetFileInfo(GameSettings settings) => PS1FileInfo.fileInfoUS;

        public override uint? TypeZDCOffset => 0x9BDF8;
        public override uint? ZDCDataOffset => 0x9ADF8;
        public override uint? EventFlagsOffset => 0x9A5F8;
    }
}