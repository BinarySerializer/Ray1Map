using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Base .xxx game manager for PS1
    /// </summary>
    public abstract class PS1_BaseXXX_Manager : PS1_Manager
    {
        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public virtual string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.World) + $"{GetWorldName(settings.World)}{settings.Level:00}.XXX";

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public virtual string GetAllfixFilePath(GameSettings settings) => GetDataPath() + $"RAY.XXX";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public virtual string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"INI.XXX";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public virtual string GetWorldFilePath(GameSettings settings) => GetWorldFolderPath(settings.World) + $"{GetWorldName(settings.World)}.XXX";

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(World world) => GetDataPath() + GetWorldName(world) + "/";

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public string GetDataPath() => "RAY/";

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"*.XXX", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Where(x => x.Length == 5)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray();
    }
}