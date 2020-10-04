using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBA_RRR_Manager : IGameManager
    {
        public GameInfo_Volume[] GetLevels(GameSettings settings) => new GameInfo_Volume[0];

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

                    if (block.Length > 4 && block[0] == 0x67 && block[1] == 0x45 && block[2] == 0x23 && block[3] == 0x01) {
                        s.DoAt(blockPointer, () => {
                            s.DoEncoded(new LZSSEncoder(blockSize), () => 
                            {
                                const int width = 240;
                                const int height = 160;

                                // Check if it's a vignette...
                                if (s.CurrentLength == (256 * 2) + (width * height))
                                {
                                    // Serialize palette
                                    var pal = s.SerializeObjectArray<ARGB1555Color>(default, 256).Select(x =>
                                    {
                                        var c = x.GetColor();
                                        c.a = 1;
                                        return c;
                                    }).ToArray();

                                    var tex = TextureHelpers.CreateTexture2D(width, height, true);

                                    for (int y = 0; y < height; y++)
                                    {
                                        for (int x = 0; x < width; x++)
                                        {
                                            var b = s.Serialize<byte>(default);

                                            tex.SetPixel(x, height - y - 1, pal[b]);
                                        }
                                    }

                                    tex.Apply();

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}_decompressed.png"), tex.EncodeToPNG());
                                }
                                else
                                {
                                    block = s.SerializeArray<byte>(default, s.CurrentLength);
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}_decompressed.dat"), block);
                                }
                            });
                        });
                    } else {
                        Util.ByteArrayToFile(Path.Combine(outputPath, $"{i}_{blockPointer.AbsoluteOffset:X8}.dat"), block);
                    }
                }
            }
        }

        public UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) => throw new NotImplementedException();

        public void SaveLevel(Context context, Unity_Level level) => throw new NotImplementedException();

        public virtual async UniTask LoadFilesAsync(Context context) => await context.AddGBAMemoryMappedFile(GetROMFilePath, 0x08000000);
    }
}