namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Event : IBinarySerializable
    {
        public byte[] Unknown1 { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public ushort XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public ushort YPosition { get; set; }

        public byte[] Unknown2 { get; set; }

        public ushort Unknown3 { get; set; }

        public ushort Unknown4 { get; set; }

        // Always 254?
        public ushort Unknown5 { get; set; }

        public byte[] Unknown6 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        public ushort Unknown7 { get; set; }

        public ushort Etat { get; set; }

        public ushort SubEtat { get; set; }

        public ushort Unknown8 { get; set; }

        public ushort Unknown9 { get; set; }

        public byte OffsetHY { get; set; }

        public byte FollowSprite { get; set; }

        public ushort Hitpoints { get; set; }
        
        public byte UnkGroup { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public byte Type { get; set; }

        // NOTE: Maybe a byte?
        public ushort HitSprite { get; set; }

        public byte[] Unknown10 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.SerializeArray<byte>(nameof(Unknown1), 28);

            serializer.Serialize(nameof(XPosition));
            serializer.Serialize(nameof(YPosition));

            serializer.SerializeArray<byte>(nameof(Unknown2), 16);
            serializer.Serialize(nameof(Unknown3));
            serializer.Serialize(nameof(Unknown4));
            serializer.Serialize(nameof(Unknown5));
            serializer.SerializeArray<byte>(nameof(Unknown6), 28);

            serializer.Serialize(nameof(OffsetBX));
            serializer.Serialize(nameof(OffsetBY));
            
            serializer.Serialize(nameof(Unknown7));

            serializer.Serialize(nameof(Etat));
            serializer.Serialize(nameof(SubEtat));

            serializer.Serialize(nameof(Unknown8));
            serializer.Serialize(nameof(Unknown9));

            serializer.Serialize(nameof(OffsetHY));
            serializer.Serialize(nameof(FollowSprite));

            serializer.Serialize(nameof(Hitpoints));

            serializer.Serialize(nameof(UnkGroup));

            serializer.Serialize(nameof(Type));

            serializer.Serialize(nameof(HitSprite));

            serializer.SerializeArray<byte>(nameof(Unknown10), 10);
        }
    }
}