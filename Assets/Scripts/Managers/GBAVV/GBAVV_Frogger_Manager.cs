using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System.Linq;

namespace R1Engine
{
    public class GBAVV_Frogger_Manager : GBAVV_Generic_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 17).ToArray()),
            new GameInfo_World(1, Enumerable.Range(0, (5 - 1) * 2).ToArray()), // Last level is null
        });

        // Exports
        public override GBAVV_ROM_Generic LoadGenericROM(Context context) => FileFactory.Read<GBAVV_ROM_Frogger>(GetROMFilePath, context);
        public override GBAVV_ROM_Generic LoadGenericROM_Mode7(Context context, int level) => null;
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_Frogger>(GetROMFilePath, context);

            return await LoadMap2DAsync(context, rom, rom.CurrentMapInfo, rom.GetTheme);
        }
        public override bool HasAssignedObjTypeGraphics => false;
    }
}