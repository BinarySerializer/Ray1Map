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
        /// The common tile-sets
        /// </summary>
        public Common_Tileset[] TileSet { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public Common_Tile[] Tiles { get; set; }

        /// <summary>
        /// The event data for every event
        /// </summary>
        public List<Common_EventData> EventData { get; set; }
    }
}