namespace R1Engine
{
    /// <summary>
    /// Encrypted file archive entry data for PC
    /// </summary>
    public class PC_EncryptedFileArchiveEntry : R1Serializable
    {
        /// <summary>
        /// The XOR key to use when decoding the file
        /// </summary>
        public byte XORKey { get; set; }

        /// <summary>
        /// The encoded file checksum
        /// </summary>
        public byte Checksum { get; set; }
        
        /// <summary>
        /// The file offset
        /// </summary>
        public uint FileOffset { get; set; }
        
        /// <summary>
        /// The file size
        /// </summary>
        public uint FileSize { get; set; }
        
        /// <summary>
        /// The file name
        /// </summary>
        public byte[] FileName { get; set; }

        /// <summary>
        /// The file name as a string
        /// </summary>
        public string FileNameString => Settings.StringEncoding.GetString(FileName).TrimEnd('\0');

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            XORKey = s.Serialize<byte>(XORKey, name: nameof(XORKey));
            Checksum = s.Serialize<byte>(Checksum, name: nameof(Checksum));
            FileOffset = s.Serialize<uint>(FileOffset, name: nameof(FileOffset));
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));

            s.DoXOR(XORKey, () => FileName = s.SerializeArray<byte>(FileName, 9, name: nameof(FileName)));
        }
    }
}