﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map
{
    /// <summary>
    /// Defines a common tile-set
    /// </summary>
    public class Unity_TileSet 
    {
        /// <summary>
        /// Creates a tile set from a tile map
        /// </summary>
        /// <param name="tileMapColors">The tile map colors</param>
        /// <param name="tileMapWidth">The tile map width, in tiles</param>
        /// <param name="cellSize">The tile size</param>
        public Unity_TileSet(IList<BaseColor> tileMapColors, int tileMapWidth, int cellSize) {
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
        public Unity_TileSet(Texture2D tileSet, int cellSize) {
            // Create the tile array
            Tiles = new Unity_TileTexture[(tileSet.width / cellSize) * (tileSet.height / cellSize)];

            // Keep track of the index
            var index = 0;

            // Extract every tile
            for (int y = 0; y < tileSet.height; y += cellSize) {
                for (int x = 0; x < tileSet.width; x += cellSize) {
                    // Create a tile
                    Tiles[index] = tileSet.CreateTile(new RectInt(x, y, cellSize, cellSize));

                    index++;
                }
            }
        }

        /// <summary>
        /// Creates a tile set from a tile array
        /// </summary>
        /// <param name="tiles">The tiles in this set</param>
        public Unity_TileSet(Unity_TileTexture[] tiles) {
            Tiles = tiles;
        }

        /// <summary>
        /// Creates an empty tileset with a single transparent tile
        /// </summary>
        public Unity_TileSet(int cellSize) 
        {
            Tiles = new Unity_TileTexture[]
            {
                TextureHelpers.CreateTexture2D(cellSize, cellSize, true, true).CreateTile()
            };
        }

        /// <summary>
        /// Creates a tileset with a single colored tile
        /// </summary>
        public Unity_TileSet(int cellSize, Color color) 
        {
            var tex = TextureHelpers.CreateTexture2D(cellSize, cellSize);

            tex.SetPixels(Enumerable.Repeat(color, cellSize * cellSize).ToArray());

            tex.Apply();

            Tiles = new Unity_TileTexture[]
            {
                tex.CreateTile()
            };
        }

        public Unity_TileSet(byte[] imgData, Color[] pal, Unity_TextureFormat format, int tileWidth) 
        {
            int bpp = format.GetAttribute<Unity_TextureFormatInfoAttribute>().BPP;
            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = imgData.Length / tileSize;

            Tiles = new Unity_TileTexture[tilesetLength];

            for (int i = 0; i < tilesetLength; i++)
                Tiles[i] = new Unity_TileTexture(
                    new Unity_Texture(tileWidth, tileWidth, pal, imgData, format, i * tileSize));
        }
        public Unity_TileSet(byte[] imgData, Color[][] pal, Unity_TextureFormat format, int tileWidth) 
        {
            int bpp = format.GetAttribute<Unity_TextureFormatInfoAttribute>().BPP;
            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = imgData.Length / tileSize;

            Tiles = new Unity_TileTexture[tilesetLength];

            for (int i = 0; i < tilesetLength; i++)
                Tiles[i] = new Unity_TileTexture(
                    new Unity_MultiPaletteTexture(tileWidth, tileWidth, pal, imgData, format, i * tileSize));
        }

        /// <summary>
        /// The tiles in this set
        /// </summary>
        public Unity_TileTexture[] Tiles { get; }

        public Unity_AnimatedTile[] AnimatedTiles { get; set; }

        public int[] GBARRR_PalOffsets { get; set; }
        public int GBAIsometric_BaseLength { get; set; }
        public int SNES_BaseLength { get; set; }
    }
}