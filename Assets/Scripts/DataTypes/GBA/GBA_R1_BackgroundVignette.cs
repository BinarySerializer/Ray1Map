namespace R1Engine
{
    /// <summary>
    /// Vignette background data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_BackgroundVignette : GBA_R1_BaseVignette
    {
        #region Properties

        protected override int PaletteCount => 6;

        #endregion

        #region Vignette Data

        // Always 0x00 except for first byte which is sometimes 1
        public byte[] UnkBytes_14 { get; set; }

        // Both pointers lead to a pointer array - size seems to be specified in UnkBytes_20 - parallax parts?
        public Pointer Pointer_18 { get; set; }
        public Pointer Pointer_1B { get; set; }

        // Third byte is either 0 or 1
        public byte[] UnkBytes_20 { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize data
            ImageDataPointer = s.SerializePointer(ImageDataPointer, name: nameof(ImageDataPointer));
            BlockIndicesPointer = s.SerializePointer(BlockIndicesPointer, name: nameof(BlockIndicesPointer));
            PaletteIndicesPointer = s.SerializePointer(PaletteIndicesPointer, name: nameof(PaletteIndicesPointer));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));

            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            UnkBytes_14 = s.SerializeArray<byte>(UnkBytes_14, 4, name: nameof(UnkBytes_14));

            Pointer_18 = s.SerializePointer(Pointer_18, name: nameof(Pointer_18));
            Pointer_1B = s.SerializePointer(Pointer_1B, name: nameof(Pointer_1B));

            UnkBytes_20 = s.SerializeArray<byte>(UnkBytes_20, 4, name: nameof(UnkBytes_20));

            // Serialize data from pointers

            base.SerializeImpl(s);
        }

        #endregion
    }
}