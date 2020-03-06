namespace R1Engine
{
    /// <summary>
    /// Base file data for Rayman Designer (PC)
    /// </summary>
    public abstract class PC_BaseFile : IBinarySerializable
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
        /// <param name="deserializer">The deserializer</param>
        public virtual void Deserialize(BinaryDeserializer deserializer)
        {
            if (deserializer.GameSettings.GameMode != GameMode.RaymanPC)
            {
                PrimaryKitHeader = deserializer.Read<byte>(5);
                SecondaryKitHeader = deserializer.Read<byte>(5);
            }
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public virtual void Serialize(BinarySerializer serializer)
        {
            if (serializer.GameSettings.GameMode != GameMode.RaymanPC)
            {
                serializer.Write(PrimaryKitHeader);
                serializer.Write(SecondaryKitHeader);
            }
        }
    }
}