using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_SplinterCellPandoraTomorrow_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 171)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(177, 177), 
            new ValueRange(179, 179),
            new ValueRange(182, 182),
            new ValueRange(184, 184),
            new ValueRange(186, 186),
            new ValueRange(189, 189),
            new ValueRange(192, 194),
            new ValueRange(196, 196),
            new ValueRange(199, 200),
            new ValueRange(203, 203)
            ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(171, 176), 
            new ValueRange(178, 178),
            new ValueRange(180, 181),
            new ValueRange(183, 183),
            new ValueRange(185, 185),
            new ValueRange(187, 188),
            new ValueRange(190, 191),
            new ValueRange(195, 195),
            new ValueRange(197, 198),
            new ValueRange(201, 202)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}