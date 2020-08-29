using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace R1Engine
{
    public class GBA_StarWarsEpisodeIII_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 142)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(153, 157), 
            new ValueRange(160, 160), 
            new ValueRange(163, 164),
            new ValueRange(169, 169),
            new ValueRange(172, 172),
            new ValueRange(182, 182),
            new ValueRange(184, 184),
            new ValueRange(186, 186),
            new ValueRange(188, 188),
            new ValueRange(190, 191),
            new ValueRange(193, 221)
            ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(142, 152), 
            new ValueRange(158, 159), 
            new ValueRange(161, 162), 
            new ValueRange(165, 168),
            new ValueRange(170, 171), 
            new ValueRange(173, 181), 
            new ValueRange(183, 183),
            new ValueRange(185, 185),
            new ValueRange(187, 187),
            new ValueRange(189, 189),
            new ValueRange(192, 192)
            ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();
    }
}