using BinarySerializer.Klonoa.DTP;
using System.Collections.Generic;

namespace Ray1Map.PSKlonoa
{
    public class PSKlonoa_DTP_Manager_Prototype_19970717 : PSKlonoa_DTP_BaseManager
    {
        public override (string Name, int SectorsCount)[] Levels => new (string Name, int SectorsCount)[]
        {
            ("FIX", 0),
            ("MENU", 0),
            ("INTRO", 0),

            ("Vision 1-1", 3),
            ("Vision 1-2", 5),
            ("Rongo Lango", 2),

            ("Vision 2-1", 4),
            ("Vision 2-2", 6),
            ("Pamela", 2),

            ("Vision 3-1", 5),
            ("Vision 3-2", 10),
            ("Gelg Bolm", 1),

            ("Vision 4-1", 5), // Different
            ("Vision 4-2", 8),
            ("Baladium", 2),

            ("Vision 5-1", 7),
            ("Vision 5-2", 9),
            ("Joka", 1),

            ("Vision 6-1", 8),
            ("Vision 6-2", 8),
            ("Ghadius", 2),

            // Order is different here than in-game
            ("Ending", 1), // Different
            ("Final Vision", 3),
            ("Nahatomb", 3),

            ("Extra Vision", 9),
        };

        public override KlonoaSettings_DTP GetKlonoaSettings(GameSettings settings) => new KlonoaSettings_DTP_Prototype_19970717();
        public override Dictionary<string, char> GetCutsceneTranslationTable => new Dictionary<string, char>();
    }
}