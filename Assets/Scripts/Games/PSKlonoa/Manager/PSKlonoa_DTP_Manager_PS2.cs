using System;
using System.IO;
using BinarySerializer;
using BinarySerializer.Klonoa;
using BinarySerializer.Klonoa.DTP;
using Cysharp.Threading.Tasks;

namespace Ray1Map.PSKlonoa
{
    public class PSKlonoa_DTP_Manager_PS2 : BaseGameManager
    {
        #region Manager

        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(Array.Empty<GameInfo_World>());

        public KlonoaSettings_DTP_PS2 GetKlonoaSettings(GameSettings settings) => new KlonoaSettings_DTP_PS2();

        #endregion

        #region Game Actions

        public override GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Extract STM", false, true, (_, output) => Extract_STMAsync(settings, output)),
            new GameAction("Extract EXE ULZ Data", false, true, (_, output) => Extract_ULZExeDataAsync(settings, output)),
        };

        public async UniTask Extract_STMAsync(GameSettings settings, string outputPath)
        {
            using var context = new Ray1MapContext(settings);
            KlonoaSettings_DTP_PS2 config = GetKlonoaSettings(settings);
            await LoadFilesAsync(context, config);
            context.AddKlonoaSettings(config);
            BinaryDeserializer s = context.Deserializer;

            // Extract from all STM files
            foreach (var fileInfo in config.FileTable)
            {
                STM stm = FileFactory.Read<STM>(context, fileInfo.FilePath);

                string stmName = fileInfo.Name;

                // Last file is a dummy file
                for (int i = 0; i < stm.Entries.Length - 1; i++)
                {
                    STM_Entry entry = stm.Entries[i];
                    string fileName = fileInfo.Files[i].FileName;

                    s.Goto(entry.FileOffset);

                    byte[] data = s.SerializeArray<byte>(default, entry.FileLength, name: nameof(fileName));

                    Util.ByteArrayToFile(Path.Combine(outputPath, stmName, fileName), data);
                }
            }
        }

        // Extracts raw image data for the credits
        public async UniTask Extract_ULZExeDataAsync(GameSettings settings, string outputPath)
        {
            using var context = new Ray1MapContext(settings);
            KlonoaSettings_DTP_PS2 config = GetKlonoaSettings(settings);
            await LoadFilesAsync(context, config, -2);
            context.AddKlonoaSettings(config);
            BinaryDeserializer s = context.Deserializer;

            Util.ExportAllCompressedData(
                context: context, 
                offset: context.FilePointer(config.FilePath_EXE), 
                encoder: new ULZEncoder(), 
                header: new byte[] { 0x55, 0x6C, 0x7A, 0x1A }, 
                outputDir: outputPath, minDecompSize: 0);
        }

        #endregion

        #region Public Methods

        public override UniTask<Unity_Level> LoadAsync(Context context)
        {
            throw new NotImplementedException();
        }

        public async UniTask LoadFilesAsync(Context context, KlonoaSettings_DTP_PS2 config, int binBlock = -1)
        {
            // Load the exe
            await context.AddLinearFileAsync(config.FilePath_EXE);

            if (binBlock >= 0)
            {
                // Load the boot data
                await context.AddLinearFileAsync(config.FileTable[config.BLOCK_Boot].FilePath);

                // Load the level
                await context.AddLinearFileAsync(config.FileTable[binBlock].FilePath);
            }
            else if (binBlock == -1)
            {
                // Load all files
                foreach (var fileInfo in config.FileTable)
                    await context.AddLinearFileAsync(fileInfo.FilePath);
            }
        }

        #endregion
    }
}