using System;
using System.IO;
using System.Linq;

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

            var commonLvl = new Common_Lev()
            {
                Width = levelData.MapWidth,
                Height = levelData.MapHeight,

                // TODO: Clean up by making a common event class
                Events = levelData.Events.Select(x => new Event()
                {
                    pos = new PxlVec((ushort)x.XPosition, (ushort)x.YPosition)
                }).ToArray(),
                
                // TODO: Clean up by making a common event class
                Tiles = new Type[levelData.Tiles.Length],
                
                // TODO: Need to set this or else it crashes
                TileSet = null
            };

            // Set the tiles
            for (int y = 0; y < levelData.MapHeight; y++)
            {
                for (int x = 0; x < levelData.MapWidth; x++)
                {
                    var index = y * levelData.MapWidth + x;

                    commonLvl.Tiles[index] = new Type()
                    {
                        col = levelData.Tiles[index].CollisionType,
                        gX = x,
                        gY = y
                    };
                }
            }

            return commonLvl;
        }
    }
}