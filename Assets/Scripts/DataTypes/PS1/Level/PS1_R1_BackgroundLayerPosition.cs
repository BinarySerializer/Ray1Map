using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Background later position data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundLayerPosition : IBinarySerializable
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
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            XPosition = deserializer.Read<ushort>();
            YPosition = deserializer.Read<ushort>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Write(XPosition);
            serializer.Write(YPosition);
        }
    }
}