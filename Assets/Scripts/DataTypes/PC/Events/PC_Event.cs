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

        public uint Unknown13 { get; set; }

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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Get the xor key to use for the event
            byte eveXor = (byte)(deserializer.GameSettings.GameMode == GameMode.RayPC || deserializer.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 145);

            DES = deserializer.Read<uint>(eveXor);
            DES2 = deserializer.Read<uint>(eveXor);
            DES3 = deserializer.Read<uint>(eveXor);
            ETA = deserializer.Read<uint>(eveXor);

            Unknown1 = deserializer.Read<uint>(eveXor);
            Unknown2 = deserializer.Read<uint>(eveXor);

            Unknown3 = deserializer.ReadArray<byte>(16, eveXor);

            XPosition = deserializer.Read<uint>(eveXor);
            YPosition = deserializer.Read<uint>(eveXor);

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (deserializer.GameSettings.GameMode == GameMode.RayKit || deserializer.GameSettings.GameMode == GameMode.RayEduPC)
                Unknown13 = deserializer.Read<uint>(eveXor);

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
            // Get the xor key to use for the event
            byte eveXor = (byte)(serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 145);

            serializer.Write(DES, eveXor);
            serializer.Write(DES2, eveXor);
            serializer.Write(DES3, eveXor);
            serializer.Write(ETA, eveXor);

            serializer.Write(Unknown1, eveXor);
            serializer.Write(Unknown2, eveXor);

            serializer.Write(Unknown3, eveXor);

            serializer.Write(XPosition, eveXor);
            serializer.Write(YPosition, eveXor);

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (serializer.GameSettings.GameMode == GameMode.RayKit || serializer.GameSettings.GameMode == GameMode.RayEduPC)
                serializer.Write(Unknown13, eveXor);

            serializer.Write(Unknown4, eveXor);
            serializer.Write(Unknown5, eveXor);

            serializer.Write(Type, eveXor);
            serializer.Write(Unknown6, eveXor);

            serializer.Write(OffsetBX, eveXor);
            serializer.Write(OffsetBY, eveXor);

            serializer.Write(Unknown7, eveXor);

            serializer.Write(SubEtat, eveXor);
            serializer.Write(Etat, eveXor);

            serializer.Write(Unknown8, eveXor);
            serializer.Write(Unknown9, eveXor);

            serializer.Write(OffsetHY, eveXor);
            serializer.Write(FollowSprite, eveXor);
            serializer.Write(HitPoints, eveXor);
            serializer.Write(UnkGroup, eveXor);
            serializer.Write(HitSprite, eveXor);

            serializer.Write(Unknown10, eveXor);
            serializer.Write(Unknown11, eveXor);

            serializer.Write(FollowEnabled, eveXor);

            serializer.Write(Unknown12, eveXor);
        }
    }
}