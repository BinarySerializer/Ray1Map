using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PS1)
    /// </summary>
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

        /// <summary>
        /// Pointer to the events
        /// </summary>
        public Pointer EventsPointer { get; set; }

        /// <summary>
        /// The amount of events in the file
        /// </summary>
        public uint EventCount { get; set; }

        /// <summary>
        /// Pointer to the event links
        /// </summary>
        public Pointer EventLinksPointer { get; set; }

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
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // HEADER
            base.SerializeImpl(s);

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
            s.DoAt(EventBlockPointer, () => 
            {
                // Serialize header
                EventsPointer = s.SerializePointer(EventsPointer, name: "EventsPointer");
                EventCount = s.Serialize<uint>(EventCount, name: "EventCount");
                EventLinksPointer = s.SerializePointer(EventLinksPointer, name: "EventLinksPointer");
                EventLinkCount = s.Serialize<uint>(EventLinkCount, name: "EventLinkCount");

                if (EventCount != EventLinkCount)
                    Debug.LogError("Event counts don't match");

                s.DoAt(EventsPointer, (() =>
                {
                    // Serialize every event
                    Events = s.SerializeObjectArray<PS1_R1_Event>(Events, EventCount, name: "Events");
                }));

                s.DoAt(EventLinksPointer, (() =>
                {
                    // Serialize the event linking table
                    EventLinkingTable = s.SerializeArray<byte>(EventLinkingTable, EventLinkCount, name: "EventLinkingTable");
                }));
            });

            // MAP BLOCK
            s.DoAt(MapBlockPointer, () => 
            {
                // Serialize map size
                Width = s.Serialize<ushort>(Width, name: "Width");
                Height = s.Serialize<ushort>(Height, name: "Height");

                // Serialize tiles
                Tiles = s.SerializeObjectArray<PS1_R1_MapTile>(Tiles, Width * Height, name: "Tiles");
            });

            // TEXTURE BLOCK
            s.DoAt(TextureBlockPointer, () => 
            {
                TextureBlock = s.SerializeArray<byte>(TextureBlock, FileSize - TextureBlockPointer.FileOffset, name: "TextureBlock");
            });
        }
    }
}