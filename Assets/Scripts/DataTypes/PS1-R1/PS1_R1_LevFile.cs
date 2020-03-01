using System;
using System.Collections.Generic;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_LevFile : ISerializableFile
    {
        /// <summary>
        /// The pointer to the offset block
        /// </summary>
        public uint OffsetBlockPointer { get; set; }

        /// <summary>
        /// The pointer to the background block
        /// </summary>
        public uint BackgroundBlockPointer { get; set; }

        /// <summary>
        /// The pointer to the event block
        /// </summary>
        public uint EventBlockPointer { get; set; }

        /// <summary>
        /// The pointer to the map block
        /// </summary>
        public uint MapBlockPointer { get; set; }

        /// <summary>
        /// The pointer to the texture block
        /// </summary>
        public uint TextureBlockPointer { get; set; }

        /// <summary>
        /// The pointer to the end of the file
        /// </summary>
        public uint FileEndPointer { get; set; }

        // TODO: This is a temp property until we serialize the actual data
        public byte[] BackgroundBlock { get; set; }

        public uint Unknown1 { get; set; }

        /// <summary>
        /// The amount of events in the file
        /// </summary>
        public uint EventCount { get; set; }

        public uint Unknown2 { get; set; }

        // NOTE: Always the same as EventCount
        public uint Unknown3 { get; set; }

        /// <summary>
        /// The events
        /// </summary>
        public PS1_R1_Event[] Events { get; set; }




        // TODO: Below here are old values which are still currently being used

        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public PS1_R1_MapTile[] Tiles { get; set; }

        // TODO: Remove?
        public PxlVec RaymanPos { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            // Read the offset block pointer
            OffsetBlockPointer = stream.Read<uint>();

            // OFFSET BLOCK

            // Read the pointers
            BackgroundBlockPointer = stream.Read<uint>();
            EventBlockPointer = stream.Read<uint>();
            MapBlockPointer = stream.Read<uint>();
            TextureBlockPointer = stream.Read<uint>();
            FileEndPointer = stream.Read<uint>();

            // BACKGROUND BLOCK

            BackgroundBlock = stream.ReadBytes((int)(EventBlockPointer - BackgroundBlockPointer));

            // EVENT BLOCK

            // Read header
            Unknown1 = stream.Read<uint>();
            EventCount = stream.Read<uint>();
            Unknown2 = stream.Read<uint>();
            Unknown3 = stream.Read<uint>();

            // Read every event
            Events = stream.Read<PS1_R1_Event>(EventCount);

            // MAP BLOCK

            // TODO: Read

            // TEXTURE BLOCK

            // TODO: Read




            stream.Position = 0;
            // TODO: Everything below is the old current code which is being changed

            var XXX = stream.ReadBytes((int)stream.Length);

            int off_types = BitConverter.ToInt32(XXX, 0xC);

            Width = BitConverter.ToUInt16(XXX, off_types);
            Height = BitConverter.ToUInt16(XXX, off_types + 2);

            int i = off_types + 4;
            Tiles = new PS1_R1_MapTile[Width * Height];

            for (int n = 0; n < Width * Height; n++, i += 2)
            {
                Tiles[n] = new PS1_R1_MapTile();
                int g = XXX[i] + ((XXX[i + 1] & 3) << 8);
                Tiles[n].gX = g & 15;
                Tiles[n].gY = g >> 4;
                Tiles[n].col = (TileCollisionType)(XXX[i + 1] >> 2);
            }

            // hack get rayman pos
            for (int b = 0; b + 4 < XXX.Length; b++)
                if (XXX[b] == 0x07 && XXX[b + 1] == 0x63 && XXX[b + 2] == 0 && XXX[b + 3] == 0)
                {
                    RaymanPos = new PxlVec(
                        BitConverter.ToUInt16(XXX, b - 0x46),
                        BitConverter.ToUInt16(XXX, b - 0x44));
                    break;
                }
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}