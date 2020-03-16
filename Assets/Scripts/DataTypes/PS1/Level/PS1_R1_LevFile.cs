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

        // TODO: This is a temp property until we serialize the actual data - this block has a texture sheet with a width of 256
        public byte[] TextureBlock { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            // HEADER

            base.Serialize(serializer);

            // BACKGROUND BLOCK

            // At this point the stream position should match the background block offset
            if (serializer.BaseStream.Position != BackgroundBlockPointer)
                Debug.LogError("Background block offset is incorrect");

            // Serialize the background layer information (always 12)
            serializer.SerializeArray<PS1_R1_BackgroundLayerPosition>(nameof(BackgroundLayerPositions), 12);
            serializer.SerializeArray<byte>(nameof(Unknown3), 16);
            serializer.SerializeArray<PS1_R1_BackgroundLayerInfo>(nameof(BackgroundLayerInfos), 12);
            // On PAL/NTSC this is 80 bytes. On NTSC-J it's more, which is why we just serialize the remaining bytes for now
            serializer.SerializeArray<byte>(nameof(Unknown4), EventBlockPointer - serializer.BaseStream.Position);

            // EVENT BLOCK

            // At this point the stream position should match the event block offset
            if (serializer.BaseStream.Position != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            // Serialize header
            serializer.Serialize(nameof(Unknown1));
            serializer.Serialize(nameof(EventCount));
            serializer.Serialize(nameof(Unknown2));
            serializer.Serialize(nameof(EventLinkCount));

            if (EventCount != EventLinkCount)
                Debug.LogError("Event counts don't match");

            // Serialize every event
            serializer.SerializeArray<PS1_R1_Event>(nameof(Events), EventCount);

            // Serialize the event linking table
            serializer.SerializeArray<byte>(nameof(EventLinkingTable), EventLinkCount);

            serializer.SerializeArray<byte>(nameof(EventBlock), MapBlockPointer - serializer.BaseStream.Position);

            // MAP BLOCK

            // At this point the stream position should match the map block offset
            if (serializer.BaseStream.Position != MapBlockPointer)
                Debug.LogError("Map block offset is incorrect");

            // Serialize map size
            serializer.Serialize(nameof(Width));
            serializer.Serialize(nameof(Height));

            // Serialize tiles
            serializer.SerializeArray<PS1_R1_MapTile>(nameof(Tiles), Width * Height);

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (serializer.BaseStream.Position != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            serializer.SerializeArray<byte>(nameof(TextureBlock), FileSize - TextureBlockPointer);

            // At this point the stream position should match the end offset
            if (serializer.BaseStream.Position != FileSize)
                Debug.LogError("End offset is incorrect");
        }
    }
}