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
            GameDirectories = new Dictionary<GameModeSelection, string>();

            Load();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The selected game mode
        /// </summary>
        public static GameModeSelection SelectedGameMode { get; set; }

        /// <summary>
        /// The selected game world
        /// </summary>
        public static World World { get; set; }

        /// <summary>
        /// The selected level index, starting from 1
        /// </summary>
        public static int Level { get; set; }

        /// <summary>
        /// The selected educational game volume
        /// </summary>
        public static string EduVolume { get; set; }

        /// <summary>
        /// True for the HD collision sheet to be used, false for the original Rayman Designer one to be used
        /// </summary>
        public static bool UseHDCollisionSheet { get; set; }

        /// <summary>
        /// Indicates if sprites should be animated in the editor
        /// </summary>
        public static bool AnimateSprites { get; set; }

        /// <summary>
        /// Indicates if always events should be shown
        /// </summary>
        public static bool ShowAlwaysEvents { get; set; }

        /// <summary>
        /// Indicates if editor events should be shown
        /// </summary>
        public static bool ShowEditorEvents { get; set; }

        /// <summary>
        /// The specified game directories for each mode
        /// </summary>
        public static Dictionary<GameModeSelection, string> GameDirectories { get; set; }

        /// <summary>
        /// Gets the current directory based on the selected mode
        /// </summary>
        public static string CurrentDirectory => GameDirectories.TryGetValue(SelectedGameMode, out var dir) ? dir : String.Empty;

        /// <summary>
        /// The string encoding to use for the game files
        /// </summary>
        public static Encoding StringEncoding { get; } = Encoding.GetEncoding(437);

        /// <summary>
        /// Gets the current game settings
        /// </summary>
        public static GameSettings GetGameSettings => new GameSettings(SelectedGameMode, CurrentDirectory, World, Level)
        {
            EduVolume = EduVolume
        };

        /// <summary>
        /// Gets a new manager instance for the specified mode
        /// </summary>
        /// <returns></returns>
        public static IGameManager GetGameManager => (IGameManager)Activator.CreateInstance(SelectedGameMode.GetAttribute<GameModeAttribute>().ManagerType);

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the settings
        /// </summary>
        public static void Save()
        {
            foreach (var mode in EnumHelpers.GetValues<GameModeSelection>())
            {
                string dir = GameDirectories.ContainsKey(mode) ? GameDirectories[mode] : "";
                EditorPrefs.SetString("Directory" + mode, dir);
            }

            EditorPrefs.SetString("GameMode", SelectedGameMode.ToString());
            EditorPrefs.SetString("SelectedWorld", World.ToString());
            EditorPrefs.SetString("EduVolume", EduVolume);
            EditorPrefs.SetInt("SelectedLevelFile", Level);
            EditorPrefs.SetBool("UseHDCollisionSheet", UseHDCollisionSheet);
            EditorPrefs.SetBool("AnimateSprites", AnimateSprites);
            EditorPrefs.SetBool("ShowAlwaysEvents", ShowAlwaysEvents);
            EditorPrefs.SetBool("ShowEditorEvents", ShowEditorEvents);
        }

        /// <summary>
        /// Loads the settings
        /// </summary>
        public static void Load()
        {
            foreach (var mode in EnumHelpers.GetValues<GameModeSelection>())
            {
                string dir = GameDirectories.ContainsKey(mode) ? GameDirectories[mode] : "";
                GameDirectories[mode] = EditorPrefs.GetString("Directory" + mode, dir);
            }

            SelectedGameMode = Enum.TryParse(EditorPrefs.GetString("GameMode", SelectedGameMode.ToString()), out GameModeSelection gameMode) ? gameMode : SelectedGameMode;
            World = Enum.TryParse(EditorPrefs.GetString("SelectedWorld", World.ToString()), out World world) ? world : World;
            EduVolume = EditorPrefs.GetString("EduVolume", EduVolume);
            Level = EditorPrefs.GetInt("SelectedLevelFile", Level);
            UseHDCollisionSheet = EditorPrefs.GetBool("UseHDCollisionSheet", UseHDCollisionSheet);
            AnimateSprites = EditorPrefs.GetBool("AnimateSprites", AnimateSprites);
            ShowAlwaysEvents = EditorPrefs.GetBool("ShowAlwaysEvents", ShowAlwaysEvents);
            ShowEditorEvents = EditorPrefs.GetBool("ShowEditorEvents", ShowEditorEvents);
        }

        #endregion
    }
}