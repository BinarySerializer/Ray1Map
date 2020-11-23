using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.IO;

namespace R1Engine
{
    public class GBC_R1PalmOS_Manager : IGameManager
    {
        public GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[0]);

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Data", false, true, (input, output) => ExportDataAsync(settings, output)), 
        };

        public async UniTask ExportDataAsync(GameSettings settings, string outputDir)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                foreach (var filePath in Directory.GetFiles(context.BasePath, "*", SearchOption.TopDirectoryOnly))
                {
                    var ext = Path.GetExtension(filePath);
                    var type = ext == ".pdb" ? Palm_Database.DatabaseType.PDB : (ext == ".prc" ? Palm_Database.DatabaseType.PRC : (Palm_Database.DatabaseType?)null);

                    if (type == null)
                        continue;

                    var relPath = Path.GetFileName(filePath);
                    await context.AddLinearSerializedFileAsync(relPath, BinaryFile.Endian.Big);

                    var dataBase = FileFactory.Read<Palm_Database>(relPath, context, (so, pd) => pd.Type = type.Value);

                    for (int i = 0; i < dataBase.RecordsCount; i++)
                    {
                        var record = dataBase.Records[i];
                        var name = type == Palm_Database.DatabaseType.PRC ? $"{record.Name}_{record.ID}" : $"{i}";

                        var bytes = s.DoAt(record.DataPointer, () => s.SerializeArray<byte>(default, record.Length, name: $"Record[{i}]"));
                        Util.ByteArrayToFile(Path.Combine(outputDir, Path.GetFileNameWithoutExtension(relPath), $"{name}.bin"), bytes);
                    }
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) => throw new NotImplementedException();

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        public UniTask LoadFilesAsync(Context context) => UniTask.CompletedTask;
    }
}