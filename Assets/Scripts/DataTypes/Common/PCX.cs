using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// PCX file data
    /// </summary>
    public class PCX : IBinarySerializable
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
                    tex.SetPixel(x, ImageHeight - y - 1, new Color(r, g, b, 1));
                }
            }

            // Apply the pixels
            tex.Apply();

            // Return the texture
            return tex;
        }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Read the header
            Identifier = deserializer.Read<byte>();
            Version = deserializer.Read<byte>();
            Encoding = deserializer.Read<byte>();
            BitsPerPixel = deserializer.Read<byte>();
            XStart = deserializer.Read<ushort>();
            YStart = deserializer.Read<ushort>();
            XEnd = deserializer.Read<ushort>();
            YEnd = deserializer.Read<ushort>();
            HorRes = deserializer.Read<ushort>();
            VerRes = deserializer.Read<ushort>();
            EGAPalette = deserializer.ReadArray<byte>(48);
            Reserved1 = deserializer.Read<byte>();
            BitPlaneCount = deserializer.Read<byte>();
            BytesPerLine = deserializer.Read<ushort>();
            PaletteType = deserializer.Read<ushort>();
            HorScreenSize = deserializer.Read<ushort>();
            VerScreenSize = deserializer.Read<ushort>();
            Reserved2 = deserializer.ReadArray<byte>(54);

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
                    var b = deserializer.Read<byte>();

                    int repeatCount;
                    byte runValue;

                    // Check if it should be repeated
                    if ((b & 0xC0) == 0xC0)
                    {
                        repeatCount = b & 0x3F;
                        runValue = deserializer.Read<byte>();
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
                deserializer.ReadArray<byte>((ulong)linePaddingSize);
            }

            // Read the initial palette byte
            PaletteStart = deserializer.Read<byte>();

            // Read the palette
            VGAPalette = deserializer.ReadArray<byte>(256 * 3);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}