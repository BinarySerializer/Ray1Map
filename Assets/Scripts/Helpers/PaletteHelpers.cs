using System.IO;
using UnityEngine;

namespace R1Engine
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
        public static void ExportPaletteToGimp(string outputPath, string name, ARGBColor[] palette)
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
        public static void ExportPalette(string outputPath, ARGBColor[] palette)
        {
            var tex = new Texture2D(palette.Length * 16, 16);

            for (int i = 0; i < palette.Length; i++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        tex.SetPixel(i * 16 + x, y, palette[i].GetColor());
                    }
                }
            }

            tex.Apply();

            File.WriteAllBytes(outputPath, tex.EncodeToPNG());
        }

        /// <summary>
        /// Exports the v-ram as an image
        /// </summary>
        /// <param name="outputPath">The path to export to</param>
        /// <param name="vram">The v-ram</param>
        public static void ExportVram(string outputPath, PS1_VRAM vram)
        {
            Texture2D vramTex = new Texture2D(16 * 128, 2 * 256);
            for (int x = 0; x < 16 * 128; x++) {
                for (int y = 0; y < 2 * 256; y++) {
                    byte val = vram.GetPixel8(0, y / 256, x, y % 256);
                    vramTex.SetPixel(x, y, new Color(val / 255f, val / 255f, val / 255f));
                }
            }
            vramTex.Apply();
            Util.ByteArrayToFile(outputPath, vramTex.EncodeToPNG());
        }
    }
}