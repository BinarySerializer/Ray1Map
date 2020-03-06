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
        /// Gets the event ID
        /// </summary>
        /// <returns>The event ID</returns>
        public string GetEventID() => $"{Type.ToString().PadLeft(3, '0')}{Etat.ToString().PadLeft(3, '0')}{SubEtat.ToString().PadLeft(3, '0')}";

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            Unknown1 = deserializer.ReadArray<byte>(28);

            XPosition = deserializer.Read<ushort>();
            YPosition = deserializer.Read<ushort>();

            Unknown2 = deserializer.ReadArray<byte>(16);
            Unknown3 = deserializer.Read<ushort>();
            Unknown4 = deserializer.Read<ushort>();
            Unknown5 = deserializer.Read<ushort>();
            Unknown6 = deserializer.ReadArray<byte>(28);

            OffsetBX = deserializer.Read<byte>();
            OffsetBY = deserializer.Read<byte>();
            
            Unknown7 = deserializer.Read<ushort>();

            Etat = deserializer.Read<ushort>();
            SubEtat = deserializer.Read<ushort>();

            Unknown8 = deserializer.Read<ushort>();
            Unknown9 = deserializer.Read<ushort>();

            OffsetHY = deserializer.Read<byte>();
            FollowSprite = deserializer.Read<byte>();

            Hitpoints = deserializer.Read<ushort>();

            UnkGroup = deserializer.Read<byte>();

            Type = deserializer.Read<byte>();

            HitSprite = deserializer.Read<ushort>();

            Unknown10 = deserializer.ReadArray<byte>(10);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Write(Unknown1);

            serializer.Write(XPosition);
            serializer.Write(YPosition);

            serializer.Write(Unknown2);
            serializer.Write(Unknown3);
            serializer.Write(Unknown4);
            serializer.Write(Unknown5);
            serializer.Write(Unknown6);

            serializer.Write(OffsetBX);
            serializer.Write(OffsetBY);

            serializer.Write(Unknown7);

            serializer.Write(Etat);
            serializer.Write(SubEtat);

            serializer.Write(Unknown8);
            serializer.Write(Unknown9);

            serializer.Write(OffsetHY);
            serializer.Write(FollowSprite);

            serializer.Write(Hitpoints);

            serializer.Write(UnkGroup);

            serializer.Write(Type);

            serializer.Write(HitSprite);

            serializer.Write(Unknown10);
        }
    }
}