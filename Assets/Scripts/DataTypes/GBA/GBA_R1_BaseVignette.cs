namespace R1Engine
{
    /// <summary>
    /// Base vignette data for Rayman Advance (GBA)
    /// </summary>
    public abstract class GBA_R1_BaseVignette : R1Serializable
    {
        #region Properties

        protected abstract int PaletteCount { get; }

        #endregion

        #region Vignette Data

        public Pointer ImageDataPointer { get; set; }

        public Pointer BlockIndicesPointer { get; set; }

        public Pointer PaletteIndicesPointer { get; set; }

        public Pointer PalettesPointer { get; set; }

        // Note: These sizes are based on the number of blocks, so the actual size is Width*8 x Heightx8
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        #endregion

        #region Parsed from Pointers

        /// <summary>
        /// The image data
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// The image block indices
        /// </summary>
        public ushort[] BlockIndices { get; set; }

        /// <summary>
        /// The palette indices
        /// </summary>
        public byte[] PaletteIndices { get; set; }

        /// <summary>
        /// The 6 available palettes (16 colors each)
        /// </summary>
        public ARGB1555Color[] Palettes { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize data from pointers

            if (ImageDataPointer != null)
                s.DoAt(ImageDataPointer, () => ImageData = s.SerializeArray<byte>(ImageData, 0x20 * Width * Height, name: nameof(ImageData)));

            if (BlockIndicesPointer != null)
                s.DoAt(BlockIndicesPointer, () => BlockIndices = s.SerializeArray<ushort>(BlockIndices, Width * Height, name: nameof(BlockIndices)));

            if (PaletteIndicesPointer != null)
                s.DoAt(PaletteIndicesPointer, () => PaletteIndices = s.SerializeArray<byte>(PaletteIndices, Width * Height, name: nameof(PaletteIndices)));

            if (PalettesPointer != null)
                s.DoAt(PalettesPointer, () => Palettes = s.SerializeObjectArray<ARGB1555Color>(Palettes, PaletteCount * 16, name: nameof(Palettes)));
        }

        #endregion
    }
}