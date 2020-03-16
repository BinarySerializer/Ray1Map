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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            // HEADER

            base.Serialize(serializer);

            // BLOCK 1

            serializer.SerializeArray<byte>(nameof(FirstBlock), SecondBlockPointer - serializer.BaseStream.Position);

            // BLOCK 2

            serializer.SerializeArray<byte>(nameof(SecondBlock), ThirdBlockPointer - serializer.BaseStream.Position);

            // BLOCK 3

            serializer.SerializeArray<byte>(nameof(ThirdBlock), EventPalette1BlockPointer - serializer.BaseStream.Position);

            // EVENT PALETTE 1

            serializer.SerializeArray<ARGB1555Color>(nameof(EventPalette1), 256);

            // EVENT PALETTE 2

            serializer.SerializeArray<ARGB1555Color>(nameof(EventPalette2), 256);

            // TILES

            // At this point the stream position should match the tiles block offset
            if (serializer.BaseStream.Position != TilesBlockPointer)
                Debug.LogError("Tiles block offset is incorrect");

            // Read the tiles index table
            serializer.SerializeArray<byte>(nameof(TilesIndexTable), PaletteBlockPointer - serializer.BaseStream.Position);

            // TILE PALETTES

            // At this point the stream position should match the palette block offset
            if (serializer.BaseStream.Position != PaletteBlockPointer)
                Debug.LogError("Palette block offset is incorrect");

            if (serializer.Mode == SerializerMode.Read)
            {
                // Create the palettes
                var palettes = new List<ARGB1555Color[]>();

                // TODO: Find way to know the number of palettes
                while (serializer.BaseStream.Position < PaletteIndexBlockPointer)
                    // Read and add to the palettes
                    palettes.Add(serializer.ReadArray<ARGB1555Color>(256));

                // Set the palettes
                TileColorPalettes = palettes.ToArray();
            }
            else
            {
                throw new NotImplementedException();
            }

            // TILE PALETTE ASSIGN

            // At this point the stream position should match the palette assign block offset
            if (serializer.BaseStream.Position != PaletteIndexBlockPointer)
                Debug.LogError("Palette assign block offset is incorrect");

            // Read the palette index table
            serializer.SerializeArray<byte>(nameof(TilePaletteIndexTable), FileSize - serializer.BaseStream.Position);

            // At this point the stream position should match the end offset
            if (serializer.BaseStream.Position != FileSize)
                Debug.LogError("End offset is incorrect");
        }
    }
}