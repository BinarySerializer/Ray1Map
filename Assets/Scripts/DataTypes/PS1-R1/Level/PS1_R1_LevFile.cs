using System.ComponentModel;
using System.IO;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1)
    /// </summary>
    [Description("Rayman 1 (PS1) Level File")]
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

        /// <summary>
        /// The background layer positions
        /// </summary>
        public PS1_R1_BackgroundLayerPosition[] BackgroundLayerPositions { get; set; }

        public byte[] Unknown3 { get; set; }

        /// <summary>
        /// The background layer info items
        /// </summary>
        public PS1_R1_BackgroundLayerInfo[] BackgroundLayerInfos { get; set; }

        public byte[] Unknown4 { get; set; }

        public uint Unknown1 { get; set; }

        /// <summary>
        /// The amount of events in the file
        /// </summary>
        public uint EventCount { get; set; }

        public uint Unknown2 { get; set; }

        /// <summary>
        /// The amount of event links in the file
        /// </summary>
        public uint EventLinkCount { get; set; }

        /// <summary>
        /// The events
        /// </summary>
        public PS1_R1_Event[] Events { get; set; }

        /// <summary>
        /// Data table for event linking
        /// </summary>
        public byte[] EventLinkingTable { get; set; }

        // TODO: This is a temp property until we serialize the actual data - this appears to contain the event commands
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

        // TODO: This is a temp property until we serialize the actual data
        public byte[] TextureBlock { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            // Read the offset block pointer
            OffsetBlockPointer = stream.Read<uint>();

            // OFFSET BLOCK

            // At this point the stream position should match the offset block offset
            if (stream.Position != OffsetBlockPointer)
                Debug.LogError("Offset block offset is incorrect");

            // Read the pointers
            BackgroundBlockPointer = stream.Read<uint>();
            EventBlockPointer = stream.Read<uint>();
            MapBlockPointer = stream.Read<uint>();
            TextureBlockPointer = stream.Read<uint>();
            FileEndPointer = stream.Read<uint>();

            // BACKGROUND BLOCK

            // At this point the stream position should match the background block offset
            if (stream.Position != BackgroundBlockPointer)
                Debug.LogError("Background block offset is incorrect");

            // Read the background layer information (always 12)
            BackgroundLayerPositions = stream.Read<PS1_R1_BackgroundLayerPosition>(12);
            Unknown3 = stream.ReadBytes(16);
            BackgroundLayerInfos = stream.Read<PS1_R1_BackgroundLayerInfo>(12);
            Unknown4 = stream.ReadBytes(80);

            // EVENT BLOCK

            // At this point the stream position should match the event block offset
            if (stream.Position != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            // Read header
            Unknown1 = stream.Read<uint>();
            EventCount = stream.Read<uint>();
            Unknown2 = stream.Read<uint>();
            EventLinkCount = stream.Read<uint>();

            if (EventCount != EventLinkCount)
                Debug.LogError("Event counts don't match");

            // Read every event
            Events = stream.Read<PS1_R1_Event>(EventCount);

            // Read the event linking table
            EventLinkingTable = stream.ReadBytes((int)EventLinkCount);

            EventBlock = stream.ReadBytes((int)(MapBlockPointer - stream.Position));

            // MAP BLOCK

            // At this point the stream position should match the map block offset
            if (stream.Position != MapBlockPointer)
                Debug.LogError("Map block offset is incorrect");

            // Read map size
            Width = stream.Read<ushort>();
            Height = stream.Read<ushort>();

            // Read tiles
            Tiles = stream.Read<PS1_R1_MapTile>((ulong)(Width * Height));

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (stream.Position != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            TextureBlock = stream.ReadBytes((int)(FileEndPointer - TextureBlockPointer));

            // At this point the stream position should match the end offset
            if (stream.Position != FileEndPointer)
                Debug.LogError("End offset is incorrect");

            Debug.Log($"PS1 R1 level loaded with size {Width}x{Height} and {EventCount} events");
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            // Write the offset block pointer
            stream.Write(OffsetBlockPointer);

            // OFFSET BLOCK

            // Read the pointers
            stream.Write(BackgroundBlockPointer);
            stream.Write(EventBlockPointer);
            stream.Write(MapBlockPointer);
            stream.Write(TextureBlockPointer);
            stream.Write(FileEndPointer);

            // BACKGROUND BLOCK

            // Write the background layer information (always 12)
            stream.Write(BackgroundLayerPositions);
            stream.Write(Unknown3);
            stream.Write(BackgroundLayerInfos);
            stream.Write(Unknown4);

            // EVENT BLOCK

            // Read header
            stream.Write(Unknown1);
            stream.Write(EventCount);
            stream.Write(Unknown2);
            stream.Write(EventLinkCount);

            // Read every event
            stream.Write(Events);

            // Write the event linking table
            stream.Write(EventLinkingTable);

            stream.Write(EventBlock);

            // MAP BLOCK

            // Read map size
            stream.Write(Width);
            stream.Write(Height);

            // Read tiles
            stream.Write(Tiles);

            // TEXTURE BLOCK

            stream.Write(TextureBlock);
        }
    }
}