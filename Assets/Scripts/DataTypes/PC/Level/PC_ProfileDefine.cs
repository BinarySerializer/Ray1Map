namespace R1Engine
{
    public class PC_ProfileDefine : R1Serializable
    {
        /// <summary>
        /// The checksum for the encrypted data
        /// </summary>
        public byte ProfileDefineChecksum { get; set; }

        /// <summary>
        /// The KIT level name
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// The KIT level author
        /// </summary>
        public string LevelAuthor { get; set; }

        /// <summary>
        /// The KIT level description
        /// </summary>
        public string LevelDescription { get; set; }

        // TODO: Serialize this (ends with some booleans for level properties)
        public byte[] UnkKitProperties { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ProfileDefineChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR(0x96, () =>
                {
                    LevelName = s.SerializeString(LevelName, 25, name: nameof(LevelName));
                    LevelAuthor = s.SerializeString(LevelAuthor, 25, name: nameof(LevelAuthor));
                    LevelDescription = s.SerializeString(LevelDescription, 113, name: nameof(LevelDescription));

                    UnkKitProperties = s.SerializeArray<byte>(UnkKitProperties, 133, name: nameof(UnkKitProperties));
                });
            }, ChecksumPlacement.Before, name: nameof(ProfileDefineChecksum));
        }
    }
}