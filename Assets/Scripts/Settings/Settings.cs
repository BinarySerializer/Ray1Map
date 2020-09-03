using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

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

        #region Events

        /// <summary>
        /// Occurs when a <see cref="SelectedGameMode"/> changes
        /// </summary>
        public static event EventHandler OnSelectedGameModeChanged;

        /// <summary>
        /// Occurs when a <see cref="World"/> changes
        /// </summary>
        public static event EventHandler OnWorldChanged;

        /// <summary>
        /// Occurs when a <see cref="Level"/> changes
        /// </summary>
        public static event EventHandler OnLevelChanged;

        /// <summary>
        /// Occurs when a <see cref="EduVolume"/> changes
        /// </summary>
        public static event EventHandler OnEduVolumeChanged;

        /// <summary>
        /// Occurs when a <see cref="UseHDCollisionSheet"/> changes
        /// </summary>
        public static event EventHandler OnUseHDCollisionSheetChanged;

        /// <summary>
        /// Occurs when a <see cref="AnimateSprites"/> changes
        /// </summary>
        public static event EventHandler OnAnimateSpritesChanged;

        /// <summary>
        /// Occurs when a <see cref="ShowAlwaysEvents"/> changes
        /// </summary>
        public static event EventHandler OnShowAlwaysEventsChanged;

        /// <summary>
        /// Occurs when a <see cref="ShowEditorEvents"/> changes
        /// </summary>
        public static event EventHandler OnShowEditorEventsChanged;

        #endregion

        #region Private Fields

        private static GameModeSelection _selectedGameMode;
        private static int _world = 1;
        private static int _level = 1;
        private static string _eduVolume;
        private static bool _useHdCollisionSheet;
        private static bool _animateSprites = true;
        private static bool _showAlwaysEvents = true;
        private static bool _showEditorEvents = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// The selected game mode
        /// </summary>
        public static GameModeSelection SelectedGameMode
        {
            get => _selectedGameMode;
            set
            {
                if (_selectedGameMode == value)
                    return;

                _selectedGameMode = value;
                OnSelectedGameModeChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The selected game world
        /// </summary>
        public static int World
        {
            get => _world;
            set
            {
                if (_world == value)
                    return;

                _world = value;
                OnWorldChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The selected level index, starting from 1
        /// </summary>
        public static int Level
        {
            get => _level;
            set 
            {
                if (_level == value)
                    return;

                _level = value;
                OnLevelChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The selected educational game volume
        /// </summary>
        public static string EduVolume
        {
            get => _eduVolume;
            set
            {
                if (_eduVolume == value)
                    return;

                _eduVolume = value;
                OnEduVolumeChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static bool LoadFromMemory { get; set; }
        
        public static string ProcessName { get; set; }

        public static int GameBasePointer { get; set; }
        public static bool FindPointerAutomatically { get; set; }

        /// <summary>
        /// True for the HD collision sheet to be used, false for the original Rayman Designer one to be used
        /// </summary>
        public static bool UseHDCollisionSheet
        {
            get => _useHdCollisionSheet;
            set
            {
                if (_useHdCollisionSheet == value)
                    return;

                _useHdCollisionSheet = value;
                OnUseHDCollisionSheetChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Indicates if sprites should be animated in the editor
        /// </summary>
        public static bool AnimateSprites
        {
            get => _animateSprites;
            set
            {
                if (_animateSprites == value)
                    return;

                _animateSprites = value;
                OnAnimateSpritesChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static bool AnimateTiles { get; set; } = true;

        /// <summary>
        /// Indicates if always events should be shown
        /// </summary>
        public static bool ShowAlwaysEvents
        {
            get => _showAlwaysEvents;
            set
            {
                if (_showAlwaysEvents == value)
                    return;

                _showAlwaysEvents = value;
                OnShowAlwaysEventsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Indicates if editor events should be shown
        /// </summary>
        public static bool ShowEditorEvents
        {
            get => _showEditorEvents;
            set
            {
                if (_showEditorEvents == value)
                    return;

                _showEditorEvents = value;
                OnShowEditorEventsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static bool ScreenshotEnumeration { get; set; }

        public static bool ShowRayman { get; set; } = true;
        public static bool ShowDefaultObjIcons { get; set; } = true;
        public static bool ShowObjOffsets { get; set; }
        public static bool FollowRaymanInMemoryMode { get; set; } = true;

        public static bool HideDirSettings { get; set; }

        /// <summary>
        /// Indicates if .BAK backup files should be created before writing
        /// </summary>
        public static bool BackupFiles { get; set; }

        public static StateSwitchingMode StateSwitchingMode { get; set; } = StateSwitchingMode.None;

        /// <summary>
        /// Indicates if debug info should be shown on event mouse hovering
        /// </summary>
        public static bool ShowDebugInfo { get; set; }


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
                    return GameDirectoriesWeb.TryGetItem(SelectedGameMode, String.Empty);
                } else {
                    return GameDirectories.TryGetItem(SelectedGameMode, String.Empty);
                }
            }
        }


        /// <summary>
        /// The string encoding to use for the game files
        /// </summary>
        public static Encoding StringEncoding { get; } = Encoding.GetEncoding(437);

        /// <summary>
        /// The size of a map cell in pixels
        /// </summary>
        public static int CellSize = 16;

        public static int PixelsPerUnit = 16;

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

        private static void SerializeSettings(ISerializer s, bool cmdLine = false) {
            if (!cmdLine) {
                GameModeSelection[] modes = EnumHelpers.GetValues<GameModeSelection>();
                foreach (GameModeSelection mode in modes) {
                    string dir = GameDirectories.ContainsKey(mode) ? GameDirectories[mode] : "";
                    GameDirectories[mode] = s.SerializeString("Directory" + mode.ToString(), dir);
                }
                if (Application.isEditor) {
                    foreach (GameModeSelection mode in modes) {
                        string dir = GameDirectoriesWeb.ContainsKey(mode) ? GameDirectoriesWeb[mode] : "";
                        GameDirectoriesWeb[mode] = s.SerializeString("WebDirectory" + mode.ToString(), dir);
                    }
                }
            }
            string modeString = s.SerializeString("GameMode", SelectedGameMode.ToString(), "mode", "m");
            SelectedGameMode = Enum.TryParse<GameModeSelection>(modeString, out GameModeSelection gameMode) ? gameMode : SelectedGameMode;
            if (cmdLine) {
                if (FileSystem.mode == FileSystem.Mode.Web) {
                    string dir = GameDirectoriesWeb.ContainsKey(SelectedGameMode) ? GameDirectoriesWeb[SelectedGameMode] : "";
                    GameDirectoriesWeb[SelectedGameMode] = s.SerializeString("Directory", dir, "dir", "directory", "folder", "f", "d");
                } else {
                    string dir = GameDirectories.ContainsKey(SelectedGameMode) ? GameDirectories[SelectedGameMode] : "";
                    GameDirectories[SelectedGameMode] = s.SerializeString("Directory", dir, "dir", "directory", "folder", "f", "d");
                }
            }

            World = s.SerializeInt("WorldIndex", World, "wld", "w");
            EduVolume = s.SerializeString("EduVolume", EduVolume, "volume", "vol");
            Level = s.SerializeInt("SelectedLevelFile", Level, "level", "lvl", "map");
            LoadFromMemory = s.SerializeBool("LoadFromMemory", LoadFromMemory);
            ProcessName = s.SerializeString("ProcessName", ProcessName);
            GameBasePointer = s.SerializeInt("GameBasePointer", GameBasePointer);
            FindPointerAutomatically = s.SerializeBool("FindPointerAutomatically", FindPointerAutomatically);
            UseHDCollisionSheet = s.SerializeBool("UseHDCollisionSheet", UseHDCollisionSheet);
            AnimateSprites = s.SerializeBool("AnimateSprites", AnimateSprites);
            AnimateTiles = s.SerializeBool("AnimateTiles", AnimateTiles);
            ShowAlwaysEvents = s.SerializeBool("ShowAlwaysEvents", ShowAlwaysEvents);

            string stateModeString = s.SerializeString("StateSwitchingMode", StateSwitchingMode.ToString());
            StateSwitchingMode = Enum.TryParse(stateModeString, out StateSwitchingMode stateMode) ? stateMode : StateSwitchingMode;
            
            ShowEditorEvents = s.SerializeBool("ShowEditorEvents", ShowEditorEvents);
            ScreenshotEnumeration = s.SerializeBool("ScreenshotEnumeration", ScreenshotEnumeration);
            BackupFiles = s.SerializeBool("BackupFiles", BackupFiles);
            ShowDebugInfo = s.SerializeBool("ShowDebugInfo", ShowDebugInfo);
            ShowDefaultObjIcons = s.SerializeBool("ShowDefaultObjIcons", ShowDefaultObjIcons);
            ShowObjOffsets = s.SerializeBool("ShowObjOffsets", ShowObjOffsets);
            ShowRayman = s.SerializeBool("ShowRayman", ShowRayman);
            FollowRaymanInMemoryMode = s.SerializeBool("FollowRaymanInMemoryMode", FollowRaymanInMemoryMode);
            HideDirSettings = s.SerializeBool("HideDirSettings", HideDirSettings);

            Log = s.SerializeBool("Log", Log);
            LogFile = s.SerializeString("LogFile", LogFile);
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        public static void Save() {
            if (Application.isEditor) {
#if UNITY_EDITOR
                ISerializer s = new EditorWriteSerializer();
                SerializeSettings(s);
#endif
            } else if (Application.platform != RuntimePlatform.WebGLPlayer) {
                using (SettingsFileWriteSerializer s = new SettingsFileWriteSerializer(settingsFile)) {
                    SerializeSettings(s);
                }
            }
        }

        /// <summary>
        /// Loads the settings
        /// </summary>
        public static void Load() {
            if (Application.isEditor) {
#if UNITY_EDITOR
                ISerializer s = new EditorReadSerializer();
                SerializeSettings(s);
#endif
            } else if (Application.platform != RuntimePlatform.WebGLPlayer) {
                if (!File.Exists(settingsFile)) {
                    Save();
                }
                ISerializer s = new SettingsFileReadSerializer(settingsFile);
                SerializeSettings(s);
            }
            ConfigureFileSystem();
            if (!Application.isEditor) {
                ParseCommandLineArguments();
            }
        }

        public static void ConfigureFileSystem() {

            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                FileSystem.mode = FileSystem.Mode.Web;
            }
#if UNITY_EDITOR
            if (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL) {
                FileSystem.mode = FileSystem.Mode.Web;
            }
#endif
        }

        static void ParseCommandLineArguments() {
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                // Read URL arguments
                ISerializer s = new WebArgumentsReadSerializer();
                SerializeSettings(s, cmdLine: true);
            } else {
                // Read command line arguments
                ISerializer s = new CmdLineReadSerializer();
                SerializeSettings(s, cmdLine: true);
            }
        }

        #endregion

        #region Subclasses (Settings serialization)

        private interface ISerializer {
            string SerializeString(string key, string value, params string[] cmdlineKeys);
            bool SerializeBool(string key, bool value, params string[] cmdlineKeys);
            int SerializeInt(string key, int value, params string[] cmdlineKeys);
        }

#if UNITY_EDITOR
        private class EditorReadSerializer : ISerializer {
            public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
                return UnityEditor.EditorPrefs.GetBool(editorPrefsPrefix + key, value);
            }

            public string SerializeString(string key, string value, params string[] cmdlineKeys) {
                return UnityEditor.EditorPrefs.GetString(editorPrefsPrefix + key, value);
            }

            public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
                return UnityEditor.EditorPrefs.GetInt(editorPrefsPrefix + key, value);
            }
        }

        private class EditorWriteSerializer : ISerializer {
            public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
                UnityEditor.EditorPrefs.SetBool(editorPrefsPrefix + key, value);
                return value;
            }

            public string SerializeString(string key, string value, params string[] cmdlineKeys) {
                UnityEditor.EditorPrefs.SetString(editorPrefsPrefix + key, value);
                return value;
            }

            public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
                UnityEditor.EditorPrefs.SetInt(editorPrefsPrefix + key, value);
                return value;
            }
        }
#endif

        private class CmdLineReadSerializer : ISerializer {
            string[] args;
            public CmdLineReadSerializer() {
                args = Environment.GetCommandLineArgs();
            }

            public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
                if (args == null || args.Length == 0 || cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
                for(int c = 0; c < cmdlineKeys.Length; c++) {
                    string cmdKey = cmdlineKeys[c];
                    if (cmdKey.Length == 1) {
                        cmdKey = "-" + cmdKey;
                    } else {
                        cmdKey = "--" + cmdKey;
                    }
                    int ind = Array.IndexOf(args, cmdKey);
                    if (ind > -1 && ind+1 < args.Length) {
                        if (bool.TryParse(args[ind+1], out bool b)) {
                            return b;
                        }
                    }
                }
                return value;
            }

            public string SerializeString(string key, string value, params string[] cmdlineKeys) {
                if (args == null || args.Length == 0 || cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
                for (int c = 0; c < cmdlineKeys.Length; c++) {
                    string cmdKey = cmdlineKeys[c];
                    if (cmdKey.Length == 1) {
                        cmdKey = "-" + cmdKey;
                    } else {
                        cmdKey = "--" + cmdKey;
                    }
                    int ind = Array.IndexOf(args, cmdKey);
                    if (ind > -1 && ind + 1 < args.Length) {
                        return args[ind + 1];
                    }
                }
                return value;
            }

            public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
                if (args == null || args.Length == 0 || cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
                for (int c = 0; c < cmdlineKeys.Length; c++) {
                    string cmdKey = cmdlineKeys[c];
                    if (cmdKey.Length == 1) {
                        cmdKey = "-" + cmdKey;
                    } else {
                        cmdKey = "--" + cmdKey;
                    }
                    int ind = Array.IndexOf(args, cmdKey);
                    if (ind > -1 && ind + 1 < args.Length) {
                        if (int.TryParse(args[ind + 1], out int val)) {
                            return val;
                        }
                    }
                }
                return value;
            }
        }

        private class WebArgumentsReadSerializer : ISerializer {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            public WebArgumentsReadSerializer() {
                string url = Application.absoluteURL;
                if (url.IndexOf('?') > 0) {
                    string urlArgsStr = url.Split('?')[1].Split('#')[0];
                    if (urlArgsStr.Length > 0) {
                        string[] urlArgs = urlArgsStr.Split('&');
                        foreach (string arg in urlArgs) {
                            string[] argKeyVal = arg.Split('=');
                            if (argKeyVal.Length > 1) {
                                settings.Add(argKeyVal[0], argKeyVal[1]);
                            }
                        }
                    }
                }
            }

            public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
                if (settings.ContainsKey(key)) {
                    if (bool.TryParse(settings[key], out bool b)) {
                        return b;
                    }
                }
                if (cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
                foreach (string cmdKey in cmdlineKeys) {
                    if (settings.ContainsKey(cmdKey)) {
                        if (bool.TryParse(settings[cmdKey], out bool b)) {
                            return b;
                        }
                    }
                }
                return value;
            }

            public string SerializeString(string key, string value, params string[] cmdlineKeys) {
                if (settings.ContainsKey(key)) {
                    return settings[key];
                }
                if (cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
                foreach (string cmdKey in cmdlineKeys) {
                    if (settings.ContainsKey(cmdKey)) {
                        return settings[cmdKey];
                    }
                }
                return value;
            }

            public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
                if (settings.ContainsKey(key)) {
                    if (int.TryParse(settings[key], out int i)) {
                        return i;
                    }
                }
                if (cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
                foreach (string cmdKey in cmdlineKeys) {
                    if (settings.ContainsKey(cmdKey)) {
                        if (int.TryParse(settings[cmdKey], out int i)) {
                            return i;
                        }
                    }
                }
                return value;
            }
        }

        private class SettingsFileWriteSerializer : ISerializer, IDisposable {
            StreamWriter writer;
            public SettingsFileWriteSerializer(string path) {
                writer = new StreamWriter(path);
            }

            public void Dispose() {
                writer?.Flush();
                ((IDisposable)writer)?.Dispose();
            }

            public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
                writer.WriteLine(key + "=" + value.ToString());
                return value;
            }

            public string SerializeString(string key, string value, params string[] cmdlineKeys) {
                writer.WriteLine(key + "=" + value.ToString());
                return value;
            }

            public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
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

            public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
                if (settings.ContainsKey(key)) {
                    if (bool.TryParse(settings[key], out bool b)) {
                        return b;
                    }
                }
                return value;
            }

            public string SerializeString(string key, string value, params string[] cmdlineKeys) {
                if (settings.ContainsKey(key)) {
                    return settings[key];
                }
                return value;
            }

            public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
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