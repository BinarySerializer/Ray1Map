using System;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Image descriptor data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_ImageDescriptor : ISerializableFile
    {
        /// <summary>
        /// The image offset in the image data
        /// </summary>
        public uint ImageOffset { get; set; }

        // Index?
        public byte Unknown1 { get; set; }

        /// <summary>
        /// The outer image width (including the margins)
        /// </summary>
        public byte OuterWidth { get; set; }

        /// <summary>
        /// The outer image height (including the margins)
        /// </summary>
        public byte OuterHeight { get; set; }

        /// <summary>
        /// The inner image width
        /// </summary>
        public byte InnerWidth { get; set; }

        /// <summary>
        /// The inner image height
        /// </summary>
        public byte InnerHeight { get; set; }

        public byte Unknown2 { get; set; }

        public byte Unknown3 { get; set; }

        public byte Unknown4{ get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            ImageOffset = stream.Read<uint>();
            Unknown1 = stream.Read<byte>();
            Unknown1 = stream.Read<byte>();
            OuterWidth = stream.Read<byte>();
            OuterHeight = stream.Read<byte>();
            InnerWidth = stream.Read<byte>();
            InnerHeight = stream.Read<byte>();
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