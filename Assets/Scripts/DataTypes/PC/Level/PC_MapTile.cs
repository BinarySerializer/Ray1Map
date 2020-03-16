namespace R1Engine
{
    /// <summary>
    /// Map tile data for PC
    /// </summary>
    public class PC_MapTile : IBinarySerializable
    {
        /// <summary>
        /// The index for the texture for this cell
        /// </summary>
        public ushort TextureIndex { get; set; }

        /// <summary>
        /// The collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// An unknown byte
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// The transparency mode for this cell
        /// </summary>
        public PC_MapTileTransparencyMode TransparencyMode { get; set; }

        /// <summary>
        /// An unknown byte
        /// </summary>
        public byte Unknown2 { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Serialize(nameof(TextureIndex));
            serializer.Serialize(nameof(CollisionType));
            serializer.Serialize(nameof(Unknown1));
            serializer.Serialize(nameof(TransparencyMode));
            serializer.Serialize(nameof(Unknown2));
        }
    }
}