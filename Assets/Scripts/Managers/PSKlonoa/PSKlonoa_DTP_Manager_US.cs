using BinarySerializer.Klonoa.DTP;
using System.Collections.Generic;

namespace R1Engine
{
    public class PSKlonoa_DTP_Manager_US : PSKlonoa_DTP_BaseManager
    {
        public override KlonoaSettings_DTP GetKlonoaSettings(GameSettings settings) => new KlonoaSettings_DTP_US();
        public override Dictionary<string, char> GetCutsceneTranslationTable => CutsceneTextTranslationTables.US;
    }
}