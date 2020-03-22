namespace R1Engine
{
    /// <summary>
    /// Base file data for Rayman Designer (PC)
    /// </summary>
    public abstract class PC_BaseFile : R1Serializable
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
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
            {
                PrimaryKitHeader = s.SerializeArray<byte>(PrimaryKitHeader, 5, name: "PrimaryKitHeader");
                SecondaryKitHeader = s.SerializeArray<byte>(SecondaryKitHeader, 5, name: "SecondaryKitHeader");
                Unknown1 = s.Serialize(Unknown1, name: "Unknown1");
            }
        }
    }
}