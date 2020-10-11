using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBA_R3Digiblast_Manager : GBA_R3_Manager
    {
        public override string GetROMFilePath => $"Rayman";

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 8), // World 1
            Enumerable.Range(8, 9), // World 2
            Enumerable.Range(17, 13), // World 3
            Enumerable.Range(30, 10), // World 4
            Enumerable.Range(40, 8), // Bonus
            Enumerable.Range(48, 6), // Ly
            Enumerable.Range(54, 5), // World
            Enumerable.Range(59, 6), // Multiplayer
        };

        public override int[] MenuLevels => new int[]
        {
            91,
            117
        };

        public override int[] AdditionalSprites4bpp => Enumerable.Range(70, 91 - 70).Concat(Enumerable.Range(92, 117 - 92)).Concat(Enumerable.Range(119, 126 - 119)).ToArray();
        public override int[] AdditionalSprites8bpp => new int[]
        {
            118
        };

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath, context);
        public override GBA_LocLanguageTable LoadLocalization(Context context) => null;

        public override async UniTask LoadFilesAsync(Context context) => await context.AddLinearSerializedFileAsync(GetROMFilePath);
    }
}