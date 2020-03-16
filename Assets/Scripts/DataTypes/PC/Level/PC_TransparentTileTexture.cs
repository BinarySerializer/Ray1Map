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
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            serializer.SerializeArray<byte>(nameof(ColorIndexes), PC_Manager.CellSize * PC_Manager.CellSize);
            serializer.SerializeArray<byte>(nameof(Alpha), PC_Manager.CellSize * PC_Manager.CellSize);
            serializer.SerializeArray<byte>(nameof(Unknown1), 32);
        }
    }
}