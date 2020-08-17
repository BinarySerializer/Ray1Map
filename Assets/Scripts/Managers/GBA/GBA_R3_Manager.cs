using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBA_R3_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 8),
            Enumerable.Range(8, 9),
            Enumerable.Range(17, 13),
            Enumerable.Range(30, 10),
            Enumerable.Range(40, 14),
            Enumerable.Range(54, 5),
            Enumerable.Range(59, 6),
        };

        // TODO: Get values
        public override int[] MenuLevels => new int[]
        {
            117
        };

        public override int DLCLevelCount => 10;

        // TODO: Find the way the game gets the vignette offsets and find remaining vignettes
        public override async UniTask ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Get the file
                var file = context.GetFile(GetROMFilePath);

                // Get the deserialize
                var s = context.Deserializer;

                // TODO: Find scrolling vignette for proto
                var isPrototype = settings.GameModeSelection == GameModeSelection.Rayman3GBAUSPrototype;

                // TODO: Move pointers to pointer table
                int vigCount = isPrototype ? 18 : 20;
                uint palOffset = (uint)(isPrototype ? 0x0DC46A : 0x0B37C0);
                uint vigOffset = (uint)(isPrototype ? 0x45FE3C : 0x20ED94);

                var palettes = new ARGB1555Color[vigCount][];

                for (int i = 0; i < vigCount; i++)
                    palettes[i] = s.DoAt((file.StartPointer + palOffset) + (512 * i), () => s.SerializeObjectArray<ARGB1555Color>(default, 256));

                // Go to vignette offset
                s.Goto(file.StartPointer + vigOffset);

                // Export every vignette
                for (int i = 0; i < vigCount; i++)
                {
                    // Get the offset
                    var offset = s.CurrentPointer;

                    // Decode data
                    byte[] data = null;
                    s.DoEncoded(new GBA_LZSSEncoder(), () => data = s.SerializeArray<byte>(default, s.CurrentLength));

                    int width;
                    int height;

                    switch (data.Length)
                    {
                        case 0x9600:
                            width = 240;
                            height = 160;
                            break;

                        case 0x5A00:
                            width = 192;
                            height = 120;
                            break;

                        // TODO: Support scrolling vignette

                        default:
                            throw new Exception("Vignette length is not supported");
                    }

                    // Create a texture
                    var tex = new Texture2D(width, height); ;

                    var palIndex = i;

                    if (isPrototype)
                    {
                        // Palettes for 5 and 6 are swapped
                        if (palIndex == 5)
                            palIndex = 6;
                        else if (palIndex == 6)
                            palIndex = 5;
                    }
                    else
                    {
                        // Palettes for 6 and 7 are swapped
                        if (palIndex == 6)
                            palIndex = 7;
                        else if (palIndex == 7)
                            palIndex = 6;
                    }

                    // Set pixels
                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            var c = palettes[palIndex][data[y * tex.width + x]].GetColor();

                            // Remove transparency
                            c.a = 1;

                            // Set pixel and reverse height
                            tex.SetPixel(x, tex.height - y - 1, c);
                        }
                    }

                    tex.Apply();

                    // Export
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"Vig_{i}_0x{offset.AbsoluteOffset:X8}.png"), tex.EncodeToPNG());

                    // Align
                    s.Align();
                }
            }
        }
    }
}