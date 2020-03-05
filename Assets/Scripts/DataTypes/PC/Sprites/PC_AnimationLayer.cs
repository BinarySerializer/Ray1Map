using System;

namespace R1Engine
{
    /// <summary>
    /// Animation layer data for PC
    /// </summary>
    public class PC_AnimationLayer : IBinarySerializable
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
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            HasHorizontalReflection = deserializer.Read<bool>();
            XPosition = deserializer.Read<byte>();
            YPosition = deserializer.Read<byte>();
            ImageIndex = deserializer.Read<byte>();
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