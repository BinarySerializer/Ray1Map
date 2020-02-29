namespace R1Engine 
{
    /// <summary>
    /// A map tile
    /// </summary>
    public class Common_Tile
    {
        /// <summary>
        /// The palette index, between 1 and 3
        /// </summary>
        public int palette { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// The y position
        /// </summary>
        public int y { get; set; }

        /// <summary>
        /// The tile index from the tile-set
        /// </summary>
        public int gIndex { get; set; }

        /// <summary>
        /// The collision type
        /// </summary>
        public TileCollisionType cType { get; set; }
    }
}