using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_R3Proto_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 8), // World 1
            Enumerable.Range(8, 9), // World 2
            Enumerable.Range(17, 13), // World 3
            Enumerable.Range(30, 10), // World 4
            Enumerable.Range(40, 7), // Bonus
            Enumerable.Range(47, 6), // Ly
            Enumerable.Range(53, 5), // World
            Enumerable.Range(58, 6), // Multiplayer
        };

        public override int[] MenuLevels => new int[]
        {
            // Menu
            89,

            // Ubisoft logo
            113,

            // Scrolling vignette
            121,
            122
        };
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => Enumerable.Range(69, 89 - 69).Concat(Enumerable.Range(90, 113 - 90)).Concat(Enumerable.Range(115, 121 - 115)).ToArray();
        public override int[] AdditionalSprites8bpp => new int[]
        {
            114
        };

        public override string[] Languages => new string[]
        {
            "English",
            "French",
            "Spanish",
            "German",
            "Italian",
            "Dutch",
        };
    }
}