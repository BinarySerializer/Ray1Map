namespace R1Engine
{
    /// <summary>
    /// Vignette file data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_VignetteFile : R1Serializable
    {
        /// <summary>
        /// The width of an image block
        /// </summary>
        public const int BlockWidth = 64;

        /// <summary>
        /// The file pointers
        /// </summary>
        public uint[] Pointers { get; set; }

        /// <summary>
        /// The file size
        /// </summary>
        public uint FileSize { get; set; }

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

        public byte[] UnknownBlock { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // HEADER

            Pointers = s.SerializeArraySize<uint, uint>(Pointers);
            Pointers = s.SerializeArray<uint>(Pointers, Pointers.Length, name: nameof(Pointers));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));

            // IMAGE BLOCK

            // Serialize header values
            Unknown1 = s.Serialize<ushort>(Unknown1, name: nameof(Unknown1));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Unknown2 = s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));

            // Create block array
            if (ImageBlocks == null)
            {
                // Calculate the length
                int length = (int)(Pointers[1] - Pointers[0]) / 2;

                // Get the size of each block
                var blockSize = Height * BlockWidth;

                ImageBlocks = new ARGB1555Color[length / blockSize][];
            }

            // Serialize blocks
            for (int i = 0; i < ImageBlocks.Length; i++)
                ImageBlocks[i] = s.SerializeObjectArray<ARGB1555Color>(ImageBlocks[i], BlockWidth * Height, name: nameof(ImageBlocks) + "[" + i + "]");

            // UNKNOWN

            UnknownBlock = s.SerializeArray<byte>(UnknownBlock, FileSize - s.CurrentPointer.FileOffset, name: nameof(UnknownBlock));
        }
    }
}