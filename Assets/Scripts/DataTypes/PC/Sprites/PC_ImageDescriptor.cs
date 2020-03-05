using System;

namespace R1Engine
{
    /// <summary>
    /// Image descriptor data for PC
    /// </summary>
    public class PC_ImageDescriptor : IBinarySerializable
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
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            ImageOffset = deserializer.Read<uint>();
            Unknown1 = deserializer.Read<byte>();
            OuterWidth = deserializer.Read<byte>();
            OuterHeight = deserializer.Read<byte>();
            InnerWidth = deserializer.Read<byte>();
            InnerHeight = deserializer.Read<byte>();
            Unknown2 = deserializer.Read<byte>();
            Unknown3 = deserializer.Read<byte>();
            Unknown4 = deserializer.Read<byte>();
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