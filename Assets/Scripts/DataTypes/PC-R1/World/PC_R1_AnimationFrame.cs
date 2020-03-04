using System;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Animation frame data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_AnimationFrame : ISerializableFile
    {
        // X position?
        public byte Unknown1 { get; set; }

        // Y position?
        public byte Unknown2 { get; set; }

        // Width?
        public byte Unknown3 { get; set; }

        // Height?
        public byte Unknown4 { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            Unknown1 = stream.Read<byte>();
            Unknown2 = stream.Read<byte>();
            Unknown3 = stream.Read<byte>();
            Unknown4 = stream.Read<byte>();
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