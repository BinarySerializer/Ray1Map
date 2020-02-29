using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_Manager : IGameManager
    {
        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <returns>The level file path</returns>
        public string GetLevelFilePath(string basePath, World world, int level)
        {
            return Path.Combine(GetWorldFolderPath(basePath, world), $"{GetWorldName(world)}{level:00}.XXX");
        }

        /// <summary>
        /// Gets the name for the world
        /// </summary>
        /// <returns>The world name</returns>
        public string GetWorldName(World world)
        {
            switch (world)
            {
                case World.Jungle:
                    return "JUN";
                case World.Music:
                    return "MUS";
                case World.Mountain:
                    return "MON";
                case World.Image:
                    return "IMG";
                case World.Cave:
                    return "CAV";
                case World.Cake:
                    return "CAK";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(string basePath, World world)
        {

            return Path.Combine(basePath, "RAY", GetWorldName(world));
        }

        /// <summary>
        /// Gets the level count for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The level count</returns>
        public int GetLevelCount(string basePath, World world)
        {
            var worldPath = GetWorldFolderPath(basePath, world);

            return Directory.EnumerateFiles(worldPath, "*.XXX", SearchOption.TopDirectoryOnly).Count(x => Path.GetFileNameWithoutExtension(x)?.Length == 5);
        }

        // TODO: Replace this entire function with the class PS1_R1_WorldFile to store the data in
        public Texture2D ReadTileSet(string basePath, World world)
        {
            var fileStream = new FileStream(Path.Combine(GetWorldFolderPath(basePath, world), $"{GetWorldName(world)}.XXX"), FileMode.Open);
            byte[] file = new byte[fileStream.Length];
            fileStream.Read(file, 0, (int)fileStream.Length);
            fileStream.Close();

            int off_tiles = BitConverter.ToInt32(file, 0x18);
            int off_palette = BitConverter.ToInt32(file, 0x1C);
            int off_assign = BitConverter.ToInt32(file, 0x20);
            int off_end = BitConverter.ToInt32(file, 0x24);

            // Palettes
            var palettes = new List<Color[]>();
            for (int i = off_palette; i < off_assign;)
            {
                var p = new Color[256];
                for (int c = 0; c < 256; c++, i += 2)
                {
                    uint colour16 = BitConverter.ToUInt16(file, i); // ABGR 1555
                    byte r = (byte)((colour16 & 0x1F) << 3);
                    byte g = (byte)(((colour16 & 0x3E0) >> 5) << 3);
                    byte b = (byte)(((colour16 & 0x7C00) >> 10) << 3);

                    if (r + g + b > 0)
                        p[c] = new Color((float)r / 255, (float)g / 255, (float)b / 255, 1);
                    else
                        p[c] = new Color(0, 0, 0, 0);
                }
                palettes.Add(p);
            }

            int tile = 0;
            int tileCount = off_end - off_assign;
            int width = 256;
            int height = (off_palette - off_tiles) / width;
            Color[] pixels = new Color[width * height];

            for (int yB = 0; yB < height; yB += 16)
            for (int xB = 0; xB < width; xB += 16, tile++)
            for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
            {
                if (tile >= tileCount)
                    goto End;
                int pixel = x + xB + (y + yB) * width;
                pixels[pixel] = palettes
                        [file[off_assign + tile]]
                    [file[off_tiles + pixel]];
            }
            End:
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <returns>The level</returns>
        public Common_Lev LoadLevel(string basePath, World world, int level) {
            // Open the level
            var levelData = FileFactory.Read<PS1_R1_LevFile>(GetLevelFilePath(basePath, world, level));

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev();

            c.Width = levelData.Width;
            c.Height = levelData.Height;
            c.Events = levelData.Events;
            c.RaymanPos = levelData.RaymanPos;

            c.TileSet = new Common_Tileset[4];
            // TODO: Other tilesets should probably be initialized here too
            Common_Tileset tileset1 = new Common_Tileset(ReadTileSet(basePath, world));
            c.TileSet[1] = tileset1;

            c.Tiles = ConvertTilesToCommon(levelData.Tiles, levelData.Width, levelData.Height);

            return c;
        }

        /// <summary>
        /// Converts a PS1_R1_Tile array to Common_Tile array
        /// </summary>
        /// <param name="tiles">Array of PS1 tiles</param>
        /// <param name="w">Level width</param>
        /// <param name="h">Level height</param>
        /// <returns>Common_Tile array</returns>
        public Common_Tile[] ConvertTilesToCommon(PS1_R1_Tile[] tiles, ushort w, ushort h) {
            Common_Tile[] finalTiles = new Common_Tile[w * h];

            int tileIndex = 0;
            for (int ty = 0; ty < (h); ty++) {
                for (int tx = 0; tx < (w); tx++) {
                    var graphicX = tiles[tileIndex].gX;
                    var graphicY = tiles[tileIndex].gY;

                    Common_Tile newTile = new Common_Tile();
                    newTile.palette = 1; // TODO: How to distinguish between which palette a tile should use?
                    newTile.x = tx;
                    newTile.y = ty;

                    newTile.cType = tiles[tileIndex].col;
                    newTile.gIndex = (16 * graphicY) + graphicX;

                    finalTiles[tileIndex] = newTile;

                    tileIndex++;
                }
            }

            return finalTiles;
        }
    }
}