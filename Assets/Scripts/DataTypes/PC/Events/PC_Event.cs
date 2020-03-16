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

        /// <summary>
        /// Indicates if the event has collision
        /// </summary>
        public bool FollowEnabled { get; set; }

        public ushort Unknown12 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            // Get the xor key to use for the event
            byte eveXor = (byte)(serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 145);

            serializer.Serialize(nameof(DES), eveXor);
            serializer.Serialize(nameof(DES2), eveXor);
            serializer.Serialize(nameof(DES3), eveXor);
            serializer.Serialize(nameof(ETA), eveXor);

            serializer.Serialize(nameof(Unknown1), eveXor);
            serializer.Serialize(nameof(Unknown2), eveXor);

            serializer.SerializeArray<byte>(nameof(Unknown3), 16, eveXor);

            serializer.Serialize(nameof(XPosition), eveXor);
            serializer.Serialize(nameof(YPosition), eveXor);

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (serializer.GameSettings.GameMode == GameMode.RayKit || serializer.GameSettings.GameMode == GameMode.RayEduPC)
                serializer.Serialize(nameof(Unknown13), eveXor);

            serializer.SerializeArray<byte>(nameof(Unknown4), 20, eveXor);
            serializer.SerializeArray<byte>(nameof(Unknown5), 28, eveXor);

            serializer.Serialize(nameof(Type), eveXor);
            serializer.Serialize(nameof(Unknown6), eveXor);

            serializer.Serialize(nameof(OffsetBX), eveXor);
            serializer.Serialize(nameof(OffsetBY), eveXor);

            serializer.Serialize(nameof(Unknown7), eveXor);

            serializer.Serialize(nameof(SubEtat), eveXor);
            serializer.Serialize(nameof(Etat), eveXor);

            serializer.Serialize(nameof(Unknown8), eveXor);
            serializer.Serialize(nameof(Unknown9), eveXor);

            serializer.Serialize(nameof(OffsetHY), eveXor);
            serializer.Serialize(nameof(FollowSprite), eveXor);
            serializer.Serialize(nameof(HitPoints), eveXor);
            serializer.Serialize(nameof(UnkGroup), eveXor);
            serializer.Serialize(nameof(HitSprite), eveXor);

            serializer.SerializeArray<byte>(nameof(Unknown10), 6, eveXor);

            serializer.Serialize(nameof(Unknown11), eveXor);

            // NOTE: This is 32 when true and 0 when false
            if (serializer.Mode == SerializerMode.Read)
                FollowEnabled = serializer.Read<byte>(eveXor) != 0;
            else
                serializer.Write((byte)(FollowEnabled ? 32 : 0), eveXor);

            serializer.Serialize(nameof(Unknown12), eveXor);
        }
    }
}