using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Event : ISerializableFile
    {
        public byte[] Unknown1 { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public ushort XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public ushort YPosition { get; set; }

        public byte[] Unknown2 { get; set; }

        public ushort Unknown3 { get; set; }

        public ushort Unknown4 { get; set; }

        // Always 254?
        public ushort Unknown5 { get; set; }

        public byte[] Unknown6 { get; set; }

        public byte OffsetBX { get; set; }

        public byte OffsetBY { get; set; }

        public ushort Unknown7 { get; set; }

        public ushort Etat { get; set; }

        public ushort SubEtat { get; set; }

        public ushort Unknown8 { get; set; }

        public ushort Unknown9 { get; set; }

        public byte OffsetHY { get; set; }

        public byte FollowSprite { get; set; }

        public ushort Hitpoints { get; set; }
        
        public byte UnkGroup { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public byte Type { get; set; }

        // NOTE: Maybe a byte?
        public ushort HitSprite { get; set; }

        public byte[] Unknown10 { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            Unknown1 = stream.ReadBytes(28);

            XPosition = stream.Read<ushort>();
            YPosition = stream.Read<ushort>();

            Unknown2 = stream.ReadBytes(16);
            Unknown3 = stream.Read<ushort>();
            Unknown4 = stream.Read<ushort>();
            Unknown5 = stream.Read<ushort>();
            Unknown6 = stream.ReadBytes(28);

            OffsetBX = stream.Read<byte>();
            OffsetBY = stream.Read<byte>();
            
            Unknown7 = stream.Read<ushort>();

            Etat = stream.Read<ushort>();
            SubEtat = stream.Read<ushort>();

            Unknown8 = stream.Read<ushort>();
            Unknown9 = stream.Read<ushort>();

            OffsetHY = stream.Read<byte>();
            FollowSprite = stream.Read<byte>();

            Hitpoints = stream.Read<ushort>();

            UnkGroup = stream.Read<byte>();

            Type = stream.Read<byte>();

            HitSprite = stream.Read<ushort>();

            Unknown10 = stream.ReadBytes(10);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            stream.Write(Unknown1);

            stream.Write(XPosition);
            stream.Write(YPosition);

            stream.Write(Unknown2);
            stream.Write(Unknown3);
            stream.Write(Unknown4);
            stream.Write(Unknown5);
            stream.Write(Unknown6);

            stream.Write(OffsetBX);
            stream.Write(OffsetBY);

            stream.Write(Unknown7);

            stream.Write(Etat);
            stream.Write(SubEtat);

            stream.Write(Unknown8);
            stream.Write(Unknown9);

            stream.Write(OffsetHY);
            stream.Write(FollowSprite);

            stream.Write(Hitpoints);

            stream.Write(UnkGroup);

            stream.Write(Type);

            stream.Write(HitSprite);

            stream.Write(Unknown10);
        }
    }
}