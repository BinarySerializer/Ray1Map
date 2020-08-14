using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBA_RRR_Manager : IGameManager
    {
        public KeyValuePair<int, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<int, int[]>[0];

        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        public virtual string GetROMFilePath => $"ROM.gba";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Blocks", false, true, (input, output) => ExportBlocksAsync(settings, output)), 
        };

        public async UniTask ExportBlocksAsync(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                var s = context.Deserializer;

                await LoadFilesAsync(context);

                var baseOffset = context.GetFile(GetROMFilePath).StartPointer + 0x722374;
                
                s.Goto(baseOffset);

                var length = s.Serialize<uint>(default);

                for (int i = 0; i < length; i++)
                {
                    s.Serialize<uint>(default);
                    var blockSize = s.Serialize<uint>(default);
                    var blockOffset = s.Serialize<uint>(default);
                    s.Serialize<uint>(default);

                    var blockPointer = baseOffset + blockOffset;

                    var block = s.DoAt(blockPointer, () => s.SerializeArray<byte>(default, blockSize));

                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}.dat"), block);

                    if (block.Length > 4 && block[0] == 0x67 && block[1] == 0x45 && block[2] == 0x23 && block[3] == 0x01) {
                        s.DoAt(blockPointer, () => {
                            s.DoEncoded(new RRRGBA_LZSSEncoder(blockSize), () => {
                                block = s.SerializeArray<byte>(default, s.CurrentLength);
                                Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}_decompressed.dat"), block);
                            });
                        });
                    }
                }
            }
        }

        public UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures) => throw new NotImplementedException();

        public void SaveLevel(Context context, BaseEditorManager editorManager) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new GBAMemoryMappedFile(context, 0x08000000)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}