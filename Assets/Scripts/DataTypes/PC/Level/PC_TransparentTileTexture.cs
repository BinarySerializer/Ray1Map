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
        public override void SerializeImpl(SerializerObject s) {
            ColorIndexes = s.SerializeArray<byte>(ColorIndexes, PC_Manager.CellSize * PC_Manager.CellSize, name: nameof(ColorIndexes));
            Alpha = s.SerializeArray<byte>(Alpha, PC_Manager.CellSize * PC_Manager.CellSize, name: nameof(Alpha));
            Unknown1 = s.SerializeArray<byte>(Unknown1, 32, name: nameof(Unknown1));
        }
    }
}