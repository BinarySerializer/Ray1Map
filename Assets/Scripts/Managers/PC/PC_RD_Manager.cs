using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_Manager : PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => Path.Combine(GetDataPath(settings.GameDirectory), $"{GetShortWorldName(settings.World)}{settings.Level:00}.LEV");

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => Path.Combine(GetDataPath(settings.GameDirectory), $"RAY{((int)settings.World + 1):00}.WLD");

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public override bool Has3Palettes => false;

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override int[] GetLevels(GameSettings settings) => Enumerable.Range(1, Directory.EnumerateFiles(GetDataPath(settings.GameDirectory), $"{GetShortWorldName(settings.World)}??.LEV", SearchOption.TopDirectoryOnly).Count()).ToArray();

        #endregion

        #region Manager Methods

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
                output.Add(Path.GetFileName(langDir), Directory.GetFiles(langDir, "*.wld", SearchOption.TopDirectoryOnly).Select(locFile => FileFactory.Read<PC_RD_EventLocFile>(locFile, new GameSettings(GameMode.RayKit, basePath))).ToArray());
            }

            return output;
        }

        #endregion
    }
}