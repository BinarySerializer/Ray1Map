namespace R1Engine
{
    /// <summary>
    /// Map block data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_MapBlock : R1Serializable
    {
        /// <summary>
        /// The map width, in tiles
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The map height, in tiles
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public PS1_R1_MapTile[] Tiles { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize map size
            Width = s.Serialize<ushort>(Width, name: "Width");
            Height = s.Serialize<ushort>(Height, name: "Height");

            // Serialize tiles
            Tiles = s.SerializeObjectArray<PS1_R1_MapTile>(Tiles, Width * Height, name: "Tiles");
        }
    }
}