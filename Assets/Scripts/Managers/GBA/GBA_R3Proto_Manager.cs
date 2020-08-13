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

        // TODO: Get values
        public override int[] MenuLevels => new int[0];

        public override int DLCLevelCount => 0;
    }
}