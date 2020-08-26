using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBA_R3NGage_Manager : GBA_R3_Manager
    {
        public override string GetROMFilePath => $"rayman3.dat";

        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 8),
            Enumerable.Range(8, 9),
            Enumerable.Range(17, 13),
            Enumerable.Range(30, 10),
            Enumerable.Range(40, 14),
            Enumerable.Range(54, 5),
            Enumerable.Range(59, 10),
        };

        public override int[] MenuLevels => new int[]
        {
            103,
            131
        };

        public override int DLCLevelCount => 0;

        public override int[] AdditionalSprites4bpp => Enumerable.Range(79, 103 - 79).Concat(Enumerable.Range(104, 131 - 104)).Concat(Enumerable.Range(133, 140 - 133)).ToArray();

        public override int[] AdditionalSprites8bpp => new int[]
        {
            132
        };

        public override async UniTask ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Load the data
                var dataBlock = LoadDataBlock(context);

                // Get the deserialize
                var s = context.Deserializer;

                const int vignetteCount = 22;
                const int vignetteStart = 143;
                const int palStart = 165;

                for (int i = 0; i < vignetteCount; i++)
                {
                    // Decode image data
                    byte[] data = null;
                    s.DoAt(dataBlock.UiOffsetTable.GetPointer(vignetteStart + i), () => s.DoEncoded(new GBA_LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength)));

                    // Read palette
                    var palette = s.DoAt(dataBlock.UiOffsetTable.GetPointer(palStart + i), () => s.SerializeObjectArray<ARGB1555Color>(default, 256));

                    // Create a texture
                    var tex = TextureHelpers.CreateTexture2D(176, 208);

                    // Set pixels
                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            var c = palette[data[y * tex.width + x]].GetColor();

                            // Remove transparency
                            c.a = 1;

                            // Set pixel and reverse height
                            tex.SetPixel(x, tex.height - y - 1, c);
                        }
                    }

                    tex.Apply();

                    // Export
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Vig_{i}.png"), tex.EncodeToPNG());
                }

                ExtractVignetteCreditsIcons(context, dataBlock, 141, outputDir);
            }
        }

        public override GBA_Data LoadDataBlock(Context context) => FileFactory.Read<GBA_Data>(GetROMFilePath, context);
        public override GBA_LocLanguageTable LoadLocalization(Context context) => null;

        public override async UniTask LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new LinearSerializedFile(context)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}