namespace R1Engine
{
    /// <summary>
    /// Background vignette file data for Rayman 1 (PS1)
    /// </summary>
    public class R1_PS1_BackgroundVignetteFile : R1_PS1BaseFile
    {
        /// <summary>
        /// The width of an image block
        /// </summary>
        public const int BlockWidth = 64;

        /// <summary>
        /// The pointer to the image block
        /// </summary>
        public Pointer ImageBlockPointer => BlockPointers[0];

        /// <summary>
        /// The pointer to the palette block
        /// </summary>
        public Pointer PaletteBlockPointer => BlockPointers[1];

        /// <summary>
        /// The image block
        /// </summary>
        public R1_PS1_VignetteBlockGroup ImageBlock { get; set; }

        public byte[] UnknownPaletteHeader { get; set; }

        /// <summary>
        /// The parallax sprites color palettes
        /// </summary>
        public RGBA5551Color[][] ParallaxPalettes { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // HEADER
            base.SerializeImpl(s);

            // IMAGE BLOCK

            s.DoAt(ImageBlockPointer, () => ImageBlock = s.SerializeObject<R1_PS1_VignetteBlockGroup>(ImageBlock, name: nameof(ImageBlock), onPreSerialize: x => x.BlockGroupSize = (int)(PaletteBlockPointer - ImageBlockPointer) / 2));

            // PARALLAX PALETTES

            s.DoAt(PaletteBlockPointer, () => 
            {
                // TODO: Get correct length and parse
                UnknownPaletteHeader = s.SerializeArray<byte>(UnknownPaletteHeader, (FileSize - PaletteBlockPointer.FileOffset) % 512, name: nameof(UnknownPaletteHeader));

                uint numPalettes = (uint)(FileSize - s.CurrentPointer.FileOffset) / (256 * 2);
                if (ParallaxPalettes == null)
                    ParallaxPalettes = new RGBA5551Color[numPalettes][];

                for (int i = 0; i < ParallaxPalettes.Length; i++)
                    ParallaxPalettes[i] = s.SerializeObjectArray<RGBA5551Color>(ParallaxPalettes[i], 256, name: nameof(ParallaxPalettes) + "[" + i + "]");
            });
        }
    }
}