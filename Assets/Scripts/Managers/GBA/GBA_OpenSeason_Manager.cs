using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_OpenSeason_Manager : GBA_Manager
    {
        // TODO: Get count
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 50)
        };

        // TODO: Get values
        public override int[] MenuLevels => new int[0];
        public override int DLCLevelCount => 0;

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}