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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            TextureIndex = deserializer.Read<ushort>();
            CollisionType = (TileCollisionType)deserializer.Read<byte>();
            Unknown1 = deserializer.Read<byte>();
            TransparencyMode = (PC_MapTileTransparencyMode)deserializer.Read<byte>();
            Unknown2 = deserializer.Read<byte>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Write(TextureIndex);
            serializer.Write((byte)CollisionType);
            serializer.Write(Unknown1);
            serializer.Write((byte)TransparencyMode);
            serializer.Write(Unknown2);
        }
    }
}