using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Ray1Map.GBA
{
    public class GBA_BatmanRiseOfSinTzu_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 76)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(99, 103),
            new ValueRange(105, 105)
        ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(93, 98),
            new ValueRange(104, 104),
            new ValueRange(106, 108)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}