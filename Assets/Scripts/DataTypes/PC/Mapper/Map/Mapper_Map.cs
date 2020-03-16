namespace R1Engine
{
    /// <summary>
    /// Map data for the Mapper
    /// </summary>
    public class Mapper_Map : IBinarySerializable
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
        public void Serialize(BinarySerializer serializer)
        {
            // Serialize map size
            serializer.Serialize(nameof(Width));
            serializer.Serialize(nameof(Height));

            // Serialize tiles
            serializer.SerializeArray<Mapper_MapTile>(nameof(Tiles), Width * Height);
        }

        #endregion
    }
}