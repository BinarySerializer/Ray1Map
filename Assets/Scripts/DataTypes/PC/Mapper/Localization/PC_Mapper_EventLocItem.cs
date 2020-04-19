namespace R1Engine
{
    /// <summary>
    /// Event localization item for Rayman Mapper (PC)
    /// </summary>
    public class PC_Mapper_EventLocItem : R1Serializable
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
        public override void SerializeImpl(SerializerObject s) {
            LocKey = s.SerializeString(LocKey);
            Name = s.SerializeString(Name);
            Description = s.SerializeString(Description);
        }
    }
}