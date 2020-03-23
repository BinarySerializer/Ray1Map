namespace R1Engine
{
    /// <summary>
    /// Map tile data for PC
    /// </summary>
    public class PC_MapTile : R1Serializable
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
        public override void SerializeImpl(SerializerObject s) {
            TextureIndex = s.Serialize<ushort>(TextureIndex, name: "TextureIndex");
            CollisionType = s.Serialize<TileCollisionType>(CollisionType, name: "CollisionType");
            Unknown1 = s.Serialize<byte>(Unknown1, name: "Unknown1");
            TransparencyMode = s.Serialize<PC_MapTileTransparencyMode>(TransparencyMode, name: "TransparencyMode");
            Unknown2 = s.Serialize<byte>(Unknown2, name: "Unknown2");
        }
    }
}