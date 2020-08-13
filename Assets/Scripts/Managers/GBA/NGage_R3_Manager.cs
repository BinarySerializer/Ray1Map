using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class NGage_R3_Manager : GBA_R3_Manager
    {
        public override string GetROMFilePath => $"rayman3.dat";

        public override int DLCLevelCount => 0;

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        public override GBA_Data LoadLevelBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath, context);

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