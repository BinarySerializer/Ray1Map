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
        public Pointer FirstBlockPointer => BlockPointers[0];

        /// <summary>
        /// The pointer to the second block
        /// </summary>
        public Pointer SecondBlockPointer => BlockPointers[1];

        /// <summary>
        /// The pointer to the third block. This block is always 0 bytes.
        /// </summary>
        public Pointer ThirdBlockPointer => BlockPointers[2];

        /// <summary>
        /// The pointer to the event palette 1 block
        /// </summary>
        public Pointer EventPalette1BlockPointer => BlockPointers[3];

        /// <summary>
        /// The pointer to the event palette 2 block
        /// </summary>
        public Pointer EventPalette2BlockPointer => BlockPointers[4];

        /// <summary>
        /// The pointer to the tiles block
        /// </summary>
        public Pointer TilesBlockPointer => BlockPointers[5];

        /// <summary>
        /// The pointer to the palette block
        /// </summary>
        public Pointer PaletteBlockPointer => BlockPointers[6];

        /// <summary>
        /// The pointer to the palette index block
        /// </summary>
        public Pointer PaletteIndexBlockPointer => BlockPointers[7];

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
        public override void SerializeImpl(SerializerObject s) {
            // HEADER
            base.SerializeImpl(s);

            // BLOCK 1
            s.DoAt(FirstBlockPointer, () => {
                FirstBlock = s.SerializeArray(FirstBlock, SecondBlockPointer - s.CurrentPointer, name: "FirstBlock");
            });

            // BLOCK 2
            s.DoAt(SecondBlockPointer, () => {
                SecondBlock = s.SerializeArray(SecondBlock, ThirdBlockPointer - s.CurrentPointer, name: "SecondBlock");
            });

            // BLOCK 3
            s.DoAt(ThirdBlockPointer, () => {
                ThirdBlock = s.SerializeArray(ThirdBlock, EventPalette1BlockPointer - s.CurrentPointer, name: "ThirdBlock");
            });

            // EVENT PALETTE 1
            s.DoAt(EventPalette1BlockPointer, () => {
                EventPalette1 = s.SerializeObjectArray(EventPalette1, 256, name: "EventPalette1");
            });

            // EVENT PALETTE 2
            s.DoAt(EventPalette2BlockPointer, () => {
                EventPalette2 = s.SerializeObjectArray(EventPalette2, 256, name: "EventPalette2");
            });

            // TILES
            s.DoAt(TilesBlockPointer, () => {
                // Read the tiles index table
                TilesIndexTable = s.SerializeArray(TilesIndexTable, PaletteBlockPointer - TilesBlockPointer, name: "TilesIndexTable");
            });

            // TILE PALETTES
            s.DoAt(PaletteBlockPointer, () => {
                // TODO: Find a better way to know the number of palettes
                uint numPalettes = (uint)(PaletteIndexBlockPointer - PaletteBlockPointer) / (256 * 2);
                if (TileColorPalettes == null) {
                    TileColorPalettes = new ARGB1555Color[numPalettes][];
                }
                for (int i = 0; i < TileColorPalettes.Length; i++) {
                    TileColorPalettes[i] = s.SerializeObjectArray(TileColorPalettes[i], 256, name: "TileColorPalettes[" + i + "]");
                }
            });

            // TILE PALETTE ASSIGN
            s.DoAt(PaletteIndexBlockPointer, () => {
                // Read the palette index table
                TilePaletteIndexTable = s.SerializeArray(TilePaletteIndexTable, FileSize - PaletteIndexBlockPointer.FileOffset, name: "TilePaletteIndexTable");
            });
        }
    }
}