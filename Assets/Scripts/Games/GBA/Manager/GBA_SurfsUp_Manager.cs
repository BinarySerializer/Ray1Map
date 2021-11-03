using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Ray1Map.GBA
{
    public class GBA_SurfsUp_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 248)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(new ValueRange(259, 270), new ValueRange(304, 309)).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(new ValueRange(248, 258), new ValueRange(271, 282), new ValueRange(284, 303), 
            // ?
            new ValueRange(321, 321)).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}