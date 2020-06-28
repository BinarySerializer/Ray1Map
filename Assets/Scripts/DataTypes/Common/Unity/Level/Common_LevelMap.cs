using System.Linq;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// Common level map data
    /// </summary>
    public class Common_LevelMap
    {
        #region Public Properties

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
        /// The width of the tileset in tiles
        /// </summary>
        public int TileSetWidth { get; set; }

        /// <summary>
        /// The tile-sets, one for each palette
        /// </summary>
        public Common_Tileset[] TileSet { get; set; }

        // TODO: Use this when copying tiles from the template to set the flag correctly!
        /// <summary>
        /// The tile data for PC
        /// </summary>
        public PC_TileTexture[] PCTiles { get; set; }

        /// <summary>
        /// Tile texture offset table for PC
        /// </summary>
        public Pointer[] PCTileOffsetTable { get; set; }

        /// <summary>
        /// The map tiles
        /// </summary>
        public Editor_MapTile[] MapTiles { get; set; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the tile for the specific map tile
        /// </summary>
        /// <param name="mapTile">The map tile</param>
        /// <returns>The tile</returns>
        public Tile GetTile(Editor_MapTile mapTile) => TileSet[mapTile.PaletteIndex - 1].Tiles[(TileSetWidth * mapTile.Data.TileMapY) + mapTile.Data.TileMapX];

        public Editor_MapTile GetMapTile(int x, int y) => MapTiles.ElementAtOrDefault((Width * y) + x);

        #endregion
    }
}