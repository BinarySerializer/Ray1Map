using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// Defines a common tile-set
    /// </summary>
    public class Common_Tileset
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="tiles">The tiles in this set</param>
        public Common_Tileset(Tile[] tiles)
        {
            Tiles = tiles;
        }

        /// <summary>
        /// The tiles in this set
        /// </summary>
        public Tile[] Tiles { get; }
    }
}