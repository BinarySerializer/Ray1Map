namespace R1Engine
{
    /// <summary>
    /// Tile texture data for PC
    /// </summary>
    public class R1_PC_TileTexture : R1Serializable
    {
        /// <summary>
        /// The color indexes for this texture
        /// </summary>
        public byte[] ColorIndexes { get; set; }

        /// <summary>
        /// A flag determining the tile transparency mode
        /// </summary>
        public uint TransparencyMode { get; set; }

        /// <summary>
        /// Unknown array of bytes. Appears to be leftover garbage data.
        /// </summary>
        public byte[] Unknown1 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            ColorIndexes = s.SerializeArray<byte>(ColorIndexes, Settings.CellSize * Settings.CellSize, name: nameof(ColorIndexes));
            TransparencyMode = s.Serialize<uint>(TransparencyMode, name: nameof(TransparencyMode));
            Unknown1 = s.SerializeArray<byte>(Unknown1, 28, name: nameof(Unknown1));
        }
    }
}