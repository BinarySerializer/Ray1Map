using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_TMNT_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 39)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(new ValueRange(46, 50), new ValueRange(59, 68)).ToArray();

        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(new ValueRange(39, 45), new ValueRange(58, 58), new ValueRange(69, 77)).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}