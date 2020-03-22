namespace R1Engine
{
    /// <summary>
    /// Tile texture data for PC
    /// </summary>
    public class PC_TileTexture : R1Serializable
    {
        /// <summary>
        /// The offset for this texture, as defines in the textures offset table. This value is not a part of the texture and has to be set manually.
        /// </summary>
        public uint TextureOffset { get; set; }

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
            ColorIndexes = s.SerializeArray(ColorIndexes, PC_Manager.CellSize * PC_Manager.CellSize, name: "ColorIndexes");
            Unknown1 = s.SerializeArray(Unknown1, 32, name: "Unknown1");
        }
    }
}