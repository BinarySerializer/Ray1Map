namespace BinarySerializer.KlonoaDTP
{
    /// <summary>
    /// A base class for BIN files
    /// </summary>
    public abstract class PS1Klonoa_BaseFile : BinarySerializable
    {
        /// <summary>
        /// The file size, should be set before serializing
        /// </summary>
        public long Pre_FileSize { get; set; }

        /// <summary>
        /// Indicates if the file is compressed using ULZ, should be set before serializing
        /// </summary>
        public bool Pre_IsCompressed { get; set; }
    }
}