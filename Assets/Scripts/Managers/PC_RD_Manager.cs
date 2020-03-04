using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_Manager : IGameManager
    {
        /// <summary>
        /// Gets the localization files for each event, with the language tag as the key
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <returns>The localization files</returns>
        public Dictionary<string, PC_RD_EventLocFile[]> GetEventLocFiles(string basePath)
        {
            var pcDataDir = Path.Combine(basePath, "PCMAP");
            
            var output = new Dictionary<string, PC_RD_EventLocFile[]>();

            foreach (var langDir in Directory.GetDirectories(pcDataDir, "???", SearchOption.TopDirectoryOnly))
            {
                output.Add(Path.GetFileName(langDir), Directory.GetFiles(langDir, "*.wld", SearchOption.TopDirectoryOnly).Select(locFile => FileFactory.Read<PC_RD_EventLocFile>(locFile)).ToArray());
            }

            return output;
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

        /// <summary>
        /// Gets the level count for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The level count</returns>
        public int GetLevelCount(string basePath, World world)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="eventInfoData">The loaded event info data</param>
        /// <returns>The level</returns>
        public Common_Lev LoadLevel(string basePath, World world, int level, EventInfoData[] eventInfoData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="commonLevelData">The common level data</param>
        public void SaveLevel(string basePath, World world, int level, Common_Lev commonLevelData)
        {
            throw new NotImplementedException();
        }
    }
}