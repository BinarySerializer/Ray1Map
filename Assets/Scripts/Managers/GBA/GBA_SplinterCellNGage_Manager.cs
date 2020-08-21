using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBA_SplinterCellNGage_Manager : GBA_SplinterCell_Manager
    {
        public override string GetROMFilePath => $"splintercell_1.dat";

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath, context);
        public override GBA_LocLanguageTable LoadLocalization(Context context) => null;

        public override async UniTask LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new LinearSerializedFile(context)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}