using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Defines a serializable file
    /// </summary>
    public interface ISerializableFile
    {
        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        void Deserialize(Stream stream);

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        void Serialize(Stream stream);
    }
}