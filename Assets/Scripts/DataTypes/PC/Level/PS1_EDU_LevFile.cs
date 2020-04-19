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

            /*

            In Jun01 the structure is as follows:

            unk[4804]          - ???
            ushort             - eventCount
            uint               - ???
            event[eventCount]  - events (136 bytes each)
            unk[624]           - unknown (always 0xCD)
            ushort[eventCount] - eventLinkTable
            unk[4]             - unknown (always 0xCD)
            commands           - eventCommands

            Commands do not have any length specified like on PC oddly enough. When label offsets are used they're separated using 0xCD twice.

            Tiles are stored in a tileset, 512x256 (where each tile is 16px). It starts around offset 31744 (should be right after the commands).

            After the tileset are the map tiles, 6 bytes each until end of file

            Also worth mentioning, allfix and world files have been modified. Any .NEW files is modified from PC.

             */

            throw new NotImplementedException();
        }

        #endregion
    }
}