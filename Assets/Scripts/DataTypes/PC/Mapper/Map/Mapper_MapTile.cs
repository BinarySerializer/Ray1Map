namespace R1Engine
{
    /// <summary>
    /// Map tile data for the Mapper
    /// </summary>
    public class Mapper_MapTile : IBinarySerializable
    {
        /// <summary>
        /// The tile texture index
        /// </summary>
        public ushort TileIndex { get; set; }

        /// <summary>
        /// The tile collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Serialize(nameof(TileIndex));

            if (serializer.Mode == SerializerMode.Read)
                CollisionType = (TileCollisionType)serializer.Read<ushort>();
            else
                serializer.Write((ushort)CollisionType);
        }
    }
}