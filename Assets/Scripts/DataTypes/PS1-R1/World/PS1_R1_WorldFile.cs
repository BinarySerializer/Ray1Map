using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// World data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_WorldFile : PS1_R1_BaseFile
    {
        /// <summary>
        /// The pointer to the first block. This block is always 0 bytes.
        /// </summary>
        public uint FirstBlockPointer => Pointers[0];

        /// <summary>
        /// The pointer to the second block
        /// </summary>
        public uint SecondBlockPointer => Pointers[1];

        /// <summary>
        /// The pointer to the third block. This block is always 0 bytes.
        /// </summary>
        public uint ThirdBlockPointer => Pointers[2];

        /// <summary>
        /// The pointer to the fourth block
        /// </summary>
        public uint FourthBlockPointer => Pointers[3];

        /// <summary>
        /// The pointer to the fifth block
        /// </summary>
        public uint FifthBlockPointer => Pointers[4];

        /// <summary>
        /// The pointer to the tiles block
        /// </summary>
        public uint TilesBlockPointer => Pointers[5];

        /// <summary>
        /// The pointer to the palette block
        /// </summary>
        public uint PaletteBlockPointer => Pointers[6];

        /// <summary>
        /// The pointer to the palette index block
        /// </summary>
        public uint PaletteIndexBlockPointer => Pointers[7];

        // Empty
        public byte[] FirstBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] SecondBlock { get; set; }

        // Empty
        public byte[] ThirdBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FourthBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FifthBlock { get; set; }

        /// <summary>
        /// The tiles palette index table
        /// </summary>
        public byte[] TilesIndexTable { get; set; }

        /// <summary>
        /// The color palettes
        /// </summary>
        public ARGBColor[][] ColorPalettes { get; set; }

        /// <summary>
        /// The palette index table
        /// </summary>
        public byte[] PaletteIndexTable { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public override void Deserialize(Stream stream)
        {
            // HEADER

            base.Deserialize(stream);

            // BLOCK 1

            FirstBlock = stream.ReadBytes((int)(SecondBlockPointer - stream.Position));

            // BLOCK 2

            SecondBlock = stream.ReadBytes((int)(ThirdBlockPointer - stream.Position));

            // BLOCK 3

            ThirdBlock = stream.ReadBytes((int)(FourthBlockPointer - stream.Position));

            // BLOCK 4

            FourthBlock = stream.ReadBytes((int)(FifthBlockPointer - stream.Position));

            // BLOCK 5

            FifthBlock = stream.ReadBytes((int)(TilesBlockPointer - stream.Position));

            // TILES

            // At this point the stream position should match the tiles block offset
            if (stream.Position != TilesBlockPointer)
                Debug.LogError("Tiles block offset is incorrect");

            // Read the tiles index table
            TilesIndexTable = stream.ReadBytes((int)(PaletteBlockPointer - stream.Position));

            // PALETTE

            // At this point the stream position should match the palette block offset
            if (stream.Position != PaletteBlockPointer)
                Debug.LogError("Palette block offset is incorrect");

            // Create the palettes
            var palettes = new List<ARGBColor[]>();

            // TODO: Find way to know the number of palettes
            while (stream.Position < PaletteIndexBlockPointer)
            {
                // Create the palette
                var palette = new ARGBColor[256];

                // Read each color
                for (int i = 0; i < palette.Length; i++)
                {
                    // Read the color value (BGR 1555)
                    uint colour16 = stream.Read<ushort>();

                    byte a = 255;
                    byte r = (byte)((colour16 & 0x1F) << 3);
                    byte g = (byte)(((colour16 & 0x3E0) >> 5) << 3);
                    byte b = (byte)(((colour16 & 0x7C00) >> 10) << 3);

                    // TODO: There might be an opacity mask in the file we can apply instead?
                    if (r == 0 && g == 0 && b == 0)
                        a = 0;

                    // Add to the palette
                    palette[i] = new ARGBColor(a, r, g, b);
                }

                // Add to the palettes
                palettes.Add(palette);
            }

            // Set the palettes
            ColorPalettes = palettes.ToArray();

            // PALETTE ASSIGN

            // At this point the stream position should match the palette assign block offset
            if (stream.Position != PaletteIndexBlockPointer)
                Debug.LogError("Palette assign block offset is incorrect");

            // Read the palette index table
            PaletteIndexTable = stream.ReadBytes((int)(FileSize - stream.Position));

            // At this point the stream position should match the end offset
            if (stream.Position != FileSize)
                Debug.LogError("End offset is incorrect");
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public override void Serialize(Stream stream)
        {
            // HEADER

            base.Serialize(stream);

            throw new NotImplementedException();
        }
    }
}