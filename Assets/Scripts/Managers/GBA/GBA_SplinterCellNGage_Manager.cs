using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;


namespace R1Engine
{
    public class GBA_SplinterCellNGage_Manager : GBA_Manager
    {
        public override string GetROMFilePath(Context context) => $"splintercell_1.dat";

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 185)
        };

        public override int[] MenuLevels => ValueRange.EnumerateRanges(
            new ValueRange(206, 206),
            new ValueRange(222, 226)
        ).ToArray();
        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => ValueRange.EnumerateRanges(
            new ValueRange(205, 205),
            new ValueRange(207, 215),
            new ValueRange(217, 221)
        ).ToArray();
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath(context), context);
        public override GBA_LocLanguageTable LoadLocalizationTable(Context context) => null;

        public override async UniTask LoadFilesAsync(Context context) => await context.AddLinearFileAsync(GetROMFilePath(context));
    }
}