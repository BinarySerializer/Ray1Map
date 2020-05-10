namespace R1Engine
{
    /// <summary>
    /// Map tile data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_MapTile : R1Serializable
    {
        /// <summary>
        /// The tile graphic index
        /// </summary>
        public ushort TileIndex { get; set; }

        /// <summary>
        /// The tile collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            TileIndex = s.Serialize<ushort>(TileIndex, name: nameof(TileIndex));
            CollisionType = (TileCollisionType)s.Serialize<ushort>((ushort)CollisionType, name: nameof(CollisionType));
        }
    }
}