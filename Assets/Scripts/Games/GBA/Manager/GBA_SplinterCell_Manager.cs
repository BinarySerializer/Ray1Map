using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Ray1Map.GBA
{
    public class GBA_SplinterCell_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 165)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(182, 182),
            new ValueRange(192, 193),
            new ValueRange(195, 195),
            new ValueRange(198, 199),
            new ValueRange(202, 202)
            ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(183, 191),
            new ValueRange(194, 194),
            new ValueRange(196, 197),
            new ValueRange(200, 201)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}