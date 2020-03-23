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
            DES = s.Serialize(DES, name: "DES");
            DES2 = s.Serialize(DES2, name: "DES2");
            DES3 = s.Serialize(DES3, name: "DES3");
            ETA = s.Serialize(ETA, name: "ETA");

            Unk1 = s.SerializeArray(Unk1, 6, name: "Unk1");

            XPosition = s.Serialize(XPosition, name: "XPosition");
            YPosition = s.Serialize(YPosition, name: "YPosition");

            Unk2 = s.Serialize(Unk3, name: "Unk2");

                // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
                Unk3 = s.Serialize(Unk3, name: "Unk3");

            Unk4 = s.SerializeArray(Unk4, 22, name: "Unk4");

            Type = s.Serialize(Type, name: "Type");
            Unk5 = s.SerializeArray(Unk5, 5, name: "Unk5");
            Unk6 = s.Serialize(Unk6, name: "Unk6");

            OffsetBX = s.Serialize(OffsetBX, name: "OffsetBX");
            OffsetBY = s.Serialize(OffsetBY, name: "OffsetBY");

            Unk7 = s.Serialize(Unk7, name: "Unk7");

            SubEtat = s.Serialize(SubEtat, name: "SubEtat");
            Etat = s.Serialize(Etat, name: "Etat");

            Unk8 = s.Serialize(Unk8, name: "Unk8");
            Unk9 = s.Serialize(Unk9, name: "Unk9");

            OffsetHY = s.Serialize(OffsetHY, name: "OffsetHY");
            FollowSprite = s.Serialize(FollowSprite, name: "FollowSprite");
            HitPoints = s.Serialize(HitPoints, name: "HitPoints");
            UnkGroup = s.Serialize(UnkGroup, name: "UnkGroup");
            HitSprite = s.Serialize(HitSprite, name: "HitSprite");

            Unk10 = s.SerializeArray(Unk10, 6, name: "Unk10");

            Unk11 = s.Serialize(Unk11, name: "Unk11");

            // NOTE: This is 32 when true and 0 when false
            FollowEnabled = s.Serialize((byte)(FollowEnabled ? 32 : 0), name: "FollowEnabled") != 0;

            Unk12 = s.Serialize(Unk12, name: "Unk12");
        }
    }
}