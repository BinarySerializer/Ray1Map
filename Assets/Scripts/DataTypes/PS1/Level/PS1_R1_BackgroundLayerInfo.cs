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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Serialize(nameof(Unknown1));
            serializer.Serialize(nameof(Layer));
            serializer.Serialize(nameof(Width));
            serializer.Serialize(nameof(Height));
            serializer.SerializeArray<byte>(nameof(Unknown2), 13);
        }
    }
}