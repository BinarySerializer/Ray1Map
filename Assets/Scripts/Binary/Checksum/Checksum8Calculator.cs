namespace R1Engine
{
    /// <summary>
    /// Checksum calculator for an 8-bit checksum
    /// </summary>
    public class Checksum8Calculator : IChecksumCalculator<byte>
    {
        /// <summary>
        /// Adds a byte to the checksum
        /// </summary>
        /// <param name="b">The byte to add</param>
        public void AddByte(byte b)
        {
            ChecksumValue = (byte)((ChecksumValue + b) % 256);
        }

        /// <summary>
        /// Adds an array of bytes to the checksum
        /// </summary>
        /// <param name="bytes">The bytes to add</param>
        public void AddBytes(byte[] bytes)
        {
            foreach (var b in bytes)
                AddByte(b);
        }

        /// <summary>
        /// The current checksum value
        /// </summary>
        public byte ChecksumValue { get; set; }
    }
}