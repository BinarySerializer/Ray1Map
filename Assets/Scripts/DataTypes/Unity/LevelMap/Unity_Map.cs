using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// Common level map data
    /// </summary>
    public class Unity_Map
    {
        #region Public Properties

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
        public Unity_MapTileMap[] TileSet { get; set; }

        /// <summary>
        /// The transparency mode for the tiles in the tileset on PC
        /// </summary>
        public R1_PC_MapTileTransparencyMode[] TileSetTransparencyModes { get; set; }

        /// <summary>
        /// Tile texture offset table for PC
        /// </summary>
        public Pointer[] PCTileOffsetTable { get; set; }

        /// <summary>
        /// The map tiles
        /// </summary>
        public Unity_Tile[] MapTiles { get; set; }

        /// <summary>
        /// Indicates if the layer should be in front of objects
        /// </summary>
        public bool IsForeground { get; set; }

        public float? Alpha { get; set; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the tile for the specific map tile
        /// </summary>
        /// <param name="mapTile">The map tile</param>
        /// <param name="settings">The game settings</param>
        /// <returns>The tile</returns>
        public Unity_TileTexture GetTile(Unity_Tile mapTile, GameSettings settings, int? tileIndexOverride = null)
        {
            // Get the tile index
            int tileIndex;
            if (tileIndexOverride.HasValue) {
                tileIndex = (TileSetWidth * tileIndexOverride.Value) + mapTile.Data.TileMapX;
            } else {
                tileIndex = (TileSetWidth * mapTile.Data.TileMapY) + mapTile.Data.TileMapX;
            }
            // Get the tile array
            var tiles = TileSet[mapTile.PaletteIndex - 1].Tiles;

            // Check if it's out of bounds
            if (tileIndex >= tiles.Length)
            {
                // If it's out of bounds and the level is Jungle 27 in PS1 EDU, hard-code to 509, which is what the game uses there
                if (settings.EngineVersion == EngineVersion.R1_PS1_Edu && settings.R1_World == R1_World.Jungle && settings.Level == 27)
                {
                    tileIndex = 509;
                }
                else
                {
                    Debug.LogWarning($"Out of bounds tile with index {tileIndex} in {settings.GameModeSelection} - {settings.World}{settings.Level}");

                    tileIndex = 0;
                }
            }

            // Return the tile
            return tiles[tileIndex];
        }

        public Unity_AnimatedTile.Instance GetAnimatedTile(Unity_Tile mapTile, GameSettings settings) {
            // Get the tile index
            var tileIndex = (TileSetWidth * mapTile.Data.TileMapY) + mapTile.Data.TileMapX;
            var tileset = TileSet[mapTile.PaletteIndex - 1];

            if (tileset.AnimatedTiles != null) {
                foreach (var at in tileset.AnimatedTiles) {
                    if (at.TileIndices?.Length > 0 && at.TileIndices[0] == tileIndex){
                        //int index = Array.IndexOf(at.TileIndices, tileIndex);
                        int index = 0;
                        if (index >= 0) {
                            return new Unity_AnimatedTile.Instance(at, index);
                        }
                    }
                }
                return null;
            }
            return null;
        }

        public Unity_Tile GetMapTile(int x, int y) => MapTiles.ElementAtOrDefault((Width * y) + x);

        #endregion
    }
}