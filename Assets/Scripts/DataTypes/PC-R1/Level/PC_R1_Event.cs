using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_Event : ISerializableFile
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

        public void Deserialize(Stream stream)
        {
            DES = stream.Read<uint>();
            DES2 = stream.Read<uint>();
            DES3 = stream.Read<uint>();
            ETA = stream.Read<uint>();

            Unknown1 = stream.Read<uint>();
            Unknown2 = stream.Read<uint>();

            Unknown3 = stream.Read<byte>(16);

            XPosition = stream.Read<uint>();
            YPosition = stream.Read<uint>();

            Unknown4 = stream.Read<byte>(20);

            Unknown5 = stream.Read<byte>(28);

            Type = stream.Read<uint>();
            Unknown6 = stream.Read<uint>();

            OffsetBX = stream.Read<byte>();
            OffsetBY = stream.Read<byte>();

            Unknown7 = stream.Read<ushort>();

            SubEtat = stream.Read<byte>();
            Etat = stream.Read<byte>();

            Unknown8 = stream.Read<ushort>();
            Unknown9 = stream.Read<uint>();

            OffsetHY = stream.Read<byte>();
            FollowSprite = stream.Read<byte>();
            HitPoints = stream.Read<ushort>();
            UnkGroup = stream.Read<byte>();
            HitSprite = stream.Read<byte>();

            Unknown10 = stream.Read<byte>(6);

            Unknown11 = stream.Read<byte>();
            FollowEnabled = stream.Read<byte>();
            Unknown12 = stream.Read<ushort>();
        }

        public void Serialize(Stream stream)
        {
            stream.Write(DES);
            stream.Write(DES2);
            stream.Write(DES3);
            stream.Write(ETA);

            stream.Write(Unknown1);
            stream.Write(Unknown2);

            stream.Write(Unknown3);

            stream.Write(XPosition);
            stream.Write(YPosition);

            stream.Write(Unknown4);
            stream.Write(Unknown5);

            stream.Write(Type);
            stream.Write(Unknown6);

            stream.Write(OffsetBX);
            stream.Write(OffsetBY);

            stream.Write(Unknown7);

            stream.Write(SubEtat);
            stream.Write(Etat);

            stream.Write(Unknown8);
            stream.Write(Unknown9);

            stream.Write(OffsetHY);
            stream.Write(FollowSprite);
            stream.Write(HitPoints);
            stream.Write(UnkGroup);
            stream.Write(HitSprite);

            stream.Write(Unknown10);
            stream.Write(Unknown11);

            stream.Write(FollowEnabled);

            stream.Write(Unknown12);
        }
    }
}