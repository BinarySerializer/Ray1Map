namespace R1Engine
{
    /// <summary>
    /// Common level data
    /// </summary>
    public class Common_Lev
    {
        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public Common_Tileset[] TileSet { get; set; }

        public Common_Tile[] Tiles { get; set; }

        public Event[] Events { get; set; }


        // TODO: Remove?
        public PxlVec RaymanPos { get; set; }
    }
}