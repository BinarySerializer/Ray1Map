using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Base file data for Rayman Designer (PC)
    /// </summary>
    public abstract class PC_RD_BaseFile : ISerializableFile
    {
        /// <summary>
        /// The primary kit header, always 5 bytes starting with KIT and then NULL padding
        /// </summary>
        public byte[] PrimaryKitHeader { get; set; }

        /// <summary>
        /// The secondary kit header, always 5 bytes starting with KIT or the language tag and then NULL padding
        /// </summary>
        public byte[] SecondaryKitHeader { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public virtual void Deserialize(Stream stream)
        {
            PrimaryKitHeader = stream.ReadBytes(5);
            SecondaryKitHeader = stream.ReadBytes(5);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public virtual void Serialize(Stream stream)
        {
            stream.Write(PrimaryKitHeader);
            stream.Write(SecondaryKitHeader);
        }
    }
}