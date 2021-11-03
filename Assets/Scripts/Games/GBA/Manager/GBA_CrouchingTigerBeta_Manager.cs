using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_CrouchingTigerBeta_Manager : GBA_CrouchingTiger_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 40),
            Enumerable.Range(118, 1)
        };
    }
}