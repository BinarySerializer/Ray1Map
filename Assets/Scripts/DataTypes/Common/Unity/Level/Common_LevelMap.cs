using System;
using System.Linq;
using UnityEngine;
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

        /// <summary>
        /// The transparency mode for the tiles in the tileset on PC
        /// </summary>
        public PC_MapTileTransparencyMode[] TileSetTransparencyModes { get; set; }

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
        /// <param name="settings">The game settings</param>
        /// <returns>The tile</returns>
        public Tile GetTile(Editor_MapTile mapTile, GameSettings settings)
        {
            // Get the tile index
            var tileIndex = (TileSetWidth * mapTile.Data.TileMapY) + mapTile.Data.TileMapX;

            // Get the tile array
            var tiles = TileSet[mapTile.PaletteIndex - 1].Tiles;

            // Check if it's out of bounds
            if (tileIndex >= tiles.Length)
            {
                // If it's out of bounds and the level is Jungle 27 in PS1 EDU, hard-code to 509, which is what the game uses there
                if (settings.EngineVersion == EngineVersion.RayEduPS1 && settings.World == World.Jungle && settings.Level == 27)
                {
                    tileIndex = 509;
                }
                else
                {
                    Debug.LogWarning($"Out of bounds tile with index {tileIndex} in {settings.GameModeSelection} - {settings.World}{settings.Level}");

                    tileIndex %= tiles.Length;
                }
            }

            // Return the tile
            return tiles[tileIndex];
        }

        public Editor_MapTile GetMapTile(int x, int y) => MapTiles.ElementAtOrDefault((Width * y) + x);

        #endregion
    }
}