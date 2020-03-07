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
        /// Unknown value, possibly a boolean
        /// </summary>
        public ushort Unknown1 { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public virtual void Deserialize(BinaryDeserializer deserializer)
        {
            if (deserializer.GameSettings.GameMode != GameMode.RayPC)
            {
                PrimaryKitHeader = deserializer.ReadArray<byte>(5);
                SecondaryKitHeader = deserializer.ReadArray<byte>(5);
                Unknown1 = deserializer.Read<ushort>();
            }
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public virtual void Serialize(BinarySerializer serializer)
        {
            if (serializer.GameSettings.GameMode != GameMode.RayPC)
            {
                serializer.Write(PrimaryKitHeader);
                serializer.Write(SecondaryKitHeader);
                serializer.Write(Unknown1);
            }
        }
    }
}