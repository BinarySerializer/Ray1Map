using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Ray1Map.GBA
{
    public class GBA_KingKong_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 164)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(172, 172),
            new ValueRange(182, 182),
            new ValueRange(191, 193),
            new ValueRange(197, 212)
            ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(164, 171),
            new ValueRange(173, 181),
            new ValueRange(183, 190),
            new ValueRange(194, 195)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}