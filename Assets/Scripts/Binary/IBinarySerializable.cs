namespace R1Engine
{
    /// <summary>
    /// Defines a serializable data type
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        void Serialize(BinarySerializer serializer);
    }
}