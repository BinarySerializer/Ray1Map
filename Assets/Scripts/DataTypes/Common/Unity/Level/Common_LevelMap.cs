namespace R1Engine
{
    /// <summary>
    /// Common level map data
    /// </summary>
    public class Common_LevelMap
    {
        /// <summary>
        /// The event scale factor
        /// </summary>
        public float ScaleFactor { get; set; } = 1;

        /// <summary>
        /// The level width
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The level height
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The common tile-sets
        /// </summary>
        public Common_Tileset[] TileSet { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public Common_Tile[] Tiles { get; set; }
    }
}