using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R1Engine {
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class Settings
    {
        #region Static Constructor

        static Settings()
        {
            GameDirectories = new Dictionary<GameModeSelection, string>();
            GameDirectoriesWeb = new Dictionary<GameModeSelection, string>();

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
        /// Indicates if .BAK backup files should be created before writing
        /// </summary>
        public static bool BackupFiles { get; set; }


        private const string editorPrefsPrefix = "Ray1Map.";
        private const string settingsFile = "Settings.txt";

        /// <summary>
        /// The specified game directories for each mode
        /// </summary>
        public static Dictionary<GameModeSelection, string> GameDirectories { get; set; }
        public static Dictionary<GameModeSelection, string> GameDirectoriesWeb { get; set; }

        /// <summary>
        /// Serialization log file
        /// </summary>
        public static string LogFile { get; set; }
        /// <summary>
        
        /// Whether to log to the serialization log file
        /// </summary>
        public static bool Log { get; set; }

        /// <summary>
        /// Gets the current directory based on the selected mode
        /// </summary>
        public static string CurrentDirectory {
            get {
                if (FileSystem.mode == FileSystem.Mode.Web) {
                    return GameDirectoriesWeb.TryGetValue(SelectedGameMode, out var dir) ? dir : String.Empty;
                } else {
                    return GameDirectories.TryGetValue(SelectedGameMode, out var dir) ? dir : String.Empty;
                }
            }
        }


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
        private static void SerializeSettings(ISerializer s) {
            GameModeSelection[] modes = EnumHelpers.GetValues<GameModeSelection>();
            foreach (GameModeSelection mode in modes) {
                string dir = GameDirectories.ContainsKey(mode) ? GameDirectories[mode] : "";
                GameDirectories[mode] = s.SerializeString("Directory" + mode.ToString(), dir);
            }
            if (UnityEngine.Application.isEditor) {
                foreach (GameModeSelection mode in modes) {
                    string dir = GameDirectoriesWeb.ContainsKey(mode) ? GameDirectoriesWeb[mode] : "";
                    GameDirectoriesWeb[mode] = s.SerializeString("WebDirectory" + mode.ToString(), dir);
                }
            }
            string modeString = s.SerializeString("GameMode", SelectedGameMode.ToString());
            SelectedGameMode = Enum.TryParse(modeString, out GameModeSelection gameMode) ? gameMode : SelectedGameMode;
            string worldString = s.SerializeString("World", World.ToString());
            World = Enum.TryParse(worldString, out World world) ? world : World;

            EduVolume = s.SerializeString("EduVolume", EduVolume);
            Level = s.SerializeInt("SelectedLevelFile", Level);
            UseHDCollisionSheet = s.SerializeBool("UseHDCollisionSheet", UseHDCollisionSheet);
            AnimateSprites = s.SerializeBool("AnimateSprites", AnimateSprites);
            ShowAlwaysEvents = s.SerializeBool("ShowAlwaysEvents", ShowAlwaysEvents);
            ShowEditorEvents = s.SerializeBool("ShowEditorEvents", ShowEditorEvents);
            BackupFiles = s.SerializeBool("BackupFiles", BackupFiles);

            Log = s.SerializeBool("Log", Log);
            LogFile = s.SerializeString("LogFile", LogFile);
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        public static void Save() {
            if (UnityEngine.Application.isEditor) {
#if UNITY_EDITOR
                ISerializer s = new EditorWriteSerializer();
                SerializeSettings(s);
#endif
            } else if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.WebGLPlayer) {
                using (SettingsFileWriteSerializer s = new SettingsFileWriteSerializer(settingsFile)) {
                    SerializeSettings(s);
                }
            }
        }

        /// <summary>
        /// Loads the settings
        /// </summary>
        public static void Load() {
            if (UnityEngine.Application.isEditor) {
#if UNITY_EDITOR
                ISerializer s = new EditorReadSerializer();
                SerializeSettings(s);
#endif
            } else if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.WebGLPlayer) {
                if (!File.Exists(settingsFile)) {
                    Save();
                }
                ISerializer s = new SettingsFileReadSerializer(settingsFile);
                SerializeSettings(s);
            }
        }
        #endregion

        #region Subclasses (Settings serialization)
        private interface ISerializer {
            string SerializeString(string key, string value);
            bool SerializeBool(string key, bool value);
            int SerializeInt(string key, int value);
        }

#if UNITY_EDITOR
        private class EditorReadSerializer : ISerializer {
            public bool SerializeBool(string key, bool value) {
                return UnityEditor.EditorPrefs.GetBool(editorPrefsPrefix + key, value);
            }

            public string SerializeString(string key, string value) {
                return UnityEditor.EditorPrefs.GetString(editorPrefsPrefix + key, value);
            }

            public int SerializeInt(string key, int value) {
                return UnityEditor.EditorPrefs.GetInt(editorPrefsPrefix + key, value);
            }
        }

        private class EditorWriteSerializer : ISerializer {
            public bool SerializeBool(string key, bool value) {
                UnityEditor.EditorPrefs.SetBool(editorPrefsPrefix + key, value);
                return value;
            }

            public string SerializeString(string key, string value) {
                UnityEditor.EditorPrefs.SetString(editorPrefsPrefix + key, value);
                return value;
            }

            public int SerializeInt(string key, int value) {
                UnityEditor.EditorPrefs.SetInt(editorPrefsPrefix + key, value);
                return value;
            }
        }
#endif

        private class SettingsFileWriteSerializer : ISerializer, IDisposable {
            StreamWriter writer;
            public SettingsFileWriteSerializer(string path) {
                writer = new StreamWriter(path);
            }

            public void Dispose() {
                ((IDisposable)writer).Dispose();
            }

            public bool SerializeBool(string key, bool value) {
                writer.WriteLine(key + "=" + value.ToString());
                return value;
            }

            public string SerializeString(string key, string value) {
                writer.WriteLine(key + "=" + value.ToString());
                return value;
            }

            public int SerializeInt(string key, int value) {
                writer.WriteLine(key + "=" + value.ToString());
                return value;
            }
        }

        private class SettingsFileReadSerializer : ISerializer {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            public SettingsFileReadSerializer(string path) {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++) {
                    // Not using split, just in case any of the values contain a =
                    int index = lines[i].IndexOf('=');
                    if (index >= 0 && index < lines[i].Length) {
                        settings.Add(lines[i].Substring(0, index), lines[i].Substring(index + 1));
                    }
                }
            }

            public bool SerializeBool(string key, bool value) {
                if (settings.ContainsKey(key)) {
                    if (bool.TryParse(settings[key], out bool b)) {
                        return b;
                    }
                }
                return value;
            }

            public string SerializeString(string key, string value) {
                if (settings.ContainsKey(key)) {
                    return settings[key];
                }
                return value;
            }

            public int SerializeInt(string key, int value) {
                if (settings.ContainsKey(key)) {
                    if (int.TryParse(settings[key], out int i)) {
                        return i;
                    }
                }
                return value;
            }
        }
        #endregion
    }
}