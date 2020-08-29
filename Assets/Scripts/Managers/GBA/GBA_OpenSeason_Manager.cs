using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_OpenSeason_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 50)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(54, 66)
            // Maps?
            //new ValueRange(102, 134)
        ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(50, 53),
            new ValueRange(67, 81),
            new ValueRange(83, 101)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}