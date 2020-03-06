using System.ComponentModel;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1)
    /// </summary>
    [Description("Rayman 1 (PS1) Level File")]
    public class PS1_R1_LevFile : PS1_R1_BaseFile
    {
        /// <summary>
        /// The pointer to the background block
        /// </summary>
        public uint BackgroundBlockPointer => Pointers[0];

        /// <summary>
        /// The pointer to the event block
        /// </summary>
        public uint EventBlockPointer => Pointers[1];

        /// <summary>
        /// The pointer to the map block
        /// </summary>
        public uint MapBlockPointer => Pointers[2];

        /// <summary>
        /// The pointer to the texture block
        /// </summary>
        public uint TextureBlockPointer => Pointers[3];

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
        /// <param name="deserializer">The deserializer</param>
        public override void Deserialize(BinaryDeserializer deserializer)
        {
            // HEADER

            base.Deserialize(deserializer);

            // BACKGROUND BLOCK

            // At this point the stream position should match the background block offset
            if (deserializer.BaseStream.Position != BackgroundBlockPointer)
                Debug.LogError("Background block offset is incorrect");

            // Read the background layer information (always 12)
            BackgroundLayerPositions = deserializer.Read<PS1_R1_BackgroundLayerPosition>(12);
            Unknown3 = deserializer.Read<byte>(16);
            BackgroundLayerInfos = deserializer.Read<PS1_R1_BackgroundLayerInfo>(12);
            // On PAL/NTSC this is 80 bytes. On NTSC-J it's more, which is why we just read the remaining bytes for now
            Unknown4 = deserializer.Read<byte>((ulong)(EventBlockPointer - deserializer.BaseStream.Position));

            // EVENT BLOCK

            // At this point the stream position should match the event block offset
            if (deserializer.BaseStream.Position != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            // Read header
            Unknown1 = deserializer.Read<uint>();
            EventCount = deserializer.Read<uint>();
            Unknown2 = deserializer.Read<uint>();
            EventLinkCount = deserializer.Read<uint>();

            if (EventCount != EventLinkCount)
                Debug.LogError("Event counts don't match");

            // Read every event
            Events = deserializer.Read<PS1_R1_Event>(EventCount);

            // Read the event linking table
            EventLinkingTable = deserializer.Read<byte>(EventLinkCount);

            EventBlock = deserializer.Read<byte>((ulong)(MapBlockPointer - deserializer.BaseStream.Position));

            // MAP BLOCK

            // At this point the stream position should match the map block offset
            if (deserializer.BaseStream.Position != MapBlockPointer)
                Debug.LogError("Map block offset is incorrect");

            // Read map size
            Width = deserializer.Read<ushort>();
            Height = deserializer.Read<ushort>();

            // Read tiles
            Tiles = deserializer.Read<PS1_R1_MapTile>((ulong)(Width * Height));

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (deserializer.BaseStream.Position != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            TextureBlock = deserializer.Read<byte>(FileSize - TextureBlockPointer);

            // At this point the stream position should match the end offset
            if (deserializer.BaseStream.Position != FileSize)
                Debug.LogError("End offset is incorrect");

            Debug.Log($"PS1 R1 level loaded with size {Width}x{Height} and {EventCount} events");
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            // HEADER

            base.Serialize(serializer);

            // BACKGROUND BLOCK

            // Write the background layer information (always 12)
            serializer.Write(BackgroundLayerPositions);
            serializer.Write(Unknown3);
            serializer.Write(BackgroundLayerInfos);
            serializer.Write(Unknown4);

            // EVENT BLOCK

            // Read header
            serializer.Write(Unknown1);
            serializer.Write(EventCount);
            serializer.Write(Unknown2);
            serializer.Write(EventLinkCount);

            // Read every event
            serializer.Write(Events);

            // Write the event linking table
            serializer.Write(EventLinkingTable);

            serializer.Write(EventBlock);

            // MAP BLOCK

            // Read map size
            serializer.Write(Width);
            serializer.Write(Height);

            // Read tiles
            serializer.Write(Tiles);

            // TEXTURE BLOCK

            serializer.Write(TextureBlock);
        }
    }
}