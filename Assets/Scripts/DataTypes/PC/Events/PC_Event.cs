namespace R1Engine
{
    /// <summary>
    /// Event data for PC
    /// </summary>
    public class PC_Event : IBinarySerializable
    {
        public uint DES { get; set; }

        public uint DES2 { get; set; }

        public uint DES3 { get; set; }

        public uint ETA { get; set; }

        public uint Unknown1 { get; set; }

        public uint Unknown2 { get; set; }

        public byte[] Unknown3 { get; set; }

        public uint XPosition { get; set; }

        public uint YPosition { get; set; }

        public byte[] Unknown4 { get; set; }

        public byte[] Unknown5 { get; set; }

        public uint Type { get; set; }

        public uint Unknown6 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        public ushort Unknown7 { get; set; }

        public byte SubEtat { get; set; }

        public byte Etat { get; set; }

        public ushort Unknown8 { get; set; }

        public uint Unknown9 { get; set; }

        public byte OffsetHY { get; set; }

        public byte FollowSprite { get; set; }

        public ushort HitPoints { get; set; }

        public byte UnkGroup { get; set; }

        public byte HitSprite { get; set; }

        public byte[] Unknown10 { get; set; }

        public byte Unknown11 { get; set; }

        public byte FollowEnabled { get; set; }

        public ushort Unknown12 { get; set; }

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
            // TODO: Add for writing
            // Get the xor key to use for the event
            byte eveXor = (byte)(deserializer.GameSettings.GameMode == GameMode.RaymanPC ? 0 : 145);

            DES = deserializer.Read<uint>(eveXor);
            DES2 = deserializer.Read<uint>(eveXor);
            DES3 = deserializer.Read<uint>(eveXor);
            ETA = deserializer.Read<uint>(eveXor);

            Unknown1 = deserializer.Read<uint>(eveXor);
            Unknown2 = deserializer.Read<uint>(eveXor);

            Unknown3 = deserializer.ReadArray<byte>(16, eveXor);

            XPosition = deserializer.Read<uint>(eveXor);
            YPosition = deserializer.Read<uint>(eveXor);

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong? - add for writing
            if (deserializer.GameSettings.GameMode != GameMode.RaymanPC)
            {
                deserializer.Read<uint>(eveXor);
            }

            Unknown4 = deserializer.ReadArray<byte>(20, eveXor);

            Unknown5 = deserializer.ReadArray<byte>(28, eveXor);

            Type = deserializer.Read<uint>(eveXor);
            Unknown6 = deserializer.Read<uint>(eveXor);

            OffsetBX = deserializer.Read<byte>(eveXor);
            OffsetBY = deserializer.Read<byte>(eveXor);

            Unknown7 = deserializer.Read<ushort>(eveXor);

            SubEtat = deserializer.Read<byte>(eveXor);
            Etat = deserializer.Read<byte>(eveXor);

            Unknown8 = deserializer.Read<ushort>(eveXor);
            Unknown9 = deserializer.Read<uint>(eveXor);

            OffsetHY = deserializer.Read<byte>(eveXor);
            FollowSprite = deserializer.Read<byte>(eveXor);
            HitPoints = deserializer.Read<ushort>(eveXor);
            UnkGroup = deserializer.Read<byte>(eveXor);
            HitSprite = deserializer.Read<byte>(eveXor);

            Unknown10 = deserializer.ReadArray<byte>(6, eveXor);

            Unknown11 = deserializer.Read<byte>(eveXor);
            FollowEnabled = deserializer.Read<byte>(eveXor);
            Unknown12 = deserializer.Read<ushort>(eveXor);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Write(DES);
            serializer.Write(DES2);
            serializer.Write(DES3);
            serializer.Write(ETA);

            serializer.Write(Unknown1);
            serializer.Write(Unknown2);

            serializer.Write(Unknown3);

            serializer.Write(XPosition);
            serializer.Write(YPosition);

            serializer.Write(Unknown4);
            serializer.Write(Unknown5);

            serializer.Write(Type);
            serializer.Write(Unknown6);

            serializer.Write(OffsetBX);
            serializer.Write(OffsetBY);

            serializer.Write(Unknown7);

            serializer.Write(SubEtat);
            serializer.Write(Etat);

            serializer.Write(Unknown8);
            serializer.Write(Unknown9);

            serializer.Write(OffsetHY);
            serializer.Write(FollowSprite);
            serializer.Write(HitPoints);
            serializer.Write(UnkGroup);
            serializer.Write(HitSprite);

            serializer.Write(Unknown10);
            serializer.Write(Unknown11);

            serializer.Write(FollowEnabled);

            serializer.Write(Unknown12);
        }
    }
}