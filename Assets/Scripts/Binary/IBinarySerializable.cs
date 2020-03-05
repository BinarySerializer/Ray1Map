namespace R1Engine
{
    /// <summary>
    /// Defines a serializable file
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        void Deserialize(BinaryDeserializer deserializer);

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        void Serialize(BinarySerializer serializer);
    }
}