using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Background later position data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundLayerPosition : ISerializableFile
    {
        /// <summary>
        /// The layer x position
        /// </summary>
        public ushort XPosition { get; set; }
        
        /// <summary>
        /// The later y position
        /// </summary>
        public ushort YPosition { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            XPosition = stream.Read<ushort>();
            YPosition = stream.Read<ushort>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            stream.Write(XPosition);
            stream.Write(YPosition);
        }
    }
}