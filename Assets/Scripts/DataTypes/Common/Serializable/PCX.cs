using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// PCX file data
    /// </summary>
    public class PCX : R1Serializable
    {
        public byte Identifier { get; set; }

        public byte Version { get; set; }

        public byte Encoding { get; set; }

        public byte BitsPerPixel { get; set; }

        public ushort XStart { get; set; }

        public ushort YStart { get; set; }

        public ushort XEnd { get; set; }

        public ushort YEnd { get; set; }

        public ushort HorRes { get; set; }

        public ushort VerRes { get; set; }

        public byte[] EGAPalette { get; set; }

        public byte Reserved1 { get; set; }

        public byte BitPlaneCount { get; set; }

        public ushort BytesPerLine { get; set; }

        public ushort PaletteType { get; set; }

        public ushort HorScreenSize { get; set; }

        public ushort VerScreenSize { get; set; }

        public byte[] Reserved2 { get; set; }

        public byte[][] ScanLines { get; set; }

        public byte PaletteStart { get; set; }

        public byte[] VGAPalette { get; set; }

        public int ImageWidth => XEnd - XStart + 1;

        public int ImageHeight => YEnd - YStart + 1;

        /// <summary>
        /// Converts the PCX data to a texture
        /// </summary>
        /// <returns>The texture</returns>
        public Texture2D ToTexture()
        {
            // Create the texture
            var tex = new Texture2D(ImageWidth, ImageHeight, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };

            // Set every pixel
            for (int y = 0; y < ImageHeight; y++)
            {
                for (int x = 0; x < ImageWidth; x++)
                {
                    // Get the palette index
                    var paletteIndex = ScanLines[y][x];

                    // Get the colors
                    var r = VGAPalette[(paletteIndex * 3) + 0] / 255f;
                    var g = VGAPalette[(paletteIndex * 3) + 1] / 255f;
                    var b = VGAPalette[(paletteIndex * 3) + 2] / 255f;

                    // Set the pixel
                    tex.SetPixel(x, y, new Color(r, g, b, 1));
                }
            }

            // Apply the pixels
            tex.Apply();

            // Return the texture
            return tex;
        }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            // Serialize the header
            Identifier = s.Serialize<byte>(Identifier, name: "Identifier");
            Version = s.Serialize<byte>(Version, name: "Version");
            Encoding = s.Serialize<byte>(Encoding, name: "Encoding");
            BitsPerPixel = s.Serialize<byte>(BitsPerPixel, name: "BitsPerPixel");
            XStart = s.Serialize<ushort>(XStart, name: "XStart");
            YStart = s.Serialize<ushort>(YStart, name: "YStart");
            XEnd = s.Serialize<ushort>(XEnd, name: "XEnd");
            YEnd = s.Serialize<ushort>(YEnd, name: "YEnd");
            HorRes = s.Serialize<ushort>(HorRes, name: "HorRes");
            VerRes = s.Serialize<ushort>(VerRes, name: "VerRes");
            EGAPalette = s.SerializeArray<byte>(EGAPalette, 48, name: "EGAPalette");
            Reserved1 = s.Serialize<byte>(Reserved1, name: "Reserved1");
            BitPlaneCount = s.Serialize<byte>(BitPlaneCount, name: "BitPlaneCount");
            BytesPerLine = s.Serialize<ushort>(BytesPerLine, name: "BytesPerLine");
            PaletteType = s.Serialize<ushort>(PaletteType, name: "PaletteType");
            HorScreenSize = s.Serialize<ushort>(HorScreenSize, name: "HorScreenSize");
            VerScreenSize = s.Serialize<ushort>(VerScreenSize, name: "VerScreenSize");
            Reserved2 = s.SerializeArray<byte>(Reserved2, 54, name: "Reserved2");

            if (s is BinaryDeserializer)
            {
                // Calculate properties
                var scanLineLength = BitPlaneCount * BytesPerLine;
                var linePaddingSize = ((scanLineLength) * (8 / BitsPerPixel)) - ImageWidth;

                // Create the scan-line array
                ScanLines = new byte[ImageHeight][];

                // Read every scan-line
                for (int i = 0; i < ImageHeight; i++)
                {
                    // Keep track of the index
                    int index = 0;

                    // Create the buffer
                    byte[] buffer = new byte[scanLineLength];

                    do
                    {
                        // Read the byte
                        var b = s.Serialize<byte>(0, name: "b");

                        int repeatCount;
                        byte runValue = 0;

                        // Check if it should be repeated
                        if ((b & 0xC0) == 0xC0)
                        {
                            repeatCount = b & 0x3F;
                            runValue = s.Serialize<byte>(runValue, name: "runValue");
                        }
                        else
                        {
                            repeatCount = 1;
                            runValue = b;
                        }

                        // Write the specified number of bytes
                        while (index < buffer.Length && repeatCount > 0)
                        {
                            buffer[index] = runValue;
                            repeatCount--;
                            index++;
                        }

                    } while (index < buffer.Length);

                    // Set the scan-line
                    ScanLines[i] = buffer;

                    // Read padding
                    s.SerializeArray<byte>(null,linePaddingSize, name: "padding");
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            // Serialize the initial palette byte
            PaletteStart = s.Serialize<byte>(PaletteStart, name: "PaletteStart");

            // Serialize the palette
            VGAPalette = s.SerializeArray<byte>(VGAPalette, 256 * 3, name: "VGAPalette");
        }
    }
}