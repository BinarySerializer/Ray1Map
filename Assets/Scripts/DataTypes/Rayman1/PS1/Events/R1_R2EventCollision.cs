namespace R1Engine
{
    /// <summary>
    /// Event collision data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2EventCollision : R1Serializable
    {
        public byte[] Unk1 { get; set; }

        // These appear to have slightly different meanings than in R1. BX and BY seem to be hitbox related.
        public byte OffsetBX { get; set; }
        public byte OffsetBY { get; set; }
        public byte OffsetHY { get; set; }

        public byte[] Unk2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.SerializeArray<byte>(Unk1, 10, name: nameof(Unk1));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));
            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));

            Unk2 = s.SerializeArray<byte>(Unk2, 3, name: nameof(Unk2));
        }
    }
}