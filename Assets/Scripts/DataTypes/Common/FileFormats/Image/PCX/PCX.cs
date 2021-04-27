using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// PCX file data
    /// </summary>
    public class PCX : BinarySerializable
    {
        public byte Manufacturer { get; set; }
        public byte Version { get; set; }
        public PCX_Encoding Encoding { get; set; }
        public byte BitsPerPixel { get; set; }

        public ushort XStart { get; set; }
        public ushort YStart { get; set; }
        public ushort XEnd { get; set; }
        public ushort YEnd { get; set; }

        public ushort HorizontalDPI { get; set; }
        public ushort VerticalDPI { get; set; }

        public RGB888Color[] EGAPalette { get; set; }

        public byte Reserved1 { get; set; }

        public byte BitPlaneCount { get; set; }
        public ushort BytesPerLine { get; set; }
        public ushort PaletteType { get; set; }
        public ushort HorScreenSize { get; set; }
        public ushort VerScreenSize { get; set; }

        public byte[] Reserved2 { get; set; }

        public byte[][] ScanLines { get; set; }
        public byte VGAPaletteStart { get; set; }
        public RGB888Color[] VGAPalette { get; set; }

        public int ImageWidth => XEnd - XStart + 1;
        public int ImageHeight => YEnd - YStart + 1;

        /// <summary>
        /// Converts the PCX data to a texture
        /// </summary>
        /// <returns>The texture</returns>
        public Texture2D ToTexture(bool flip = false)
        {
            // Create the texture
            var tex = TextureHelpers.CreateTexture2D(ImageWidth, ImageHeight);

            // Set every pixel
            for (int y = 0; y < ImageHeight; y++)
            {
                for (int x = 0; x < ImageWidth; x++)
                {
                    // Get the palette index
                    var paletteIndex = ScanLines[y][x];

                    // Set the pixel
                    tex.SetPixel(x, flip ? (ImageHeight - y - 1) : y, VGAPalette[paletteIndex].GetColor());
                }
            }

            // Apply the pixels
            tex.Apply();

            // Return the texture
            return tex;
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the header
            Manufacturer = s.Serialize<byte>(Manufacturer, name: nameof(Manufacturer));

            if (Manufacturer != 0x0A)
                throw new BinarySerializableException(this, $"The PCX {nameof(Manufacturer)} value is invalid. Value is {Manufacturer}. Expected value is 10.");

            Version = s.Serialize<byte>(Version, name: nameof(Version));
            Encoding = s.Serialize<PCX_Encoding>(Encoding, name: nameof(Encoding));
            BitsPerPixel = s.Serialize<byte>(BitsPerPixel, name: nameof(BitsPerPixel));
            XStart = s.Serialize<ushort>(XStart, name: nameof(XStart));
            YStart = s.Serialize<ushort>(YStart, name: nameof(YStart));
            XEnd = s.Serialize<ushort>(XEnd, name: nameof(XEnd));
            YEnd = s.Serialize<ushort>(YEnd, name: nameof(YEnd));
            HorizontalDPI = s.Serialize<ushort>(HorizontalDPI, name: nameof(HorizontalDPI));
            VerticalDPI = s.Serialize<ushort>(VerticalDPI, name: nameof(VerticalDPI));
            EGAPalette = s.SerializeObjectArray<RGB888Color>(EGAPalette, 16, name: nameof(EGAPalette));
            Reserved1 = s.Serialize<byte>(Reserved1, name: nameof(Reserved1));
            BitPlaneCount = s.Serialize<byte>(BitPlaneCount, name: nameof(BitPlaneCount));
            BytesPerLine = s.Serialize<ushort>(BytesPerLine, name: nameof(BytesPerLine));
            PaletteType = s.Serialize<ushort>(PaletteType, name: nameof(PaletteType));
            HorScreenSize = s.Serialize<ushort>(HorScreenSize, name: nameof(HorScreenSize));
            VerScreenSize = s.Serialize<ushort>(VerScreenSize, name: nameof(VerScreenSize));
            Reserved2 = s.SerializeArray<byte>(Reserved2, 54, name: nameof(Reserved2));

            // Calculate properties
            var scanLineLength = BitPlaneCount * BytesPerLine;
            var linePaddingSize = scanLineLength * (8 / BitsPerPixel) - ImageWidth;

            // Create the scan-line array
            if (ScanLines == null)
                ScanLines = new byte[ImageHeight][];

            // Read every scan-line
            for (int i = 0; i < ImageHeight; i++)
            {
                // Serialize the scan-line using the PCX RLE encoding
                ScanLines[i] = s.DoEncodedIf(new PCX_Encoder(scanLineLength), Encoding == PCX_Encoding.RLE, () => s.SerializeArray(ScanLines[i], scanLineLength, name: $"{nameof(ScanLines)}[{i}]"));

                // Serialize padding
                s.SerializePadding(linePaddingSize);
            }

            // If the version is not 5 or if there is no more data to read we return
            if (Version != 5 || s.CurrentPointer.FileOffset >= s.CurrentLength)
                return;

            // Attempt to serialize the initial palette byte
            VGAPaletteStart = s.Serialize<byte>(VGAPaletteStart, name: nameof(VGAPaletteStart));

            // If the byte is not valid we go back and return
            if (VGAPaletteStart != 0x0C)
            {
                s.Goto(s.CurrentPointer - 1);
                return;
            }

            // Serialize the palette
            VGAPalette = s.SerializeObjectArray<RGB888Color>(VGAPalette, 256, name: nameof(VGAPalette));
        }
    }
}