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
        public string PrimaryKitHeader { get; set; }

        /// <summary>
        /// The secondary kit header, always 5 bytes starting with KIT or the language tag and then NULL padding
        /// </summary>
        public string SecondaryKitHeader { get; set; }

        /// <summary>
        /// Unknown value, possibly a boolean
        /// </summary>
        public ushort Unknown1 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPS1)
            {
                PrimaryKitHeader = s.SerializeString(PrimaryKitHeader, 5, name: nameof(PrimaryKitHeader));
                SecondaryKitHeader = s.SerializeString(SecondaryKitHeader, 5, name: nameof(SecondaryKitHeader));
                Unknown1 = s.Serialize<ushort>(Unknown1, name: nameof(Unknown1));
            }
        }
    }
}