namespace R1Engine
{
    /// <summary>
    /// Background later data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundLayerInfo : IBinarySerializable
    {
        public uint Unknown1 { get; set; }

        /// <summary>
        /// The layer the background appears on
        /// </summary>
        public byte Layer { get; set; }

        /// <summary>
        /// The background width
        /// </summary>
        public byte Width { get; set; }

        /// <summary>
        /// The background height
        /// </summary>
        public byte Height { get; set; }

        public byte[] Unknown2 { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            Unknown1 = deserializer.Read<uint>();
            Layer = deserializer.Read<byte>();
            Width = deserializer.Read<byte>();
            Height = deserializer.Read<byte>();
            Unknown2 = deserializer.ReadArray<byte>(13);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Write(Unknown1);
            serializer.Write(Layer);
            serializer.Write(Width);
            serializer.Write(Height);
            serializer.Write(Unknown2);
        }
    }
}