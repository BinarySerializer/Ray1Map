using System;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Animation layer data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_AnimationLayer : ISerializableFile
    {
        /// <summary>
        /// Indicates if the layer has horizontal reflection
        /// </summary>
        public bool HasHorizontalReflection { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public byte XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public byte YPosition { get; set; }

        /// <summary>
        /// The image index as it appears in the image block
        /// </summary>
        public byte ImageIndex { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            HasHorizontalReflection = stream.Read<bool>();
            XPosition = stream.Read<byte>();
            YPosition = stream.Read<byte>();
            ImageIndex = stream.Read<byte>();
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