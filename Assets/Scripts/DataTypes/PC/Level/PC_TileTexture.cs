namespace R1Engine
{
    /// <summary>
    /// Tile texture data for PC
    /// </summary>
    public class PC_TileTexture : R1Serializable
    {
        /// <summary>
        /// The color indexes for this texture
        /// </summary>
        public byte[] ColorIndexes { get; set; }

        /// <summary>
        /// Unknown array of bytes, always 32 in length
        /// </summary>
        public byte[] Unknown1 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            ColorIndexes = s.SerializeArray<byte>(ColorIndexes, Settings.CellSize * Settings.CellSize, name: nameof(ColorIndexes));
            Unknown1 = s.SerializeArray<byte>(Unknown1, 32, name: nameof(Unknown1));
        }
    }
}