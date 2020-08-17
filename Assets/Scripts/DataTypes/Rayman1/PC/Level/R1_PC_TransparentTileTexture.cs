namespace R1Engine
{
    /// <summary>
    /// Transparent tile texture data for PC
    /// </summary>
    public class R1_PC_TransparentTileTexture : R1_PC_TileTexture
    {
        /// <summary>
        /// The alpha channel values for each texture pixel
        /// </summary>
        public byte[] Alpha { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            ColorIndexes = s.SerializeArray<byte>(ColorIndexes, Settings.CellSize * Settings.CellSize, name: nameof(ColorIndexes));
            Alpha = s.SerializeArray<byte>(Alpha, Settings.CellSize * Settings.CellSize, name: nameof(Alpha));
            TransparencyMode = s.Serialize<uint>(TransparencyMode, name: nameof(TransparencyMode));
            Unknown1 = s.SerializeArray<byte>(Unknown1, 28, name: nameof(Unknown1));
        }
    }
}