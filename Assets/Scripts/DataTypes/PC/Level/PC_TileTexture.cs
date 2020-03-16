namespace R1Engine
{
    /// <summary>
    /// Tile texture data for PC
    /// </summary>
    public class PC_TileTexture : IBinarySerializable
    {
        /// <summary>
        /// The offset for this texture, as defines in the textures offset table. This value is not a part of the texture and has to be set manually.
        /// </summary>
        public uint Offset { get; set; }

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
        public virtual void Serialize(BinarySerializer serializer)
        {
            serializer.SerializeArray<byte>(nameof(ColorIndexes), PC_Manager.CellSize * PC_Manager.CellSize);
            serializer.SerializeArray<byte>(nameof(Unknown1), 32);
        }
    }
}