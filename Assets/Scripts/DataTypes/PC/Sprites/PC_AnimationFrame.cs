using System;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Animation frame data for PC
    /// </summary>
    public class PC_AnimationFrame : IBinarySerializable
    {
        // TODO: Verify the values are correct

        /// <summary>
        /// The frame x position
        /// </summary>
        public byte XPosition { get; set; }

        /// <summary>
        /// The frame y position
        /// </summary>
        public byte YPosition { get; set; }

        /// <summary>
        /// The frame width
        /// </summary>
        public byte Width { get; set; }

        /// <summary>
        /// The frame height
        /// </summary>
        public byte Height { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            XPosition = deserializer.Read<byte>();
            YPosition = deserializer.Read<byte>();
            Width = deserializer.Read<byte>();
            Height = deserializer.Read<byte>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}