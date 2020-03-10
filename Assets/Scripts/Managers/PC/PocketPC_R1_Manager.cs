using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Ultimate (Pocket PC)
    /// </summary>
    public class PocketPC_R1_Manager : PC_R1_Manager
    {
        #region Values and paths

        /// <summary>
        /// The file mode to use
        /// </summary>
        public override FileMode FileMode => FileMode.CompressedGZip;

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override int[] GetLevels(GameSettings settings) => Enumerable.Range(1, Directory.EnumerateFiles(GetWorldFolderPath(settings), $"{GetShortWorldName(settings.World)}??.lev.gz", SearchOption.TopDirectoryOnly).Count()).ToArray();

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => Path.Combine(GetDataPath(settings.GameDirectory), $"bray.dat.gz");

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public override string GetAllfixFilePath(GameSettings settings) => Path.Combine(GetDataPath(settings.GameDirectory), $"allfix.dat.gz");

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => Path.Combine(GetWorldFolderPath(settings), $"{GetShortWorldName(settings.World)}{settings.Level}.lev.gz");

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => Path.Combine(GetDataPath(settings.GameDirectory), $"ray{(int)settings.World + 1}.wld.gz");

        #endregion
    }
}