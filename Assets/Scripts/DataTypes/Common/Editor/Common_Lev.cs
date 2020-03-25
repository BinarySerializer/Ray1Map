using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// Common level data
    /// </summary>
    public class Common_Lev
    {
        /// <summary>
        /// The level width
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The level height
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The common tile-set where the first one is the collision and the other 3 the tile-sets for each palette
        /// </summary>
        public Common_Tileset[] TileSet { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public Common_Tile[] Tiles { get; set; }

        /// <summary>
        /// The events
        /// </summary>
        public List<Common_Event> Events { get; set; }
    }
}