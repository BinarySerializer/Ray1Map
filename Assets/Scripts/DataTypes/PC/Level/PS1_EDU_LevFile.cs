using System;

namespace R1Engine
{
    /// <summary>
    /// Level data for EDU on PS1
    /// </summary>
    public class PS1_EDU_LevFile : PC_BaseFile
    {
        #region Public Properties

        public byte[] Unk1 { get; set; }

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

        public byte[] Unk2 { get; set; }

        /// <summary>
        /// The amount of events
        /// </summary>
        public ushort EventCount { get; set; }

        public uint Unk3 { get; set; }

        /// <summary>
        /// The events
        /// </summary>
        public PC_Event[] Events { get; set; }

        // Always 0xCD
        public byte[] UnkEventPadding1 { get; set; }

        /// <summary>
        /// The event link table
        /// </summary>
        public ushort[] EventLinkTable { get; set; }

        // Always 0xCD
        public byte[] UnkEventPadding2 { get; set; }

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

            // Same as Kit header?
            Unk1 = s.SerializeArray<byte>(Unk1, 64, name: nameof(Unk1));

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
            Unk2 = s.SerializeArray<byte>(Unk2, 4800, name: nameof(Unk2));

            // Serialize events
            EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));

            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));

            Events = s.SerializeObjectArray<PC_Event>(Events, EventCount, name: nameof(Events));

            // Hack to get padding length
            int getPaddingLength()
            {
                var paddingLength = 0;

                s.DoAt(s.CurrentPointer, () =>
                {
                    while (s.Serialize<byte>(0) == 0xCD)
                        paddingLength++;
                });

                return paddingLength;
            }

            var eventPaddingLength = UnkEventPadding1?.Length ?? getPaddingLength();
            UnkEventPadding1 = s.SerializeArray<byte>(UnkEventPadding1, eventPaddingLength, name: nameof(UnkEventPadding1));

            EventLinkTable = s.SerializeArray<ushort>(EventLinkTable, EventCount, name: nameof(EventLinkTable));

            eventPaddingLength = UnkEventPadding2?.Length ?? getPaddingLength();
            UnkEventPadding2 = s.SerializeArray<byte>(UnkEventPadding2, eventPaddingLength, name: nameof(UnkEventPadding2));

            // After this comes the commands. They do not have any length specified like on PC. When label offsets are used they're separated using 0xCD twice.

            // After this comes the tiles. They are stored in a tileset, 512x256 (where each tile is 16px).

            // After this comes the map tiles, 6 bytes each until end of file

            throw new NotImplementedException();
        }

        #endregion
    }
}