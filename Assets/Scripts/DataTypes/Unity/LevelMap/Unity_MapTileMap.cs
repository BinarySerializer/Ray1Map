using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Defines a common tile-set
    /// </summary>
    public class Unity_MapTileMap {
        /// <summary>
        /// Creates a tile set from a tile map
        /// </summary>
        /// <param name="tileMapColors">The tile map colors</param>
        /// <param name="tileMapWidth">The tile map width, in tiles</param>
        /// <param name="cellSize">The tile size</param>
        public Unity_MapTileMap(IList<ARGBColor> tileMapColors, int tileMapWidth, int cellSize) {
            // Create the tile array
            Tiles = new Unity_TileTexture[tileMapColors.Count / (cellSize * cellSize)];

            // Create each tile
            for (var index = 0; index < Tiles.Length; index++) {
                // Create the texture
                Texture2D tex = TextureHelpers.CreateTexture2D(cellSize, cellSize);

                // Get the tile x and y
                var tileY = (int)Math.Floor(index / (double)tileMapWidth);
                var tileX = (index - (tileMapWidth * tileY));

                var tileOffset = (tileY * tileMapWidth * cellSize * cellSize) + (tileX * cellSize);

                // Set every pixel
                for (int y = 0; y < cellSize; y++) {
                    for (int x = 0; x < cellSize; x++) {
                        tex.SetPixel(x, y, tileMapColors[(tileOffset + (y * cellSize * tileMapWidth + x))].GetColor());
                    }
                }

                // Apply the pixels
                tex.Apply();

                // Create a tile
                Tiles[index] = tex.CreateTile();
            }
        }

        /// <summary>
        /// Creates a tile set from a tile-set texture
        /// </summary>
        /// <param name="tileSet">The tile-set texture</param>
        /// <param name="cellSize">The tile size</param>
        public Unity_MapTileMap(Texture2D tileSet, int cellSize) {
            // Create the tile array
            Tiles = new Unity_TileTexture[(tileSet.width / cellSize) * (tileSet.height / cellSize)];

            // Keep track of the index
            var index = 0;

            // Extract every tile
            for (int y = 0; y < tileSet.height; y += cellSize) {
                for (int x = 0; x < tileSet.width; x += cellSize) {
                    // Create a tile
                    Tiles[index] = tileSet.CreateTile(new Rect(x, y, cellSize, cellSize));

                    index++;
                }
            }
        }

        /// <summary>
        /// Creates a tile set from a tile array
        /// </summary>
        /// <param name="tiles">The tiles in this set</param>
        public Unity_MapTileMap(Unity_TileTexture[] tiles) {
            Tiles = tiles;
        }

        /// <summary>
        /// The tiles in this set
        /// </summary>
        public Unity_TileTexture[] Tiles { get; }

        public Unity_AnimatedTile[] AnimatedTiles { get; set; }

        public int[] GBARRR_PalOffsets { get; set; }
    }
}