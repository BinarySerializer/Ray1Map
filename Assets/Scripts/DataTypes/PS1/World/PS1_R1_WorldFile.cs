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
        /// The pointer to the event palette 1 block
        /// </summary>
        public uint EventPalette1BlockPointer => Pointers[3];

        /// <summary>
        /// The pointer to the event palette 2 block
        /// </summary>
        public uint EventPalette2BlockPointer => Pointers[4];

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

        /// <summary>
        /// The event palette
        /// </summary>
        public ARGB1555Color[] EventPalette1 { get; set; }

        /// <summary>
        /// The event palette
        /// </summary>
        public ARGB1555Color[] EventPalette2 { get; set; }

        /// <summary>
        /// The tiles palette index table
        /// </summary>
        public byte[] TilesIndexTable { get; set; }

        /// <summary>
        /// The tile color palettes
        /// </summary>
        public ARGB1555Color[][] TileColorPalettes { get; set; }

        /// <summary>
        /// The tile palette index table
        /// </summary>
        public byte[] TilePaletteIndexTable { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public override void Deserialize(BinaryDeserializer deserializer)
        {
            // HEADER

            base.Deserialize(deserializer);

            // BLOCK 1

            FirstBlock = deserializer.ReadArray<byte>((ulong)(SecondBlockPointer - deserializer.BaseStream.Position));

            // BLOCK 2

            SecondBlock = deserializer.ReadArray<byte>((ulong)(ThirdBlockPointer - deserializer.BaseStream.Position));

            // BLOCK 3

            ThirdBlock = deserializer.ReadArray<byte>((ulong)(EventPalette1BlockPointer - deserializer.BaseStream.Position));

            // EVENT PALETTE 1

            EventPalette1 = deserializer.ReadArray<ARGB1555Color>(256);

            // EVENT PALETTE 2

            EventPalette2 = deserializer.ReadArray<ARGB1555Color>(256);

            // TILES

            // At this point the stream position should match the tiles block offset
            if (deserializer.BaseStream.Position != TilesBlockPointer)
                Debug.LogError("Tiles block offset is incorrect");

            // Read the tiles index table
            TilesIndexTable = deserializer.ReadArray<byte>((ulong)(PaletteBlockPointer - deserializer.BaseStream.Position));

            // TILE PALETTES

            // At this point the stream position should match the palette block offset
            if (deserializer.BaseStream.Position != PaletteBlockPointer)
                Debug.LogError("Palette block offset is incorrect");

            // Create the palettes
            var palettes = new List<ARGB1555Color[]>();

            // TODO: Find way to know the number of palettes
            while (deserializer.BaseStream.Position < PaletteIndexBlockPointer)
                // Read and add to the palettes
                palettes.Add(deserializer.ReadArray<ARGB1555Color>(256));

            // Set the palettes
            TileColorPalettes = palettes.ToArray();

            // TILE PALETTE ASSIGN

            // At this point the stream position should match the palette assign block offset
            if (deserializer.BaseStream.Position != PaletteIndexBlockPointer)
                Debug.LogError("Palette assign block offset is incorrect");

            // Read the palette index table
            TilePaletteIndexTable = deserializer.ReadArray<byte>((ulong)(FileSize - deserializer.BaseStream.Position));

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