using System;
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

        /// <summary>
        /// The event type
        /// </summary>
        public byte Type { get; set; }

        public byte[] Unknown3 { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            Unknown1 = stream.ReadBytes(28);

            XPosition = stream.Read<ushort>();
            YPosition = stream.Read<ushort>();

            Unknown2 = stream.ReadBytes(67);

            Type = stream.Read<byte>();

            Unknown3 = stream.ReadBytes(12);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}