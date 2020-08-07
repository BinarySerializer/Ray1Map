namespace R1Engine
{
    /// <summary>
    /// Vignette block group data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_VignetteBlockGroup : R1Serializable
    {
        /// <summary>
        /// The size of the block group, in pixels
        /// </summary>
        public int BlockGroupSize { get; set; }

        public ushort Unknown1 { get; set; }

        /// <summary>
        /// The image width
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The image height
        /// </summary>
        public ushort Height { get; set; }

        public ushort Unknown2 { get; set; }

        /// <summary>
        /// The image blocks, each one 64 pixels wide
        /// </summary>
        public ARGB1555Color[][] ImageBlocks { get; set; }

        /// <summary>
        /// Gets the block width based on engine version
        /// </summary>
        /// <param name="engineVersion">The engine version</param>
        /// <returns>The block width</returns>
        public int GetBlockWidth(EngineVersion engineVersion) => engineVersion == EngineVersion.RayPS1JPDemoVol3 ? 32 : 64;

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header values
            Unknown1 = s.Serialize<ushort>(Unknown1, name: nameof(Unknown1));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Unknown2 = s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));

            // Get the block width
            var blockWidth = GetBlockWidth(s.GameSettings.EngineVersion);

            // Create block array
            if (ImageBlocks == null)
            {
                // Get the size of each block
                var blockSize = Height * blockWidth;

                ImageBlocks = new ARGB1555Color[BlockGroupSize / blockSize][];
            }

            // Serialize blocks
            for (int i = 0; i < ImageBlocks.Length; i++)
                ImageBlocks[i] = s.SerializeObjectArray<ARGB1555Color>(ImageBlocks[i], blockWidth * Height, name: nameof(ImageBlocks) + "[" + i + "]");
        }
    }
}