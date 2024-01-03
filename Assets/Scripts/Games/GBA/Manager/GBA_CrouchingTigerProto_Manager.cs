using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.GBA
{
    public class GBA_CrouchingTigerProto_Manager : GBA_CrouchingTiger_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 1)
        };
    }
}