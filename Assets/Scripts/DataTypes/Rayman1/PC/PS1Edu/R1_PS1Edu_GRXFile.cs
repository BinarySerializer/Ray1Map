namespace R1Engine
{
    /// <summary>
    /// GRX file data for EDU on PS1
    /// </summary>
    public class R1_PS1Edu_GRXFile : R1Serializable
    {
        /// <summary>
        /// The file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The file size
        /// </summary>
        public uint FileSize { get; set; }

        /// <summary>
        /// The file offset, based on the base offset
        /// </summary>
        public uint FileOffset { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            FileName = s.SerializeString(FileName, name: nameof(FileName));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
        }
    }
}