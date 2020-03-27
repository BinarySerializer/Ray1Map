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

        public EventType Type { get; set; }

        public byte[] Unk5 { get; set; }

        public byte Unk6 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        // TODO: This is actually two bytes - with one of them being 255 when the letters in EDU games should be lower-case (they default to upper-case)
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
            DES = s.Serialize<uint>(DES, name: nameof(DES));
            DES2 = s.Serialize<uint>(DES2, name: nameof(DES2));
            DES3 = s.Serialize<uint>(DES3, name: nameof(DES3));
            ETA = s.Serialize<uint>(ETA, name: nameof(ETA));

            Unk1 = s.SerializeArray<uint>(Unk1, 6, name: nameof(Unk1));

            XPosition = s.Serialize<uint>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<uint>(YPosition, name: nameof(YPosition));

            Unk2 = s.Serialize<uint>(Unk3, name: nameof(Unk2));

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (s.GameSettings.EngineVersion == EngineVersion.RayKit || s.GameSettings.EngineVersion == EngineVersion.RayEduPC)
                Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));

            Unk4 = s.SerializeArray<ushort>(Unk4, 22, name: nameof(Unk4));

            Type = s.Serialize<EventType>(Type, name: nameof(Type));
            Unk5 = s.SerializeArray<byte>(Unk5, 5, name: nameof(Unk5));
            Unk6 = s.Serialize<byte>(Unk6, name: nameof(Unk6));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            Unk7 = s.Serialize<ushort>(Unk7, name: nameof(Unk7));

            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));

            Unk8 = s.Serialize<ushort>(Unk8, name: nameof(Unk8));
            Unk9 = s.Serialize<uint>(Unk9, name: nameof(Unk9));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
            HitPoints = s.Serialize<ushort>(HitPoints, name: nameof(HitPoints));
            UnkGroup = s.Serialize<byte>(UnkGroup, name: nameof(UnkGroup));
            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));

            Unk10 = s.SerializeArray<byte>(Unk10, 6, name: nameof(Unk10));

            Unk11 = s.Serialize<byte>(Unk11, name: nameof(Unk11));

            // NOTE: This is 32 when true and 0 when false
            FollowEnabled = s.Serialize<byte>((byte)(FollowEnabled ? 32 : 0), name: nameof(FollowEnabled)) != 0;

            Unk12 = s.Serialize<ushort>(Unk12, name: nameof(Unk12));
        }
    }
}