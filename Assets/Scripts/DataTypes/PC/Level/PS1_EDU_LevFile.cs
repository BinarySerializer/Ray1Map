using System;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Level data for EDU on PS1
    /// </summary>
    public class PS1_EDU_LevFile : PC_BaseFile
    {
        #region Public Properties
        public byte[] Header_Unk1 { get; set; }
        public byte[] Header_Unk2 { get; set; }
        public byte[] Header_Unk3_1 { get; set; }
        public byte[] Header_Unk3_2 { get; set; }

        /// <summary>
        /// The width of the map, in cells
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The height of the map, in cells
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The color palettes
        /// </summary>
        public RGB666Color[][] ColorPalettes { get; set; }

        /// <summary>
        /// Last Plan 1 Palette, always set to 2
        /// </summary>
        public byte LastPlan1Palette { get; set; }

        public uint Unk1_1 { get; set; }
        public byte[] Unk2 { get; set; }
        public uint Unk3 { get; set; }
        public uint Unk4 { get; set; }

        /// <summary>
        /// The amount of events
        /// </summary>
        public ushort EventCount { get; set; }

        public uint EventBlockSize { get; set; }
        public Pointer EventBlockPointer { get; set; }

        /// <summary>
        /// The events
        /// </summary>
        public PC_Event[] Events { get; set; }

        /// <summary>
        /// The event link table
        /// </summary>
        public ushort[] EventLinkTable { get; set; }

        public PC_EventCommand[] EventCommands { get; set; }

        // After event block
        public ushort[] EventNumCommands { get; set; }
        public ushort[] EventNumLabelOffsets { get; set; }
        public byte[] TileTextures { get; set; }
        public uint MapBlockSize { get; set; }
        public PC_MapTile[] MapTiles { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            // PC HEADER
            base.SerializeImpl(s);

            // HEADER BLOCK
            Header_Unk1 = s.SerializeArray<byte>(Header_Unk1, 0xD, name: nameof(Header_Unk1));
            Header_Unk2 = s.SerializeArray<byte>(Header_Unk2, 0x3, name: nameof(Header_Unk2));
            Header_Unk3_1 = s.SerializeArray<byte>(Header_Unk3_1, 0x18, name: nameof(Header_Unk3_1));
            Header_Unk3_2 = s.SerializeArray<byte>(Header_Unk3_2, 0x18, name: nameof(Header_Unk3_2));

            // Serialize map size
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            // Create the palettes if necessary
            if (ColorPalettes == null) {
                ColorPalettes = new RGB666Color[][]
                {
                    new RGB666Color[256],
                    new RGB666Color[256],
                    new RGB666Color[256],
                };
            }

            // Serialize each palette
            for (var paletteIndex = 0; paletteIndex < ColorPalettes.Length; paletteIndex++) {
                var palette = ColorPalettes[paletteIndex];
                ColorPalettes[paletteIndex] = s.SerializeObjectArray<RGB666Color>(palette, palette.Length, name: nameof(ColorPalettes) + "[" + paletteIndex + "]");
            }

            // Serialize unknown byte
            LastPlan1Palette = s.Serialize<byte>(LastPlan1Palette, name: nameof(LastPlan1Palette));

            // Serialize unknown bytes
            Unk1_1 = s.Serialize<uint>(Unk1_1, name: nameof(Unk1_1));
            Unk2 = s.SerializeArray<byte>(Unk2, 0x12C0, name: nameof(Unk2));

            // Serialize event block header
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));
            Unk4 = s.Serialize<uint>(Unk4, name: nameof(Unk4));
            EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));
            EventBlockSize = s.Serialize<uint>(EventBlockSize, name: nameof(EventBlockSize));
            EventBlockPointer = s.CurrentPointer;
            s.Goto(EventBlockPointer + EventBlockSize);

            // Unknown (event-related but outside of main event block)
            EventNumCommands = s.SerializeArray<ushort>(EventNumCommands, EventCount, name: nameof(EventNumCommands));
            EventNumLabelOffsets = s.SerializeArray<ushort>(EventNumLabelOffsets, EventCount, name: nameof(EventNumLabelOffsets));

            // After this comes the tiles. They are stored in a tileset, 512x256 (where each tile is 16px).
            TileTextures = s.SerializeArray<byte>(TileTextures, 512 * 256, name: nameof(TileTextures));

            // After this comes the map tiles, 6 bytes each until end of file
            MapBlockSize = s.Serialize<uint>(MapBlockSize, name: nameof(MapBlockSize));
            MapTiles = s.SerializeObjectArray<PC_MapTile>(MapTiles, MapBlockSize / 6, name: nameof(MapTiles));


            // Finally, read the events
            s.DoAt(EventBlockPointer, () => {
                int GetPosInEventBlock() {
                    int currentPos = (int)(s.CurrentPointer - EventBlockPointer);
                    return currentPos;
                }
                // Start of event block
                Events = s.SerializeObjectArray<PC_Event>(Events, EventCount, name: nameof(Events));

                s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, EventCount * 4).ToArray(), EventCount * 4, name: "Padding");
                if (EventCount % 2 != 0) {
                    int padding = 4;
                    s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, padding).ToArray(), padding, name: "Padding");
                }

                EventLinkTable = s.SerializeArray<ushort>(EventLinkTable, EventCount, name: nameof(EventLinkTable));

                if (EventCount % 2 == 0) {
                    int padding = 2;
                    s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, padding).ToArray(), padding, name: "Padding");
                }

                // After this comes the commands. They do not have any length specified like on PC. When label offsets are used they're separated using 0xCD twice.
                if (EventCommands == null) {
                    EventCommands = new PC_EventCommand[EventCount];
                }
                for (int i = 0; i < EventCount; i++) {
                    EventCommands[i] = new PC_EventCommand();
                    if (EventNumCommands[i] != 0) {
                        if (GetPosInEventBlock() % 4 != 0) {
                            int padding = 4 - GetPosInEventBlock() % 4;
                            s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, padding).ToArray(), padding, name: "Padding");
                        }
                        EventCommands[i].CommandLength = EventNumCommands[i];
                        EventCommands[i].Commands = s.SerializeObject<Common_EventCommandCollection>(EventCommands[i].Commands, name: nameof(PC_EventCommand.Commands));
                    } else {
                        EventCommands[i].Commands = new Common_EventCommandCollection() {
                            Commands = new Common_EventCommand[0]
                        };
                    }
                    if (EventNumLabelOffsets[i] != 0) {
                        if (GetPosInEventBlock() % 4 != 0) {
                            int padding = 4 - GetPosInEventBlock() % 4;
                            s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, padding).ToArray(), padding, name: "Padding");
                        }
                        EventCommands[i].LabelOffsetCount = EventNumLabelOffsets[i];
                        EventCommands[i].LabelOffsetTable = s.SerializeArray<ushort>(EventCommands[i].LabelOffsetTable, EventCommands[i].LabelOffsetCount, name: nameof(PC_EventCommand.LabelOffsetTable));
                    } else {
                        EventCommands[i].LabelOffsetTable = new ushort[0];
                    }
                }
            });
        }

        #endregion
    }
}