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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public virtual void Deserialize(BinaryDeserializer deserializer)
        {
            // Set the color array
            ColorIndexes = deserializer.ReadArray<byte>(PC_R1_Manager.CellSize * PC_R1_Manager.CellSize);
            Unknown1 = deserializer.ReadArray<byte>(32);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public virtual void Serialize(BinarySerializer serializer)
        {
            serializer.Write(ColorIndexes);
            serializer.Write(Unknown1);
        }
    }
}