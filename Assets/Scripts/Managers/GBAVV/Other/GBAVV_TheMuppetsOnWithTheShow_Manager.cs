using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBAVV_TheMuppetsOnWithTheShow_Manager : GBAVV_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 0).ToArray()),
        });

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_Default>(GetROMFilePath, context);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Graphics
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0801B9C4,
            0x08024A7C,
            0x08034B48,
            0x0803D094,
            0x08046C18,
            0x08057114,
            0x0805D9E4,
            0x0805E45C,
            0x0805ECD8,
        };

        // Load
        public override UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            // All maps are hard-coded
            throw new System.NotImplementedException();
        }
    }
}