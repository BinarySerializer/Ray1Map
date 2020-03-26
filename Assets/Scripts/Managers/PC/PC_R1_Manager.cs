using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_Manager : PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override int[] GetLevels(GameSettings settings) => Enumerable.Range(1, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(settings), "RAY??.LEV", SearchOption.TopDirectoryOnly).Count<string>()).ToArray<int>();

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(GameSettings settings) => GetDataPath() + GetWorldName(settings.World) + "/";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"BRAY.DAT";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => $"VIGNET.DAT";

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings) + $"RAY{settings.Level}.LEV";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"RAY{(int)settings.World + 1}.WLD";

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public override bool Has3Palettes => true;

        /// <summary>
        /// The available map names, if any
        /// </summary>
        public override Dictionary<World, Dictionary<int, string>> MapNames => R1Engine.MapNames.Ray1MapNames;

        #endregion
    }
}