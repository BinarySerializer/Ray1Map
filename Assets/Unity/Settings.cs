using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace R1Engine
{
    [InitializeOnLoad]
    public static class Settings
    {
        #region Static Constructor

        static Settings()
        {
            GameDirectories = new Dictionary<GameMode, string>();

            Load();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The selected game mode
        /// </summary>
        public static GameMode Mode { get; set; }

        /// <summary>
        /// The selected game world
        /// </summary>
        public static World World { get; set; }

        /// <summary>
        /// The selected level index, starting from 1
        /// </summary>
        public static int Level { get; set; }

        /// <summary>
        /// True for the HD collision sheet to be used, false for the original Rayman Designer one to be used
        /// </summary>
        public static bool UseHDCollisionSheet { get; set; }

        /// <summary>
        /// The specified game directories for each mode
        /// </summary>
        public static Dictionary<GameMode, string> GameDirectories { get; set; }

        /// <summary>
        /// Gets the current directory based on the selected mode
        /// </summary>
        public static string CurrentDirectory => GameDirectories.TryGetValue(Mode, out var dir) ? dir : String.Empty;

        /// <summary>
        /// The string encoding to use for the game files
        /// </summary>
        public static Encoding StringEncoding { get; } = Encoding.GetEncoding(437);

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the settings
        /// </summary>
        public static void Save()
        {
            foreach (var mode in EnumHelpers.GetValues<GameMode>())
            {
                string dir = GameDirectories.ContainsKey(mode) ? GameDirectories[mode] : "";
                EditorPrefs.SetString("Directory" + mode, dir);
            }

            EditorPrefs.SetString("GameMode", Mode.ToString());
            EditorPrefs.SetString("SelectedWorld", World.ToString());
            EditorPrefs.SetInt("SelectedLevelFile", Level);
        }

        /// <summary>
        /// Loads the settings
        /// </summary>
        public static void Load()
        {
            foreach (var mode in EnumHelpers.GetValues<GameMode>())
            {
                string dir = GameDirectories.ContainsKey(mode) ? GameDirectories[mode] : "";
                GameDirectories[mode] = EditorPrefs.GetString("Directory" + mode, dir);
            }

            Mode = Enum.TryParse(EditorPrefs.GetString("GameMode", Mode.ToString()), out GameMode gameMode) ? gameMode : Mode;
            World = Enum.TryParse(EditorPrefs.GetString("SelectedWorld", World.ToString()), out World world) ? world : World;
            Level = EditorPrefs.GetInt("SelectedLevelFile", Level);
        }

        /// <summary>
        /// Gets a new manager instance for the specified mode
        /// </summary>
        /// <returns></returns>
        public static IGameManager GetManager()
        {
            switch (Mode)
            {
                case GameMode.RaymanPS1:
                    return new PS1_R1_Manager();

                case GameMode.RaymanPC:
                    return new PC_R1_Manager();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}