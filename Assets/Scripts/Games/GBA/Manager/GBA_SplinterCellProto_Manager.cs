using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Ray1Map.GBA
{
    public class GBA_SplinterCellProto_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 162)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(176, 176),
            new ValueRange(186, 186),
            new ValueRange(188, 188),
            new ValueRange(191, 192)
        ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(177, 185),
            new ValueRange(187, 187),
            new ValueRange(189, 190),
            new ValueRange(193, 194)
        ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}