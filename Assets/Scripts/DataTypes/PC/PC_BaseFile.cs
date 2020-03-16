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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public virtual void Serialize(BinarySerializer serializer)
        {
            if (serializer.GameSettings.GameMode == GameMode.RayKit || serializer.GameSettings.GameMode == GameMode.RayEduPC)
            {
                serializer.SerializeArray<byte>(nameof(PrimaryKitHeader), 5);
                serializer.SerializeArray<byte>(nameof(SecondaryKitHeader), 5);
                serializer.Serialize(nameof(Unknown1));
            }
        }
    }
}