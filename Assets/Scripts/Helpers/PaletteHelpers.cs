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
    }
}