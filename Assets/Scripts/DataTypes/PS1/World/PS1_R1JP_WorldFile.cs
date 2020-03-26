using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// World data for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class PS1_R1JP_WorldFile : PS1_R1_BaseFile
    {
        /// <summary>
        /// The pointer to the first block
        /// </summary>
        public Pointer FirstBlockPointer => BlockPointers[0];

        /// <summary>
        /// The pointer to the second block
        /// </summary>
        public Pointer SecondBlockPointer => BlockPointers[1];

        /// <summary>
        /// The pointer to the event texture block
        /// </summary>
        public Pointer EventTexturesBlockPointer => BlockPointers[2];

        /// <summary>
        /// The pointer to the fourth block
        /// </summary>
        public Pointer FourthBlockPointer => BlockPointers[3];

        /// <summary>
        /// The pointer to the fifth block
        /// </summary>
        public Pointer UnknownPaletteBlockPointer => BlockPointers[4];

        /// <summary>
        /// The pointer to the tiles block
        /// </summary>
        public Pointer TilesBlockPointer => BlockPointers[5];

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FirstBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] SecondBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] EventTexturesBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FourthBlock { get; set; }

        public RGB555Color[] UnknownPalette { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public RGB555Color[][] Tiles { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            // HEADER

            base.SerializeImpl(s);

            // BLOCK 1
            s.DoAt(FirstBlockPointer, () => {
                FirstBlock = s.SerializeArray<byte>(FirstBlock, SecondBlockPointer - s.CurrentPointer, name: nameof(FirstBlock));
            });

            // BLOCK 2
            s.DoAt(SecondBlockPointer, () => {
                SecondBlock = s.SerializeArray<byte>(SecondBlock, EventTexturesBlockPointer - s.CurrentPointer, name: nameof(SecondBlock));
            });

            // EVENT TEXTURES
            s.DoAt(EventTexturesBlockPointer, () => {
                EventTexturesBlock = s.SerializeArray<byte>(EventTexturesBlock, FourthBlockPointer - s.CurrentPointer, name: nameof(EventTexturesBlock));
            });

            // BLOCK 4
            s.DoAt(FourthBlockPointer, () => {
                FourthBlock = s.SerializeArray<byte>(FourthBlock, UnknownPaletteBlockPointer - s.CurrentPointer, name: nameof(FourthBlock));
            });

            // UNKNOWN PALETTE
            s.DoAt(UnknownPaletteBlockPointer, () => {
                UnknownPalette = s.SerializeObjectArray<RGB555Color>(UnknownPalette, 256, name: nameof(UnknownPalette));
            });

            // TILES
            s.DoAt(TilesBlockPointer, () => {
                // TODO: Find a better way to know the number of tiles
                uint cellSize = PS1_Manager.CellSize * PS1_Manager.CellSize;
                uint numTiles = (FileSize - s.CurrentPointer.FileOffset) / cellSize / 2;
                // Create & serialize the tiles array
                Tiles = new RGB555Color[numTiles][];
                for (int i = 0; i < Tiles.Length; i++)
                    Tiles[i] = s.SerializeObjectArray<RGB555Color>(Tiles[i], cellSize, name: nameof(Tiles) + "[" + i + "]");
            });
        }
    }
}