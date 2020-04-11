namespace R1Engine
{
    /// <summary>
    /// Background vignette file data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundVignetteFile : R1Serializable
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

        /// <summary>
        /// The image block
        /// </summary>
        public PS1_R1_VignetteBlockGroup ImageBlock { get; set; }

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

            ImageBlock = s.SerializeObject<PS1_R1_VignetteBlockGroup>(ImageBlock, name: nameof(ImageBlock), onPreSerialize: x => x.BlockGroupSize = (int)(Pointers[1] - Pointers[0]) / 2);

            // UNKNOWN

            UnknownBlock = s.SerializeArray<byte>(UnknownBlock, FileSize - s.CurrentPointer.FileOffset, name: nameof(UnknownBlock));
        }
    }
}