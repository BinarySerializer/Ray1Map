namespace R1Engine
{
    /// <summary>
    /// Event data for PC
    /// </summary>
    public class PC_Event : R1Serializable
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
        public override void SerializeImpl(SerializerObject s) {
            DES = s.Serialize<uint>(DES, name: "DES");
            DES2 = s.Serialize<uint>(DES2, name: "DES2");
            DES3 = s.Serialize<uint>(DES3, name: "DES3");
            ETA = s.Serialize<uint>(ETA, name: "ETA");

            Unknown1 = s.Serialize<uint>(Unknown1, name: "Unknown1");
            Unknown2 = s.Serialize<uint>(Unknown2, name: "Unknown2");

            Unknown3 = s.SerializeArray<byte>(Unknown3, 16, name: "Unknown3");

            XPosition = s.Serialize<uint>(XPosition, name: "XPosition");
            YPosition = s.Serialize<uint>(YPosition, name: "YPosition");

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
                Unknown13 = s.Serialize<uint>(Unknown13, name: "Unknown13");

            Unknown4 = s.SerializeArray<byte>(Unknown4, 20, name: "Unknown4");
            Unknown5 = s.SerializeArray<byte>(Unknown5, 28, name: "Unknown5");

            Type = s.Serialize<uint>(Type, name: "Type");
            Unknown6 = s.Serialize<uint>(Unknown6, name: "Unknown6");

            OffsetBX = s.Serialize<byte>(OffsetBX, name: "OffsetBX");
            OffsetBY = s.Serialize<byte>(OffsetBY, name: "OffsetBY");

            Unknown7 = s.Serialize<ushort>(Unknown7, name: "Unknown7");

            SubEtat = s.Serialize<byte>(SubEtat, name: "SubEtat");
            Etat = s.Serialize<byte>(Etat, name: "Etat");

            Unknown8 = s.Serialize<ushort>(Unknown8, name: "Unknown8");
            Unknown9 = s.Serialize<uint>(Unknown9, name: "Unknown9");

            OffsetHY = s.Serialize<byte>(OffsetHY, name: "OffsetHY");
            FollowSprite = s.Serialize<byte>(FollowSprite, name: "FollowSprite");
            HitPoints = s.Serialize<ushort>(HitPoints, name: "HitPoints");
            UnkGroup = s.Serialize<byte>(UnkGroup, name: "UnkGroup");
            HitSprite = s.Serialize<byte>(HitSprite, name: "HitSprite");

            Unknown10 = s.SerializeArray<byte>(Unknown10, 6, name: "Unknown10");

            Unknown11 = s.Serialize<byte>(Unknown11, name: "Unknown11");

            // NOTE: This is 32 when true and 0 when false
            FollowEnabled = s.Serialize<byte>((byte)(FollowEnabled ? 32 : 0), name: "FollowEnabled") != 0;

            Unknown12 = s.Serialize<ushort>(Unknown12, name: "Unknown12");
        }
    }
}