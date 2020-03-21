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
        public Pointer BackgroundBlockPointer => BlockPointers[0];

        /// <summary>
        /// The pointer to the event block
        /// </summary>
        public Pointer EventBlockPointer => BlockPointers[1];

        /// <summary>
        /// The pointer to the map block
        /// </summary>
        public Pointer MapBlockPointer => BlockPointers[2];

        /// <summary>
        /// The pointer to the texture block
        /// </summary>
        public Pointer TextureBlockPointer => BlockPointers[3];

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
        public override void SerializeImpl(SerializerObject s) {
            // HEADER

            base.SerializeImpl(s);

            // At this point the stream position should match the background block offset
            if (s.CurrentPointer != BackgroundBlockPointer)
                Debug.LogError("Background block offset is incorrect");

            // BACKGROUND BLOCK
            s.DoAt(BackgroundBlockPointer, () => {
                // Serialize the background layer information (always 12)
                BackgroundLayerPositions = s.SerializeObjectArray<PS1_R1_BackgroundLayerPosition>(BackgroundLayerPositions, 12, name: "BackgroundLayerPositions");
                Unknown3 = s.SerializeArray<byte>(Unknown3, 16, name: "Unknown3");
                BackgroundLayerInfos = s.SerializeObjectArray<PS1_R1_BackgroundLayerInfo>(BackgroundLayerInfos, 12, name: "BackgroundLayerInfos");
                // On PAL/NTSC this is 80 bytes. On NTSC-J it's more, which is why we just serialize the remaining bytes for now
                Unknown4 = s.SerializeArray<byte>(Unknown4, EventBlockPointer - s.CurrentPointer, name: "Unknown4");
            });

            // EVENT BLOCK
            s.DoAt(EventBlockPointer, () => {
                // Serialize header
                Unknown1 = s.Serialize(Unknown1, name: "Unknown1");
                EventCount = s.Serialize(EventCount, name: "EventCount");
                Unknown2 = s.Serialize(Unknown2, name: "Unknown2");
                EventLinkCount = s.Serialize(EventLinkCount, name: "EventLinkCount");

                if (EventCount != EventLinkCount)
                    Debug.LogError("Event counts don't match");

                // Serialize every event
                Events = s.SerializeObjectArray<PS1_R1_Event>(Events, EventCount, name: "Events");

                // Serialize the event linking table
                EventLinkingTable = s.SerializeArray<byte>(EventLinkingTable, EventLinkCount, name: "EventLinkingTable");

                EventBlock = s.SerializeArray<byte>(EventBlock, MapBlockPointer - s.CurrentPointer, name: "EventBlock");
            });

            // MAP BLOCK
            s.DoAt(MapBlockPointer, () => {
                // Serialize map size
                Width = s.Serialize(Width, name: "Width");
                Height = s.Serialize(Height, name: "Height");

                // Serialize tiles
                Tiles = s.SerializeObjectArray<PS1_R1_MapTile>(Tiles, Width * Height, name: "Tiles");

                // At this point the stream position should match the texture block offset
                if (s.CurrentPointer != TextureBlockPointer)
                    Debug.LogError("Texture block offset is incorrect");
            });

            // TEXTURE BLOCK
            s.DoAt(TextureBlockPointer, () => {
                TextureBlock = s.SerializeArray<byte>(TextureBlock, FileSize - TextureBlockPointer.FileOffset, name: "TextureBlock");

                // At this point the stream position should match the end offset
                if (s.CurrentPointer.FileOffset != FileSize)
                    Debug.LogError("End offset is incorrect");
            });
        }
    }
}