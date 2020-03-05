using System;
using System.Collections.Generic;
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
        /// <param name="deserializer">The deserializer</param>
        public override void Deserialize(BinaryDeserializer deserializer)
        {
            // HEADER

            base.Deserialize(deserializer);

            // BLOCK 1

            FirstBlock = deserializer.ReadBytes((int)(SecondBlockPointer - deserializer.BaseStream.Position));

            // BLOCK 2

            SecondBlock = deserializer.ReadBytes((int)(ThirdBlockPointer - deserializer.BaseStream.Position));

            // BLOCK 3

            ThirdBlock = deserializer.ReadBytes((int)(FourthBlockPointer - deserializer.BaseStream.Position));

            // BLOCK 4

            FourthBlock = deserializer.ReadBytes((int)(FifthBlockPointer - deserializer.BaseStream.Position));

            // BLOCK 5

            FifthBlock = deserializer.ReadBytes((int)(TilesBlockPointer - deserializer.BaseStream.Position));

            // TILES

            // At this point the stream position should match the tiles block offset
            if (deserializer.BaseStream.Position != TilesBlockPointer)
                Debug.LogError("Tiles block offset is incorrect");

            // Read the tiles index table
            TilesIndexTable = deserializer.ReadBytes((int)(PaletteBlockPointer - deserializer.BaseStream.Position));

            // PALETTE

            // At this point the stream position should match the palette block offset
            if (deserializer.BaseStream.Position != PaletteBlockPointer)
                Debug.LogError("Palette block offset is incorrect");

            // Create the palettes
            var palettes = new List<ARGBColor[]>();

            // TODO: Find way to know the number of palettes
            while (deserializer.BaseStream.Position < PaletteIndexBlockPointer)
            {
                // Create the palette
                var palette = new ARGBColor[256];

                // Read each color
                for (int i = 0; i < palette.Length; i++)
                {
                    // Read the color value (BGR 1555)
                    uint colour16 = deserializer.Read<ushort>();

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
            if (deserializer.BaseStream.Position != PaletteIndexBlockPointer)
                Debug.LogError("Palette assign block offset is incorrect");

            // Read the palette index table
            PaletteIndexTable = deserializer.ReadBytes((int)(FileSize - deserializer.BaseStream.Position));

            // At this point the stream position should match the end offset
            if (deserializer.BaseStream.Position != FileSize)
                Debug.LogError("End offset is incorrect");
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            // HEADER

            base.Serialize(serializer);

            throw new NotImplementedException();
        }
    }
}