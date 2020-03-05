namespace R1Engine
{
    /// <summary>
    /// Transparent tile texture data for PC
    /// </summary>
    public class PC_TransparentTileTexture : PC_TileTexture
    {
        /// <summary>
        /// The alpha channel values for each texture pixel
        /// </summary>
        public byte[] Alpha { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public override void Deserialize(BinaryDeserializer deserializer)
        {
            ColorIndexes = deserializer.ReadBytes(PC_R1_Manager.CellSize * PC_R1_Manager.CellSize);
            Alpha = deserializer.ReadBytes(PC_R1_Manager.CellSize * PC_R1_Manager.CellSize);
            Unknown1 = deserializer.ReadBytes(32);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            serializer.Write(ColorIndexes);
            serializer.Write(Alpha);
            serializer.Write(Unknown1);
        }
    }
}