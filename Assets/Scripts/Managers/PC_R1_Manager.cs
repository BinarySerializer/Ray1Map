using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_Manager : IGameManager
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
            return Path.Combine(GetWorldFolderPath(basePath, world), $"RAY{level}.LEV");
        }

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(string basePath, World world)
        {
            // Helper method for getting the folder name for the world
            string GetWorldFolderName()
            {
                switch (world)
                {
                    case World.Jungle:
                        return "JUNGLE";
                    case World.Music:
                        return "MUSIC";
                    case World.Mountain:
                        return "MOUNTAIN";
                    case World.Image:
                        return "IMAGE";
                    case World.Cave:
                        return "CAVE";
                    case World.Cake:
                        return "CAKE";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(world), world, null);
                }
            }

            return Path.Combine(basePath, "PCMAP", GetWorldFolderName());
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

            return Directory.EnumerateFiles(worldPath, "RAY??.LEV", SearchOption.TopDirectoryOnly).Count();
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <returns>The level</returns>
        public Common_Lev LoadLevel(string basePath, World world, int level)
        {
            // Read the level data
            var levelData = FileFactory.Read<PC_R1_LevFile>(GetLevelFilePath(basePath, world, level));

            // Convert levelData to common level format
            Common_Lev c = new Common_Lev();
            c.Width = levelData.MapWidth;
            c.Height = levelData.MapHeight;
            // TODO: Clean up by making a common event class
            c.Events = levelData.Events.Select(x => new Event() { pos = new PxlVec((ushort)x.XPosition, (ushort)x.YPosition) }).ToArray();

            c.TileSet = new Common_Tileset[4];
            // TODO: Other tilesets should probably be initialized here too
            Common_Tileset tileset1 = new Common_Tileset(ReadTileSet(basePath, world));
            c.TileSet[1] = tileset1;

            var til = new PC_R1_Tile[levelData.Tiles.Length];
            // Set the tiles
            for (int y = 0; y < levelData.MapHeight; y++) {
                for (int x = 0; x < levelData.MapWidth; x++) {
                    var index = y * levelData.MapWidth + x;

                    til[index] = new PC_R1_Tile() {
                        col = levelData.Tiles[index].CollisionType,
                        gX = x,
                        gY = y
                    };
                }
            }

            c.Tiles = ConvertTilesToCommon(til, levelData.MapWidth, levelData.MapHeight);


            return c;
        }

        /// <summary>
        /// Converts a PC_R1_Tile array to Common_Tile array
        /// </summary>
        /// <param name="tiles">Array of PC tiles</param>
        /// <param name="w">Level width</param>
        /// <param name="h">Level height</param>
        /// <returns>Common_Tile array</returns>
        public Common_Tile[] ConvertTilesToCommon(PC_R1_Tile[] tiles, ushort w, ushort h) {
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

        public Texture2D ReadTileSet(string basePath, World world) {

            // TODO: Make this (or PC_R1_WorldFile) to generate a Texture2D of the map, for each 3 palettes

            return null;
        }
    }
}