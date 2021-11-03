﻿using System.Collections.Generic;
using System.IO;
using BinarySerializer;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// Helper methods for palettes
    /// </summary>
    public static class PaletteHelpers
    {
        /// <summary>
        /// Exports the specified palette to the gimp palette format
        /// </summary>
        /// <param name="outputPath">The path to export to</param>
        /// <param name="name">The palette name</param>
        /// <param name="palette">The palette</param>
        public static void ExportPaletteToGimp(string outputPath, string name, BaseColor[] palette)
        {
            // Create the file
            using (var fileStream = File.Create(outputPath))
            {
                // Use a writer
                using (var writer = new StreamWriter(fileStream))
                {
                    // Write header
                    writer.WriteLine("GIMP Palette");
                    writer.WriteLine("Name: " + name);
                    writer.WriteLine("#");

                    // Write colors
                    foreach (var color in palette)
                        writer.WriteLine($"{color.Red,-3} {color.Green,-3} {color.Blue,-3}");
                }
            }
        }

        /// <summary>
        /// Exports a palette to a file
        /// </summary>
        /// <param name="outputPath">The path to export to</param>
        /// <param name="palette">The palette</param>
        public static void ExportPalette(string outputPath, IList<BaseColor> palette, int scale = 16, int offset = 0, int? optionalLength = null, int? optionalWrap = null, bool reverseY = false)
        {
            int length = optionalLength ?? palette.Count;
            int wrap = optionalWrap ?? length;
            var tex = TextureHelpers.CreateTexture2D(Mathf.Min(length, wrap) * scale, Mathf.CeilToInt(length / (float)wrap) * scale, clear: true);

            for (int i = 0; i < length; i++)
            {
                int mainY = (tex.height/scale) - 1 - (i / wrap);
                int mainX = i % wrap;
                
                Color col = palette[offset + i].GetColor();
                
                // Remove transparency
                col = new Color(col.r, col.g, col.b);
                
                for (int y = 0; y < scale; y++)
                {
                    for (int x = 0; x < scale; x++)
                    {
                        var xx = mainX * scale + x;
                        var yy = mainY * scale + y;

                        if (reverseY)
                            yy = tex.height - yy - 1;

                        tex.SetPixel(xx, yy, col);
                    }
                }
            }

            tex.Apply();

            Util.ByteArrayToFile(outputPath, tex.EncodeToPNG());
        }

        /// <summary>
        /// Exports the v-ram as an image
        /// </summary>
        /// <param name="outputPath">The path to export to</param>
        /// <param name="vram">The v-ram</param>
        public static void ExportVram(string outputPath, PS1_VRAM vram)
        {
            Texture2D vramTex = TextureHelpers.CreateTexture2D(16 * 128, 2 * 256);
            for (int x = 0; x < 16 * 128; x++) {
                for (int y = 0; y < 2 * 256; y++) {
                    byte val = vram.GetPixel8(0, y / 256, x, y % 256);
                    vramTex.SetPixel(x, (2 * 256) - 1 - y, new Color(val / 255f, val / 255f, val / 255f));
                }
            }
            vramTex.Apply();
            Util.ByteArrayToFile(outputPath, vramTex.EncodeToPNG());
        }
    }
}