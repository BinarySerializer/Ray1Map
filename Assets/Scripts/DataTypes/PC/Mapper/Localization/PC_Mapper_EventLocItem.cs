namespace R1Engine
{
    /// <summary>
    /// Event localization item for Rayman Mapper (PC)
    /// </summary>
    public class PC_Mapper_EventLocItem : IBinarySerializable
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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            if (serializer.Mode == SerializerMode.Read)
            {
                LocKey = serializer.ReadNullTerminatedString();
                Name = serializer.ReadNullTerminatedString();
                Description = serializer.ReadNullTerminatedString();
            }
            else
            {
                serializer.WriteNullTerminatedString(LocKey);
                serializer.WriteNullTerminatedString(Name);
                serializer.WriteNullTerminatedString(Description);
            }
        }
    }
}