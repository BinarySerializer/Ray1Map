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

            SecondBlock = deserializer.ReadArray<byte>((ulong)(EventTexturesBlockPointer - deserializer.BaseStream.Position));

            // EVENT TEXTURES

            EventTexturesBlock = deserializer.ReadArray<byte>((ulong)(FourthBlockPointer - deserializer.BaseStream.Position));

            // BLOCK 4

            FourthBlock = deserializer.ReadArray<byte>((ulong)(UnknownPaletteBlockPointer - deserializer.BaseStream.Position));

            // UNKNOWN PALETTE

            UnknownPalette = deserializer.ReadArray<ARGB1555Color>(256);

            // TILES

            // At this point the stream position should match the tiles block offset
            if (deserializer.BaseStream.Position != TilesBlockPointer)
                Debug.LogError("Tiles block offset is incorrect");

            // Create the tiles array
            Tiles = new ARGB1555Color[(FileSize - deserializer.BaseStream.Position) / (PS1_R1_Manager.CellSize * PS1_R1_Manager.CellSize * 2)][];

            // Read every tile
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = deserializer.ReadArray<ARGB1555Color>(PS1_R1_Manager.CellSize * PS1_R1_Manager.CellSize);
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