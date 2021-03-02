using System;
using System.IO;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_Manager : IGameManager
    {
        public string ExeFilePath => @"6rac.app";
        public string DataFilePath => @"data.gob";

        public GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[0];

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output)),
        };

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputPath, bool includeAbsolutePointer = true)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                await LoadFilesAsync(context);

                // Load the data file
                var data = FileFactory.Read<GBAVV_NitroKart_NGage_DataFile>(DataFilePath, context);

                // Enumerate every block in the offset table
                for (uint i = 0; i < data.BlocksCount; i++)
                {
                    // Get the offset
                    var offset = data.DataFileEntries[i];

                    var append = includeAbsolutePointer ? $"_{offset.Pointer.AbsoluteOffset:X8}" : String.Empty;

                    var bytes = s.DoAt(offset.Pointer, () => s.SerializeArray<byte>(default, offset.BlockLength, name: $"Block[{i}]"));

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}{append}.dat"), bytes);
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            throw new NotImplementedException();
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public async UniTask LoadFilesAsync(Context context)
        {
            await context.AddLinearSerializedFileAsync(ExeFilePath);
            await context.AddLinearSerializedFileAsync(DataFilePath);
        }
    }
}