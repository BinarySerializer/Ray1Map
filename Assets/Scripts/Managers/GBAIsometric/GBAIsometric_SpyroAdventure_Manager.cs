using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.IO;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_SpyroAdventure_Manager : IGameManager
    {
        public const int CellSize = 8;

        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 38).ToArray()), 
        });

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
                new GameAction("Export Data Blocks", false, true, (input, output) => ExportDataBlocks(settings, output))
        };

        public async UniTask ExportDataBlocks(GameSettings settings, string outputPath) {
            using (var context = new Context(settings)) {
                var s = context.Deserializer;
                await LoadFilesAsync(context);

                // TODO: Don't hard-code pointer
                var dataTable = s.DoAt(context.FilePointer(GetROMFilePath) + 0x1c0b60, () => s.SerializeObject<GBAIsometric_Spyro_DataTable>(default, name: "DataTable"));

                for (int i = 0; i < dataTable.DataEntries.Length; i++)
                {
                    var data = dataTable.DoAtBlock(context, i, size => s.SerializeArray<byte>(default, size, name: $"Block[{i}]"));
                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i:000}_0x{dataTable.DataEntries[i].DataPointer.AbsoluteOffset:X8}.dat"), data);
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            throw new NotImplementedException();
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}