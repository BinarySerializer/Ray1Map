namespace R1Engine
{
    /// <summary>
    /// Common map data
    /// </summary>
    public class MapData : R1Serializable
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
        public MapTile[] Tiles { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize map size
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            // Serialize tiles
            Tiles = s.SerializeObjectArray<MapTile>(Tiles, Width * Height, name: nameof(Tiles));
        }

        #endregion
    }
}