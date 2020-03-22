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
            DES = s.Serialize(DES, name: "DES");
            DES2 = s.Serialize(DES2, name: "DES2");
            DES3 = s.Serialize(DES3, name: "DES3");
            ETA = s.Serialize(ETA, name: "ETA");

            Unknown1 = s.Serialize(Unknown1, name: "Unknown1");
            Unknown2 = s.Serialize(Unknown2, name: "Unknown2");

            Unknown3 = s.SerializeArray(Unknown3, 16, name: "Unknown3");

            XPosition = s.Serialize(XPosition, name: "XPosition");
            YPosition = s.Serialize(YPosition, name: "YPosition");

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
                Unknown13 = s.Serialize(Unknown13, name: "Unknown13");

            Unknown4 = s.SerializeArray(Unknown4, 20, name: "Unknown4");
            Unknown5 = s.SerializeArray(Unknown5, 28, name: "Unknown5");

            Type = s.Serialize(Type, name: "Type");
            Unknown6 = s.Serialize(Unknown6, name: "Unknown6");

            OffsetBX = s.Serialize(OffsetBX, name: "OffsetBX");
            OffsetBY = s.Serialize(OffsetBY, name: "OffsetBY");

            Unknown7 = s.Serialize(Unknown7, name: "Unknown7");

            SubEtat = s.Serialize(SubEtat, name: "SubEtat");
            Etat = s.Serialize(Etat, name: "Etat");

            Unknown8 = s.Serialize(Unknown8, name: "Unknown8");
            Unknown9 = s.Serialize(Unknown9, name: "Unknown9");

            OffsetHY = s.Serialize(OffsetHY, name: "OffsetHY");
            FollowSprite = s.Serialize(FollowSprite, name: "FollowSprite");
            HitPoints = s.Serialize(HitPoints, name: "HitPoints");
            UnkGroup = s.Serialize(UnkGroup, name: "UnkGroup");
            HitSprite = s.Serialize(HitSprite, name: "HitSprite");

            Unknown10 = s.SerializeArray(Unknown10, 6, name: "Unknown10");

            Unknown11 = s.Serialize(Unknown11, name: "Unknown11");

            // NOTE: This is 32 when true and 0 when false
            FollowEnabled = s.Serialize((byte)(FollowEnabled ? 32 : 0), name: "FollowEnabled") != 0;

            Unknown12 = s.Serialize(Unknown12, name: "Unknown12");
        }
    }
}