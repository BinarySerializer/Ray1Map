using System;
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

        // TODO: This is a temp property until we serialize the actual data
        public byte[] EventBlock { get; set; }

        /// <summary>
        /// The map width, in tiles
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The map height, in tiles
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public PS1_R1_MapTile[] Tiles { get; set; }

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

            EventBlock = stream.ReadBytes((int)(MapBlockPointer - stream.Position));

            // MAP BLOCK

            // Read map size
            Width = stream.Read<ushort>();
            Height = stream.Read<ushort>();

            // Read tiles
            Tiles = stream.Read<PS1_R1_MapTile>((ulong)(Width * Height));

            // TEXTURE BLOCK

            // TODO: Read
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