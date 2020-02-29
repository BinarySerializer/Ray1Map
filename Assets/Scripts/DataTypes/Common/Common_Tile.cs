namespace R1Engine 
{
    // TODO: Clean up & add to common data types
    public class Common_Tile
    {
        // Link to a specific palette (in this case, a tilemap index)
        public int palette;
        // Coordinates of the tile
        public int x;
        public int y;
        // Graphical index of the tile
        public int gIndex;
        // Collision type
        public TileCollisionType cType;
    }
}