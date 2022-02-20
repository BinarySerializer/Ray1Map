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
        /// Creates a tile set from a tile array
        /// </summary>
        /// <param name="tiles">The tiles in this set</param>
        public Unity_TileSet(Unity_TileTexture[] tiles)
        {
            Tiles = tiles;
        }

        /// <summary>
        /// Creates an empty tileset with a single transparent tile
        /// </summary>
        public Unity_TileSet(int tileWidth) : this(tileWidth, Color.clear) { }

        /// <summary>
        /// Creates a tileset with a single colored tile
        /// </summary>
        public Unity_TileSet(int tileWidth, Color color)
        {
            Color[] pixels = Enumerable.Repeat(color, tileWidth * tileWidth).ToArray();
            Tiles = new Unity_TileTexture(new Unity_Texture(pixels, tileWidth, tileWidth)).YieldToArray();
        }

        /// <summary>
        /// Creates a tile set from 2-dimensional tile-set pixel array. Only use this if the width is greater than 1.
        /// </summary>
        /// <param name="pixels">The tile set pixels</param>
        /// <param name="tileSetTilesWidth">The tile set width, in tiles</param>
        /// <param name="tileWidth">The tile size</param>
        public Unity_TileSet(IList<BaseColor> pixels, int tileSetTilesWidth, int tileWidth)
        {
            // Get the amount of tiles
            int tilesCount = pixels.Count / (tileWidth * tileWidth);

            // Create the tile array
            Tiles = new Unity_TileTexture[tilesCount];

            // Calculate dimensions
            int tileSetTilesHeight = tilesCount / tileSetTilesWidth;
            int tileSetWidth = tileSetTilesWidth * tileWidth;
            //int tileSetHeight = tileSetTilesHeight * tileSize;

            int tileIndex = 0;

            // Create each tile
            for (int tileY = 0; tileY < tileSetTilesHeight; tileY++)
            {
                for (int tileX = 0; tileX < tileSetTilesWidth; tileX++)
                {
                    int tileOffset = (tileY * tileWidth * tileSetWidth) + (tileX * tileWidth);

                    Color[] tilePixels = new Color[tileWidth * tileWidth];

                    // Set every pixel
                    for (int y = 0; y < tileWidth; y++)
                    {
                        for (int x = 0; x < tileWidth; x++)
                        {
                            tilePixels[y * tileWidth + x] = pixels[tileOffset + y * tileSetWidth + x].GetColor();
                        }
                    }

                    Tiles[tileIndex] = new Unity_TileTexture(new Unity_Texture(tilePixels, tileWidth, tileWidth));

                    tileIndex++;
                }
            }
        }

        /// <summary>
        /// Creates a tile set from 2-dimensional paletted image data. Only use this if the width is greater than 1.
        /// </summary>
        /// <param name="imgData">The tile set image data</param>
        /// <param name="palette">The palette</param>
        /// <param name="format">The image format</param>
        /// <param name="tileSetTilesWidth">The tile set width, in tiles</param>
        /// <param name="tileWidth">The tile size</param>
        public Unity_TileSet(byte[] imgData, Unity_Palette palette, Unity_TextureFormat format, int tileSetTilesWidth, int tileWidth)
        {
            int bpp = format.GetAttribute<Unity_TextureFormatInfoAttribute>().BPP;
            int bppFactor = (8 / bpp);

            // Get the amount of tiles
            int tilesCount = (imgData.Length / (tileWidth * tileWidth)) / bppFactor;

            // Create the tile array
            Tiles = new Unity_TileTexture[tilesCount];

            // Calculate dimensions
            int tileSetTilesHeight = tilesCount / tileSetTilesWidth;
            int tileSetWidth = tileSetTilesWidth * tileWidth;
            //int tileSetHeight = tileSetTilesHeight * tileSize;

            int tileIndex = 0;

            // Create each tile
            for (int tileY = 0; tileY < tileSetTilesHeight; tileY++)
            {
                for (int tileX = 0; tileX < tileSetTilesWidth; tileX++)
                {
                    int tileOffset = (tileY * tileWidth * tileSetWidth) + (tileX * tileWidth);

                    byte[] tileImgData = new byte[tileWidth * tileWidth / bppFactor];

                    // Set every pixel
                    for (int y = 0; y < tileWidth; y++)
                    {
                        for (int x = 0; x < tileWidth / bppFactor; x++)
                        {
                            tileImgData[y * tileWidth + x] = imgData[tileOffset + y * tileSetWidth + x];
                        }
                    }

                    Tiles[tileIndex] = new Unity_TileTexture(new Unity_PalettedTexture(tileImgData, palette, tileWidth, tileWidth, format));

                    tileIndex++;
                }
            }
        }

        /// <summary>
        /// Creates a tile set from a pixel array
        /// </summary>
        /// <param name="pixels">The tile set pixels</param>
        /// <param name="tileWidth">The tile size</param>
        public Unity_TileSet(Color[] pixels, int tileWidth)
        {
            int tileSize = tileWidth * tileWidth;
            int tilesetLength = pixels.Length / tileSize;

            Tiles = new Unity_TileTexture[tilesetLength];

            for (int i = 0; i < tilesetLength; i++)
                Tiles[i] = new Unity_TileTexture(new Unity_Texture(pixels, tileWidth, tileWidth, i * tileSize));
        }

        /// <summary>
        /// Creates a tile set from paletted image data
        /// </summary>
        /// <param name="imgData">The image data for the tile set</param>
        /// <param name="pal">The palette</param>
        /// <param name="format">The image format</param>
        /// <param name="tileWidth">The tile size</param>
        public Unity_TileSet(byte[] imgData, Unity_Palette pal, Unity_TextureFormat format, int tileWidth)
        {
            int bpp = format.GetAttribute<Unity_TextureFormatInfoAttribute>().BPP;
            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = imgData.Length / tileSize;

            Tiles = new Unity_TileTexture[tilesetLength];

            for (int i = 0; i < tilesetLength; i++)
                Tiles[i] = new Unity_TileTexture(new Unity_PalettedTexture(imgData, pal, tileWidth, tileWidth, format, i * tileSize));
        }

        /// <summary>
        /// Creates a tile set from paletted image data with multiple palettes
        /// </summary>
        /// <param name="imgData">The image data for the tile set</param>
        /// <param name="pal">The palettes</param>
        /// <param name="format">The image format</param>
        /// <param name="tileWidth">The tile size</param>
        public Unity_TileSet(byte[] imgData, Unity_Palette[] pal, Unity_TextureFormat format, int tileWidth)
        {
            int bpp = format.GetAttribute<Unity_TextureFormatInfoAttribute>().BPP;
            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = imgData.Length / tileSize;

            Tiles = new Unity_TileTexture[tilesetLength];

            for (int i = 0; i < tilesetLength; i++)
                Tiles[i] = new Unity_TileTexture(new Unity_MultiPalettedTexture(imgData, pal, tileWidth, tileWidth, format, i * tileSize));
        }

        /// <summary>
        /// Creates a tile set from a tile-set texture
        /// </summary>
        /// <param name="tileSet">The tile-set texture</param>
        /// <param name="tileWidth">The tile size</param>
        public Unity_TileSet(Texture2D tileSet, int tileWidth)
        {
            // Create the tile array
            Tiles = new Unity_TileTexture[(tileSet.width / tileWidth) * (tileSet.height / tileWidth)];

            // Keep track of the index
            var index = 0;

            // Extract every tile
            for (int y = 0; y < tileSet.height; y += tileWidth) 
            {
                for (int x = 0; x < tileSet.width; x += tileWidth) 
                {
                    // Create a tile
                    Tiles[index] = tileSet.CreateTile(new RectInt(x, y, tileWidth, tileWidth));

                    index++;
                }
            }
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