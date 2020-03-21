namespace R1Engine
{
    /// <summary>
    /// Map data for the Mapper
    /// </summary>
    public class Mapper_Map : R1Serializable
    {
        #region Public Properties

        /// <summary>
        /// The width of the map, in cells
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The height of the map, in cells
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The tiles for the map
        /// </summary>
        public Mapper_MapTile[] Tiles { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            // Serialize map size
            Width = s.Serialize(Width, name: "Width");
            Height = s.Serialize(Height, name: "Height");

            // Serialize tiles
            Tiles = s.SerializeObjectArray<Mapper_MapTile>(Tiles, Width * Height, name: "Tiles");
        }

        #endregion
    }
}