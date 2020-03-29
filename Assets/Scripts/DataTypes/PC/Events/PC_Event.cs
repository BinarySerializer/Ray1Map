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

        public uint Unk_16 { get; set; }
        public uint Unk_20 { get; set; }
        public uint Unk_24 { get; set; }
        public uint Unk_28 { get; set; }
        public uint Unk_32 { get; set; }
        public uint Unk_36 { get; set; }

        public uint XPosition { get; set; }

        public uint YPosition { get; set; }

        public uint Unk_48 { get; set; }

        public uint Unk_52_Kit { get; set; }

        public ushort Unk_52 { get; set; }
        public ushort Unk_54 { get; set; }
        public ushort Unk_56 { get; set; }
        public ushort Unk_58 { get; set; }
        public ushort Unk_60 { get; set; }
        public ushort Unk_62 { get; set; }
        public ushort Unk_64 { get; set; }
        public ushort Unk_66 { get; set; }
        public ushort Unk_68 { get; set; }
        public ushort Unk_70 { get; set; }
        public ushort Unk_72 { get; set; }
        public ushort Unk_74 { get; set; }
        public ushort Unk_76 { get; set; }
        public ushort Unk_78 { get; set; }
        public ushort Unk_80 { get; set; }
        public ushort Unk_82 { get; set; }
        public ushort Unk_84 { get; set; }
        public ushort Unk_86 { get; set; }
        public ushort Unk_88 { get; set; }
        public ushort Unk_90 { get; set; }
        public ushort Unk_92 { get; set; }
        public ushort Unk_94 { get; set; }

        public EventType Type { get; set; }

        public byte[] Unk_98 { get; set; }

        public byte Unk_103 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        // TODO: This is actually two bytes - with one of them being 255 when the letters in EDU games should be lower-case (they default to upper-case)
        public ushort Unk_106 { get; set; }

        public byte SubEtat { get; set; }

        public byte Etat { get; set; }

        public ushort Unk_110 { get; set; }

        public uint Unk_112 { get; set; }

        public byte OffsetHY { get; set; }

        public byte FollowSprite { get; set; }

        public ushort HitPoints { get; set; }

        public byte Layer { get; set; }

        public byte HitSprite { get; set; }

        public byte Unk_122 { get; set; }
        public byte Unk_123 { get; set; }
        public byte Unk_124 { get; set; }
        public byte Unk_125 { get; set; }

        /// <summary>
        /// The layer, as stored in the game. Always 0 when serialized
        /// </summary>
        public byte TempLayer { get; set; }

        public byte Unk_127 { get; set; }

        public byte Unk_128 { get; set; }

        /// /// <summary>
        /// Flags of the event.
        /// Flags & (1 << 2) = switched on
        /// Flags & (1 << 3) = "detect zone flag"? I think it might be the flip flag for some enemies
        /// Flags & (1 << 5) = follow enabled (indicates if the event has collision
        /// Flags & (1 << 8) = execute commands
        /// </summary>
        public byte Flags { get; set; }
        public bool FollowEnabled {
            get { return BitHelpers.ExtractBits(Flags, 1, 5) == 1; }
            set { BitHelpers.SetBits(value ? 1 : 0, Flags, 1, 5); }
        }

        public ushort Unk_130 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            DES = s.Serialize<uint>(DES, name: nameof(DES));
            DES2 = s.Serialize<uint>(DES2, name: nameof(DES2));
            DES3 = s.Serialize<uint>(DES3, name: nameof(DES3));
            ETA = s.Serialize<uint>(ETA, name: nameof(ETA));

            Unk_16 = s.Serialize<uint>(Unk_16, name: nameof(Unk_16));
            Unk_20 = s.Serialize<uint>(Unk_20, name: nameof(Unk_20));
            Unk_24 = s.Serialize<uint>(Unk_24, name: nameof(Unk_24));
            Unk_28 = s.Serialize<uint>(Unk_28, name: nameof(Unk_28));
            Unk_32 = s.Serialize<uint>(Unk_32, name: nameof(Unk_32));
            Unk_36 = s.Serialize<uint>(Unk_36, name: nameof(Unk_36));

            XPosition = s.Serialize<uint>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<uint>(YPosition, name: nameof(YPosition));

            Unk_48 = s.Serialize<uint>(Unk_48, name: nameof(Unk_48));

            // TODO: Kit and edu has 4 more bytes between here and the type value - where does it belong?
            if (s.GameSettings.EngineVersion == EngineVersion.RayKit || s.GameSettings.EngineVersion == EngineVersion.RayEduPC)
                Unk_52_Kit = s.Serialize<uint>(Unk_52_Kit, name: nameof(Unk_52_Kit));

            Unk_52 = s.Serialize<ushort>(Unk_52, name: nameof(Unk_52));
            Unk_54 = s.Serialize<ushort>(Unk_54, name: nameof(Unk_54));
            Unk_56 = s.Serialize<ushort>(Unk_56, name: nameof(Unk_56));
            Unk_58 = s.Serialize<ushort>(Unk_58, name: nameof(Unk_58));
            Unk_60 = s.Serialize<ushort>(Unk_60, name: nameof(Unk_60));
            Unk_62 = s.Serialize<ushort>(Unk_62, name: nameof(Unk_62));
            Unk_64 = s.Serialize<ushort>(Unk_64, name: nameof(Unk_64));
            Unk_66 = s.Serialize<ushort>(Unk_66, name: nameof(Unk_66));
            Unk_68 = s.Serialize<ushort>(Unk_68, name: nameof(Unk_68));
            Unk_70 = s.Serialize<ushort>(Unk_70, name: nameof(Unk_70));
            Unk_72 = s.Serialize<ushort>(Unk_72, name: nameof(Unk_72));
            Unk_74 = s.Serialize<ushort>(Unk_74, name: nameof(Unk_74));
            Unk_76 = s.Serialize<ushort>(Unk_76, name: nameof(Unk_76));
            Unk_78 = s.Serialize<ushort>(Unk_78, name: nameof(Unk_78));
            Unk_80 = s.Serialize<ushort>(Unk_80, name: nameof(Unk_80));
            Unk_82 = s.Serialize<ushort>(Unk_82, name: nameof(Unk_82));
            Unk_84 = s.Serialize<ushort>(Unk_84, name: nameof(Unk_84));
            Unk_86 = s.Serialize<ushort>(Unk_86, name: nameof(Unk_86));
            Unk_88 = s.Serialize<ushort>(Unk_88, name: nameof(Unk_88));
            Unk_90 = s.Serialize<ushort>(Unk_90, name: nameof(Unk_90));
            Unk_92 = s.Serialize<ushort>(Unk_92, name: nameof(Unk_92));
            Unk_94 = s.Serialize<ushort>(Unk_94, name: nameof(Unk_94));

            Type = s.Serialize<EventType>(Type, name: nameof(Type));
            Unk_98 = s.SerializeArray<byte>(Unk_98, 5, name: nameof(Unk_98));
            Unk_103 = s.Serialize<byte>(Unk_103, name: nameof(Unk_103));

            OffsetBX = s.Serialize<byte>(OffsetBX, name: nameof(OffsetBX));
            OffsetBY = s.Serialize<byte>(OffsetBY, name: nameof(OffsetBY));

            Unk_106 = s.Serialize<ushort>(Unk_106, name: nameof(Unk_106));

            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));

            Unk_110 = s.Serialize<ushort>(Unk_110, name: nameof(Unk_110));
            Unk_112 = s.Serialize<uint>(Unk_112, name: nameof(Unk_112));

            OffsetHY = s.Serialize<byte>(OffsetHY, name: nameof(OffsetHY));
            FollowSprite = s.Serialize<byte>(FollowSprite, name: nameof(FollowSprite));
            HitPoints = s.Serialize<ushort>(HitPoints, name: nameof(HitPoints));
            Layer = s.Serialize<byte>(Layer, name: nameof(Layer));
            HitSprite = s.Serialize<byte>(HitSprite, name: nameof(HitSprite));

            Unk_122 = s.Serialize<byte>(Unk_122, name: nameof(Unk_122));
            Unk_123 = s.Serialize<byte>(Unk_123, name: nameof(Unk_123));
            Unk_124 = s.Serialize<byte>(Unk_124, name: nameof(Unk_124));
            Unk_125 = s.Serialize<byte>(Unk_125, name: nameof(Unk_125));
            TempLayer = s.Serialize<byte>(TempLayer, name: nameof(TempLayer));
            Unk_127 = s.Serialize<byte>(Unk_127, name: nameof(Unk_127));

            Unk_128 = s.Serialize<byte>(Unk_128, name: nameof(Unk_128));

            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));

            Unk_130 = s.Serialize<ushort>(Unk_130, name: nameof(Unk_130));
        }
    }
}