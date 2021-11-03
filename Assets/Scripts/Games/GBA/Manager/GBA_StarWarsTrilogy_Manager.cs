using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Ray1Map.GBA
{
    public class GBA_StarWarsTrilogy_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 90)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(new ValueRange(120, 120), new ValueRange(122, 123), new ValueRange(126, 126), new ValueRange(128, 129), new ValueRange(134, 135), new ValueRange(137, 138)).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(new ValueRange(90, 118), new ValueRange(121, 121), new ValueRange(124, 125), new ValueRange(127, 127), new ValueRange(130, 133), new ValueRange(136, 136), new ValueRange(139, 140)).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}