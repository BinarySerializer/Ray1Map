namespace R1Engine
{
    /// <summary>
    /// Background vignette file data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundVignetteFile : PS1_R1_BaseFile
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
        public PS1_R1_VignetteBlockGroup ImageBlock { get; set; }

        public byte[] UnknownPaletteHeader { get; set; }

        /// <summary>
        /// The parallax sprites color palettes
        /// </summary>
        public ARGB1555Color[][] ParallaxPalettes { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // HEADER
            base.SerializeImpl(s);

            // IMAGE BLOCK

            s.DoAt(ImageBlockPointer, () => ImageBlock = s.SerializeObject<PS1_R1_VignetteBlockGroup>(ImageBlock, name: nameof(ImageBlock), onPreSerialize: x => x.BlockGroupSize = (int)(PaletteBlockPointer - ImageBlockPointer) / 2));

            // PARALLAX PALETTES

            s.DoAt(PaletteBlockPointer, () => 
            {
                // TODO: Get correct length and parse
                UnknownPaletteHeader = s.SerializeArray<byte>(UnknownPaletteHeader, (FileSize - PaletteBlockPointer.FileOffset) % 512, name: nameof(UnknownPaletteHeader));

                uint numPalettes = (uint)(FileSize - s.CurrentPointer.FileOffset) / (256 * 2);
                if (ParallaxPalettes == null)
                    ParallaxPalettes = new ARGB1555Color[numPalettes][];

                for (int i = 0; i < ParallaxPalettes.Length; i++)
                    ParallaxPalettes[i] = s.SerializeObjectArray<ARGB1555Color>(ParallaxPalettes[i], 256, name: nameof(ParallaxPalettes) + "[" + i + "]");
            });
        }
    }
}