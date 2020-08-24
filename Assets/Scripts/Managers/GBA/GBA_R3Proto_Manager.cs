using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBA_R3Proto_Manager : GBA_R3_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 8),
            Enumerable.Range(8, 9),
            Enumerable.Range(17, 13),
            Enumerable.Range(30, 10),
            Enumerable.Range(40, 13),
            Enumerable.Range(53, 5),
            Enumerable.Range(58, 6),
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
    }
}