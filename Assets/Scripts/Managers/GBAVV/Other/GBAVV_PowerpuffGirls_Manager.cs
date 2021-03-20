using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public abstract class GBAVV_PowerpuffGirls_Manager : GBAVV_BaseManager
    {
        // Metadata

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 58).ToArray()),
        });

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_PowerpuffGirls>(GetROMFilePath, context);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_PowerpuffGirls>(GetROMFilePath, context);

            return await LoadMap2DAsync(context, rom, rom.CurrentMap, false);
        }
    }
    public class GBAVV_PowerpuffGirlsEU_Manager : GBAVV_PowerpuffGirls_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x08056E04,
            0x08064E08,
            0x0806B2D8,
            0x08079BE4,
            0x08081C38,
            0x08081F38,
            0x080835A0,
            0x080854A8,
        };
    }
    public class GBAVV_PowerpuffGirlsUS_Manager : GBAVV_PowerpuffGirls_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x080447C4,
            0x080527C8,
            0x08058C98,
            0x080675A0,
            0x0806F5F4,
            0x0806F8F4,
            0x08070F5C,
            0x08072E64,
        };
    }
}