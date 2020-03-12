using System;

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
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            TileIndex = deserializer.Read<ushort>();
            CollisionType = (TileCollisionType)deserializer.Read<ushort>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}