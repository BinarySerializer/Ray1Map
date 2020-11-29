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
            Enumerable.Range(0, 8), // World 1
            Enumerable.Range(8, 9), // World 2
            Enumerable.Range(17, 13), // World 3
            Enumerable.Range(30, 10), // World 4
            Enumerable.Range(40, 8), // Bonus
            Enumerable.Range(48, 6), // Ly
            Enumerable.Range(54, 5), // World
            Enumerable.Range(59, 6), // Multiplayer
        };

        public override int[] MenuLevels => new int[]
        {
            91,
            117
        };

        public override int DLCLevelCount => 10;

        public override int[] AdditionalSprites4bpp => Enumerable.Range(70, 91 - 70).Concat(Enumerable.Range(92, 117 - 92)).Concat(Enumerable.Range(119, 126 - 119)).ToArray();
        public override int[] AdditionalSprites8bpp => new int[]
        {
            118
        };

        // TODO: Find the way the game gets the vignette offsets
        public override async UniTask ExtractVignetteAsync(GameSettings settings, string outputDir)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Load the ROM
                await LoadFilesAsync(context);

                // Get the file
                var file = context.GetFile(GetROMFilePath(context));

                // Get the deserialize
                var s = context.Deserializer;

                var pointerTable = PointerTables.GBA_PointerTable(context, file);

                int vigCount = settings.GameModeSelection == GameModeSelection.Rayman3GBAUSPrototype ? 18 : 20;

                var palettes = new RGBA5551Color[vigCount][];

                for (int i = 0; i < vigCount; i++)
                    palettes[i] = s.DoAt(pointerTable[GBA_Pointer.VignettePalettes] + (512 * i), () => s.SerializeObjectArray<RGBA5551Color>(default, 256));

                // Go to vignette offset
                s.Goto(pointerTable[GBA_Pointer.Vignette]);

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

                        default:
                            throw new Exception("Vignette length is not supported");
                    }

                    // Create a texture
                    var tex = TextureHelpers.CreateTexture2D(width, height);

                    var palIndex = i;

                    if (settings.GameModeSelection == GameModeSelection.Rayman3GBAUSPrototype)
                    {
                        // Palettes for 5 and 6 are swapped
                        if (palIndex == 5)
                            palIndex = 6;
                        else if (palIndex == 6)
                            palIndex = 5;
                    }
                    else if (settings.GameModeSelection != GameModeSelection.Rayman3Digiblast)
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

                if (settings.GameModeSelection != GameModeSelection.Rayman3GBAUSPrototype)
                    ExtractVignetteCreditsIcons(context, LoadDataBlock(context), 127, outputDir);
            }
        }

        public void ExtractVignetteCreditsIcons(Context context, GBA_Data dataBlock, int offset, string outputDir)
        {
            var s = context.Deserializer;

            const int creditsIcons = 16;

            var creditsIconsData = s.DoAt(dataBlock.UiOffsetTable.GetPointer(offset), () => s.SerializeObject<GBA_DummyBlock>(default)).Data;
            var creditsIconsPal = s.DoAt(dataBlock.UiOffsetTable.GetPointer(offset + 1) + 8, () => s.SerializeObjectArray<RGBA5551Color>(default, 256));

            for (int i = 0; i < creditsIcons; i++)
            {
                // Create a texture
                var creditsIconsTex = TextureHelpers.CreateTexture2D(64, 64);

                // Set pixels
                for (int y = 0; y < creditsIconsTex.height; y++)
                {
                    for (int x = 0; x < creditsIconsTex.width; x++)
                    {
                        var c = creditsIconsPal[creditsIconsData[y * creditsIconsTex.width + x + ((i + 1) * 4) + (i * (64 * 64)) + 20]].GetColor();

                        // Remove transparency
                        c.a = 1;

                        // Set pixel
                        creditsIconsTex.SetPixel(x, y, c);
                    }
                }

                creditsIconsTex.Apply();

                // Export
                Util.ByteArrayToFile(Path.Combine(outputDir, $"Icon_{i}.png"), creditsIconsTex.EncodeToPNG());
            }
        }
    }
}