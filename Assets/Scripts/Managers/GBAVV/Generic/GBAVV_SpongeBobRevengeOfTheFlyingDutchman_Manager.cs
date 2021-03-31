using Cysharp.Threading.Tasks;

using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine
{
    public abstract class GBAVV_SpongeBobRevengeOfTheFlyingDutchman_Manager : GBAVV_Generic_BaseManager
    {
        // Metadata
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 7).ToArray()),
            new GameInfo_World(1, Enumerable.Range(0, 11).ToArray()),
            new GameInfo_World(2, Enumerable.Range(0, 11).ToArray()),
            new GameInfo_World(3, Enumerable.Range(0, 11).ToArray()),
            new GameInfo_World(4, Enumerable.Range(0, 10).ToArray()),
            new GameInfo_World(5, Enumerable.Range(0, 11).ToArray()),
            new GameInfo_World(6, Enumerable.Range(0, 11).ToArray()),
            new GameInfo_World(7, Enumerable.Range(0, 15).ToArray()),
            new GameInfo_World(8, Enumerable.Range(0, 7).ToArray()),
            new GameInfo_World(9, Enumerable.Range(0, 8).ToArray()),
            new GameInfo_World(10, Enumerable.Range(0, 12).ToArray()),
            new GameInfo_World(11, Enumerable.Range(0, 14).ToArray()),
            new GameInfo_World(12, Enumerable.Range(0, 1).ToArray()),
            new GameInfo_World(13, Enumerable.Range(0, 1).ToArray()),
            new GameInfo_World(14, Enumerable.Range(0, 1).ToArray()),
            new GameInfo_World(15, Enumerable.Range(0, 1).ToArray()),
            new GameInfo_World(16, Enumerable.Range(0, 8).ToArray()),
            new GameInfo_World(17, Enumerable.Range(0, 9).ToArray()),
            new GameInfo_World(18, Enumerable.Range(0, 9).ToArray()),
            new GameInfo_World(19, Enumerable.Range(0, 9).ToArray()),
            new GameInfo_World(20, Enumerable.Range(0, 9).ToArray()),
            new GameInfo_World(21, Enumerable.Range(0, 7).ToArray()),
            new GameInfo_World(22, Enumerable.Range(0, 11).ToArray()),
            new GameInfo_World(23, Enumerable.Range(0, 6).ToArray()),
            new GameInfo_World(24, Enumerable.Range(0, 7).ToArray()),
            new GameInfo_World(25, Enumerable.Range(0, 10).ToArray()),
            new GameInfo_World(26, Enumerable.Range(0, 11).ToArray()),
            new GameInfo_World(27, Enumerable.Range(0, 1).ToArray()),
            new GameInfo_World(28, Enumerable.Range(0, 1).ToArray()),
            new GameInfo_World(29, Enumerable.Range(0, 1).ToArray()),
            new GameInfo_World(30, Enumerable.Range(0, 1).ToArray()),
        });

        // Exports
        public override GBAVV_BaseROM LoadROMForExport(Context context) => FileFactory.Read<GBAVV_ROM_SpongeBobRevengeOfTheFlyingDutchman>(GetROMFilePath, context);
        public override GBAVV_ROM_Generic LoadROMForMode7Export(Context context, int level) => FileFactory.Read<GBAVV_ROM_SpongeBobRevengeOfTheFlyingDutchman>(GetROMFilePath, context, (_, x) => x.ForceMode7 = true);
        public override UniTask ExportCutscenesAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();

        // Load
        public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = "Loading data";
            await Controller.WaitIfNecessary();

            var rom = FileFactory.Read<GBAVV_ROM_SpongeBobRevengeOfTheFlyingDutchman>(GetROMFilePath, context);

            if (rom.CurrentMapInfo.SpongeBob_IsMode7)
                return await LoadMode7Async(context, rom, rom.Mode7_LevelInfos[rom.CurrentMapInfo.Index3D]);
            else
                return await LoadMap2DAsync(context, rom, rom.CurrentMapInfo, rom.GetTheme, false);
        }
        public abstract uint ROMMemPointersOffset { get; }

        // Mode7
        public override int[] Mode7AnimSetCounts => new int[]
        {
            34
        };
        public override int Mode7LevelsCount => 9;

        // Helpers
        public void GenerateWorldList(GBAVV_ROM_SpongeBobRevengeOfTheFlyingDutchman rom)
        {
            var str = new StringBuilder();

            for (int i = 0; i < rom.LevelInfos.Length; i++)
                str.AppendLine($"new GameInfo_World({i}, Enumerable.Range(0, {rom.LevelInfos[i].MapsCount}).ToArray()),");

            str.ToString().CopyToClipboard();
        }
    }
    public class GBAVV_SpongeBobRevengeOfTheFlyingDutchmanEUUS_Manager : GBAVV_SpongeBobRevengeOfTheFlyingDutchman_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x084AB158
        };

        public override uint ROMMemPointersOffset => 0x57C5DFC;
    }
    public class GBAVV_SpongeBobRevengeOfTheFlyingDutchmanUSBeta_Manager : GBAVV_SpongeBobRevengeOfTheFlyingDutchman_Manager
    {
        public override uint[] GraphicsDataPointers => new uint[]
        {
            0x084ACC9C
        };
        public override uint ROMMemPointersOffset => 0x57C7B80;
    }
}