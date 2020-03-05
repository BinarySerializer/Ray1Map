namespace R1Engine
{
    /// <summary>
    /// Event localization item for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_EventLocItem : IBinarySerializable
    {
        /// <summary>
        /// The localization key
        /// </summary>
        public string LocKey { get; set; }

        /// <summary>
        /// The localized name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The localized description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            LocKey = deserializer.ReadNullTerminatedString();
            Name = deserializer.ReadNullTerminatedString();
            Description = deserializer.ReadNullTerminatedString();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.WriteNullTerminatedString(LocKey);
            serializer.WriteNullTerminatedString(Name);
            serializer.WriteNullTerminatedString(Description);
        }
    }
}