using System;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Educational (PC)
    /// </summary>
    public class PC_EDU_Manager : PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetVolumePath(settings) + $"{GetShortWorldName(settings.World)}{settings.Level:00}.lev";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetVolumePath(settings) + $"RAY{((int)settings.World + 1):00}.WLD";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => GetVolumePath(settings) + $"VIGNET.DAT";

        /// <summary>
        /// Gets the volume data path
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The volume data path</returns>
        public string GetVolumePath(GameSettings settings) => GetDataPath() + settings.EduVolume + "/";

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public override bool Has3Palettes => true;

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override int[] GetLevels(GameSettings settings) => Directory.EnumerateFiles(settings.GameDirectory + "/" + GetVolumePath(settings), $"{GetShortWorldName(settings.World)}??.lev", SearchOption.TopDirectoryOnly).Select(x => Int32.Parse(Path.GetFileNameWithoutExtension(x).Substring(3))).ToArray();

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        public override string[] GetEduVolumes(GameSettings settings) => Directory.GetDirectories(settings.GameDirectory + "/" + GetDataPath(), "???", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToArray();

        #endregion
    }
}