using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_Sabrina_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 50)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(51, 53),
            new ValueRange(58, 58),
            new ValueRange(60, 60),
            new ValueRange(63, 63),
            new ValueRange(66, 66),
            new ValueRange(75, 76),
            new ValueRange(85, 85),
            new ValueRange(87, 87),
            new ValueRange(100, 100)
            ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(54, 57),
            new ValueRange(59, 59),
            new ValueRange(61, 62),
            new ValueRange(64, 65),
            new ValueRange(67, 74),
            new ValueRange(77, 84),
            new ValueRange(86, 86),
            new ValueRange(88, 99),
            new ValueRange(101, 112)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}