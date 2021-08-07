using System.Collections.Generic;
using BinarySerializer.KlonoaDTP;

namespace R1Engine
{
    public class PSKlonoa_DTP_Manager_US : PSKlonoa_DTP_BaseManager
    {
        public override LoaderConfiguration GetLoaderConfig(GameSettings settings) => new LoaderConfiguration_DTP_US();
        public override Dictionary<string, char> GetCutsceneTranslationTable => CutsceneTextTranslationTables.US;
    }
}