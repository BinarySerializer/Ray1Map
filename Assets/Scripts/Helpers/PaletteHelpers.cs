using System.IO;

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
    }
}