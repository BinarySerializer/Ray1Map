using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public abstract class GBAVV_BruceLeeReturnOfTheLegend_Manager : GBAVV_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 36).ToArray()),
        });

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_BruceLee>(GetROMFilePath, context);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_BruceLee>(GetROMFilePath, context);

            return await LoadMap2DAsync(context, rom, rom.CurrentMap, false);
        }
    }
    public class GBAVV_BruceLeeReturnOfTheLegendEU_Manager : GBAVV_BruceLeeReturnOfTheLegend_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0803DA60,
            0x08049B78,
            0x0806D56C,
        };
    }
    public class GBAVV_BruceLeeReturnOfTheLegendUS_Manager : GBAVV_BruceLeeReturnOfTheLegend_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x0802EE84,
            0x0803AF9C,
            0x0805E990,
        };
    }
}