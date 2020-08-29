using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_PoPSoT_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 149)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(154, 154),
            new ValueRange(166, 166),
            new ValueRange(169, 171),
            new ValueRange(176, 177)
            // Might be maps, but have a different structure if so
            //new ValueRange(181, 200)
            ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(150, 153),
            new ValueRange(155, 164),
            new ValueRange(167, 168),
            new ValueRange(178, 180),
            new ValueRange(201, 201)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}