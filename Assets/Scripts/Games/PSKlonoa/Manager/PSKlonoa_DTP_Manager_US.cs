using BinarySerializer.Klonoa.DTP;
using System.Collections.Generic;

namespace Ray1Map.PSKlonoa
{
    public class PSKlonoa_DTP_Manager_US : PSKlonoa_DTP_BaseManager
    {
        public override KlonoaSettings_DTP_PS1 GetKlonoaSettings(GameSettings settings) => new KlonoaSettings_DTP_US();
        public override Dictionary<string, char> GetCutsceneTranslationTable => CutsceneTextTranslationTables.US;
    }
}