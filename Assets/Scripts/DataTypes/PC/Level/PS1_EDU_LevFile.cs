using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Level data for EDU on PS1
    /// </summary>
    public class PS1_EDU_LevFile : R1Serializable
    {
        #region Public Properties

        public PC_KitLevelDefinesBlock LevelDefines { get; set; }

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

        // Leftover from the PC version - irrelevant here
        public uint[] LeftoverTextureOffsetTable { get; set; }

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
            // HEADER BLOCK
            LevelDefines = s.SerializeObject<PC_KitLevelDefinesBlock>(LevelDefines, name: nameof(LevelDefines));

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
            LeftoverTextureOffsetTable = s.SerializeArray<uint>(LeftoverTextureOffsetTable, 1200, name: nameof(LeftoverTextureOffsetTable));

            // Serialize event block header
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));
            Unk4 = s.Serialize<uint>(Unk4, name: nameof(Unk4));
            EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));
            EventBlockSize = s.Serialize<uint>(EventBlockSize, name: nameof(EventBlockSize));
            EventBlockPointer = s.CurrentPointer;
            s.Goto(EventBlockPointer + EventBlockSize);

            // Serialize event command counts
            EventNumCommands = s.SerializeArray<ushort>(EventNumCommands, EventCount, name: nameof(EventNumCommands));
            EventNumLabelOffsets = s.SerializeArray<ushort>(EventNumLabelOffsets, EventCount, name: nameof(EventNumLabelOffsets));

            // Serialize tile-set texture data
            TileTextures = s.SerializeArray<byte>(TileTextures, 512 * 256, name: nameof(TileTextures));

            // Serialize the map tiles
            MapBlockSize = s.Serialize<uint>(MapBlockSize, name: nameof(MapBlockSize));
            MapTiles = s.SerializeObjectArray<PC_MapTile>(MapTiles, MapBlockSize / 6, name: nameof(MapTiles));


            // Finally, read the events
            s.DoAt(EventBlockPointer, () => {

                // Helper method to get the current position inside of the event block
                int GetPosInEventBlock() {
                    int currentPos = (int)(s.CurrentPointer - EventBlockPointer);
                    return currentPos;
                }

                // Serialize the events
                Events = s.SerializeObjectArray<PC_Event>(Events, EventCount, name: nameof(Events));

                // Padding...
                s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, EventCount * 4).ToArray(), EventCount * 4, name: "Padding");
                if (EventCount % 2 != 0) {
                    const int padding = 4;
                    s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, padding).ToArray(), padding, name: "Padding");
                }

                // Serialize the event link table
                EventLinkTable = s.SerializeArray<ushort>(EventLinkTable, EventCount, name: nameof(EventLinkTable));

                // Padding...
                if (EventCount % 2 == 0) {
                    const int padding = 2;
                    s.SerializeArray<byte>(Enumerable.Repeat((byte)0xCD, padding).ToArray(), padding, name: "Padding");
                }

                // Serialize the commands
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