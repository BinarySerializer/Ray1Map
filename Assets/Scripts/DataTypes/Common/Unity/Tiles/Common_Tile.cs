using System;

namespace R1Engine 
{
    /// <summary>
    /// A map tile
    /// </summary>
    public class Common_Tile : ICloneable
    {
        /// <summary>
        /// The palette index, between 1 and 3
        /// </summary>
        public int PaletteIndex { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public int XPosition { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public int YPosition { get; set; }

        /// <summary>
        /// The tile index from the tile-set
        /// </summary>
        public int TileSetGraphicIndex { get; set; }

        /// <summary>
        /// The collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// Creates a clone of the object
        /// </summary>
        /// <returns>The object clone</returns>
        public Common_Tile CloneTile()
        {
            return (Common_Tile)Clone();
        }

        /// <summary>
        /// Creates a clone of the object
        /// </summary>
        /// <returns>The object clone</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}