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

        public uint[] Unk1 { get; set; }

        public uint XPosition { get; set; }

        public uint YPosition { get; set; }

        public uint Unk2 { get; set; }

        public uint Unk3 { get; set; }

        public ushort[] Unk4 { get; set; }

        public ushort Type { get; set; }

        public byte[] Unk5 { get; set; }

        public byte Unk6 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        public ushort Unk7 { get; set; }

        public byte SubEtat { get; set; }

        public byte Etat { get; set; }

        public ushort Unk8 { get; set; }

        public uint Unk9 { get; set; }

        public byte OffsetHY { get; set; }

        public byte FollowSprite { get; set; }

        public ushort HitPoints { get; set; }

        public byte UnkGroup { get; set; }

        public byte HitSprite { get; set; }

        public byte[] Unk10 { get; set; }

        public byte Unk11 { get; set; }

        /// <summary>
        /// Indicates if the event has collision
        /// </summary>
        public bool FollowEnabled { get; set; }

        public ushort Unk12 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            DES = s.Serialize<uint>(DES, name: "DES");
            DES2 = s.Serialize<uint>(DES2, name: "DES2");
            DES3 = s.Serialize<uint>(DES3, name: "DES3");
            ETA = s.Serialize<uint>(ETA, name: "ETA");

            Unk1 = s.SerializeArray<uint>(Unk1, 6, name: "Unk1");

            XPosition = s.Serialize<uint>(XPosition, name: "XPosition");
            YPosition = s.Serialize<uint>(YPosition, name: "YPosition");

            Unk2 = s.Serialize<uint>(Unk3, name: "Unk2");

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
                Unk3 = s.Serialize<uint>(Unk3, name: "Unk3");

            Unk4 = s.SerializeArray<ushort>(Unk4, 22, name: "Unk4");

            Type = s.Serialize<ushort>(Type, name: "Type");
            Unk5 = s.SerializeArray<byte>(Unk5, 5, name: "Unk5");
            Unk6 = s.Serialize<byte>(Unk6, name: "Unk6");

            OffsetBX = s.Serialize<byte>(OffsetBX, name: "OffsetBX");
            OffsetBY = s.Serialize<byte>(OffsetBY, name: "OffsetBY");

            Unk7 = s.Serialize<ushort>(Unk7, name: "Unk7");

            SubEtat = s.Serialize<byte>(SubEtat, name: "SubEtat");
            Etat = s.Serialize<byte>(Etat, name: "Etat");

            Unk8 = s.Serialize<ushort>(Unk8, name: "Unk8");
            Unk9 = s.Serialize<uint>(Unk9, name: "Unk9");

            OffsetHY = s.Serialize<byte>(OffsetHY, name: "OffsetHY");
            FollowSprite = s.Serialize<byte>(FollowSprite, name: "FollowSprite");
            HitPoints = s.Serialize<ushort>(HitPoints, name: "HitPoints");
            UnkGroup = s.Serialize<byte>(UnkGroup, name: "UnkGroup");
            HitSprite = s.Serialize<byte>(HitSprite, name: "HitSprite");

            Unk10 = s.SerializeArray<byte>(Unk10, 6, name: "Unk10");

            Unk11 = s.Serialize<byte>(Unk11, name: "Unk11");

            // NOTE: This is 32 when true and 0 when false
            FollowEnabled = s.Serialize<byte>((byte)(FollowEnabled ? 32 : 0), name: "FollowEnabled") != 0;

            Unk12 = s.Serialize<ushort>(Unk12, name: "Unk12");
        }
    }
}