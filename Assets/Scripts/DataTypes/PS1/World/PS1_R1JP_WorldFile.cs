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
        public uint FirstBlockPointer => Pointers[0];

        /// <summary>
        /// The pointer to the second block
        /// </summary>
        public uint SecondBlockPointer => Pointers[1];

        /// <summary>
        /// The pointer to the event texture block
        /// </summary>
        public uint EventTexturesBlockPointer => Pointers[2];

        /// <summary>
        /// The pointer to the fourth block
        /// </summary>
        public uint FourthBlockPointer => Pointers[3];

        /// <summary>
        /// The pointer to the fifth block
        /// </summary>
        public uint UnknownPaletteBlockPointer => Pointers[4];

        /// <summary>
        /// The pointer to the tiles block
        /// </summary>
        public uint TilesBlockPointer => Pointers[5];

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FirstBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] SecondBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] EventTexturesBlock { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] FourthBlock { get; set; }

        public ARGB1555Color[] UnknownPalette { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public ARGB1555Color[][] Tiles { get; set; }

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

            serializer.SerializeArray<byte>(nameof(SecondBlock), EventTexturesBlockPointer - serializer.BaseStream.Position);

            // EVENT TEXTURES

            serializer.SerializeArray<byte>(nameof(EventTexturesBlock), FourthBlockPointer - serializer.BaseStream.Position);

            // BLOCK 4

            serializer.SerializeArray<byte>(nameof(FourthBlock), UnknownPaletteBlockPointer - serializer.BaseStream.Position);

            // UNKNOWN PALETTE

            serializer.SerializeArray<ARGB1555Color>(nameof(UnknownPalette), 256);

            // TILES

            // At this point the stream position should match the tiles block offset
            if (serializer.BaseStream.Position != TilesBlockPointer)
                Debug.LogError("Tiles block offset is incorrect");

            if (serializer.Mode == SerializerMode.Read)
            {
                // Create the tiles array
                Tiles = new ARGB1555Color[(FileSize - serializer.BaseStream.Position) / (PS1_Manager.CellSize * PS1_Manager.CellSize * 2)][];

                // Read every tile
                for (int i = 0; i < Tiles.Length; i++)
                    Tiles[i] = serializer.ReadArray<ARGB1555Color>(PS1_Manager.CellSize * PS1_Manager.CellSize);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}