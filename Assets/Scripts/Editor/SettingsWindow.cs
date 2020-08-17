using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R1Engine;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SettingsWindow : UnityWindow
{
    [MenuItem("Ray1Map/Settings")]
    public static void ShowWindow()
	{
		GetWindow<SettingsWindow>(false, "Settings", true);
	}

	private void OnEnable()
	{
		titleContent = EditorGUIUtility.IconContent("Settings");
		titleContent.text = "Settings";
	}

    public string[] DefaultMemoryOptionNames { get; } = new string[]
    {
        "Custom",
        "DOSBox 0.74"
    };

    public string[] DefaultMemoryOptionProcessNames { get; } = new string[]
    {
        null,
        "DOSBox.exe"
    };

    public int[] DefaultMemoryOptionPointers { get; } = new int[]
    {
        -1,
        0x01D3A1A0
    };

    public int DefaultMemoryOptionsIndex { get; set; }
    public int PreviousDefaultMemoryOptionsIndex { get; set; } = -1;

    bool isFirstRun = true;

    protected override async UniTask UpdateEditorFields()
	{
        FileSystem.Mode fileMode = FileSystem.Mode.Normal;
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL) {
            fileMode = FileSystem.Mode.Web;
        }

        // Increase label width due to it being cut off otherwise
        EditorGUIUtility.labelWidth = 192;

		if (TotalyPos == 0f)
            TotalyPos = position.height;

		scrollbarShown = TotalyPos > position.height;
		ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

		EditorGUI.BeginChangeCheck();

        if (fileMode == FileSystem.Mode.Web) {
            EditorGUI.HelpBox(GetNextRect(ref YPos, height: 40f), "Your build target is configured as WebGL. Ray1Map will attempt to load from the server.", MessageType.Warning);
        }

        // Mode

        DrawHeader("Mode");

        Settings.SelectedGameMode = EditorField("Game", Settings.SelectedGameMode, getEnumOptions: () => EnumHelpers.GetValues<GameModeSelection>().Select(x => x.GetAttribute<GameModeAttribute>().DisplayName).ToArray());
        
        Settings.LoadFromMemory = EditorField("Load from memory", Settings.LoadFromMemory);
        
        // Memory
        
        if (Settings.LoadFromMemory)
        {
            DrawHeader("Memory");

            DefaultMemoryOptionsIndex = EditorField("Default memory options", DefaultMemoryOptionsIndex, DefaultMemoryOptionNames);

            if (DefaultMemoryOptionsIndex != PreviousDefaultMemoryOptionsIndex)
            {
                if (PreviousDefaultMemoryOptionsIndex == -1)
                {
                    var match = Enumerable.Range(0, DefaultMemoryOptionNames.Length).FirstOrDefault(x => DefaultMemoryOptionProcessNames[x] == Settings.ProcessName && DefaultMemoryOptionPointers[x] == Settings.GameBasePointer);

                    if (match > 0)
                        DefaultMemoryOptionsIndex = match;
                }

                PreviousDefaultMemoryOptionsIndex = DefaultMemoryOptionsIndex;

                if (DefaultMemoryOptionsIndex != 0)
                {
                    Settings.ProcessName = DefaultMemoryOptionProcessNames[DefaultMemoryOptionsIndex];
                    Settings.GameBasePointer = DefaultMemoryOptionPointers[DefaultMemoryOptionsIndex];
                }
            }

            EditorGUI.BeginDisabledGroup(DefaultMemoryOptionsIndex != 0);

            Settings.ProcessName = EditorField("Process name", Settings.ProcessName);
            Settings.GameBasePointer = EditorField("Game memory pointer", Settings.GameBasePointer);
         
            EditorGUI.EndDisabledGroup();

            Settings.FindPointerAutomatically = EditorField("Find pointer automatically", Settings.FindPointerAutomatically);
        }

        // Map

        DrawHeader("Map");

        if (!isFirstRun)
            Settings.World = AvailableWorlds.ElementAtOrDefault(EditorField("World", AvailableWorlds.FindItemIndex(x => x == Settings.World), WorldOptions));

        try
        {
			// Only update if previous values don't match
			if (!PrevLvlValues.ComparePreviousValues())
            {
                //Debug.Log($"Updated levels for world {Settings.World}");

                var manager = Settings.GetGameManager;
                var settings = Settings.GetGameSettings;

                var mapNames = MapNames.GetMapNames(settings.Game);
                var worldNames = MapNames.GetWorldNames(settings.Game);

                CurrentLevels = manager.GetLevels(settings)
                    .Select(x => new KeyValuePair<int, KeyValuePair<int, string>[]>(x.Key, x.Value.OrderBy(i => i)
                        .Select(i => new KeyValuePair<int, string>(i, mapNames?.TryGetItem(Settings.World)?.TryGetItem(i)))
                        .ToArray()))
                    .ToArray();
                AvailableWorlds = CurrentLevels.Where(x => x.Value.Any()).Select(x => x.Key).ToArray();

                // Helper method for getting the world name
                string GetWorldName(int worldNum, string worldName) => worldName != null ? $"{worldNum:00} - {worldName}" : $"{worldNum}";

                WorldOptions = AvailableWorlds.Select(x => worldNames?.TryGetItem(x)).Select((x, i) => GetWorldName(AvailableWorlds[i], x)).ToArray();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }

        if (fileMode == FileSystem.Mode.Web) {
            var lvlIndex = EditorField("Map", Settings.Level);

            if (lvlIndex >= 0)
                Settings.Level = lvlIndex;
        } else {
            var levels = Directory.Exists(Settings.CurrentDirectory) ? CurrentLevels : new KeyValuePair<int, KeyValuePair<int, string>[]>[0];

            var currentLevels = levels.FindItem(x => x.Key == Settings.World).Value ?? new KeyValuePair<int, string>[0];

            if (currentLevels.All(x => x.Key != Settings.Level))
                Settings.Level = currentLevels.FirstOrDefault().Key;

            // Helper method for getting the level name
            string GetLvlName(int lvlNum, string lvlName) => lvlName != null ? $"{lvlNum:00} - {lvlName}" : $"{lvlNum}";

            var lvlIndex = EditorField("Map", currentLevels.FindItemIndex(x => x.Key == Settings.Level), currentLevels.Select(x => GetLvlName(x.Key, x.Value)).ToArray());

            if (currentLevels.Length > lvlIndex && lvlIndex != -1)
                Settings.Level = currentLevels[lvlIndex].Key;
        }

        EditorGUI.BeginDisabledGroup(Settings.SelectedGameMode != GameModeSelection.RaymanEducationalPC && Settings.SelectedGameMode != GameModeSelection.RaymanQuizPC && Settings.SelectedGameMode != GameModeSelection.RaymanEducationalPS1);

        try
		{
            // Only update if previous values don't match
            if (!PrevLvlValues.ComparePreviousValues())
            {
				//Debug.Log("Updated EDU volumes");
                CurrentEduVolumes = Settings.GetGameManager.GetEduVolumes(Settings.GetGameSettings);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }

		var eduIndex = EditorField("Volume", CurrentEduVolumes.FindItemIndex(x => x == Settings.EduVolume), CurrentEduVolumes);

		if (CurrentEduVolumes.Length > eduIndex && eduIndex != -1)
			Settings.EduVolume = CurrentEduVolumes[eduIndex];

        EditorGUI.EndDisabledGroup();

        // Directories
        DrawHeader("Directories" + (fileMode == FileSystem.Mode.Web ? " (Web)" : ""));

        Settings.HideDirSettings = EditorField("Hide directory fields", Settings.HideDirSettings);

        if (!Settings.HideDirSettings)
        {
            var modes = EnumHelpers.GetValues<GameModeSelection>();
            if (fileMode == FileSystem.Mode.Web)
            {
                foreach (var mode in modes)
                {
                    Settings.GameDirectoriesWeb[mode] = EditorField(mode.GetAttribute<GameModeAttribute>()?.DisplayName ?? "N/A", Settings.GameDirectoriesWeb.TryGetItem(mode, String.Empty));
                }
            }
            else
            {
                foreach (var mode in modes)
                {
                    Settings.GameDirectories[mode] = DirectoryField(GetNextRect(ref YPos), mode.GetAttribute<GameModeAttribute>()?.DisplayName ?? "N/A", Settings.GameDirectories.TryGetItem(mode, String.Empty));
                }
            }
        }

        // Miscellaneous

        DrawHeader("Miscellaneous");

        Settings.StateSwitchingMode = EditorField("State switching", Settings.StateSwitchingMode);

        Settings.UseHDCollisionSheet = EditorField("Use HD collision sheet", Settings.UseHDCollisionSheet);

        Settings.AnimateSprites = EditorField("Animate sprites", Settings.AnimateSprites);

        Settings.ShowAlwaysEvents = EditorField("Show always events", Settings.ShowAlwaysEvents);

        Settings.ShowEditorEvents = EditorField("Show editor events", Settings.ShowEditorEvents);

        Settings.ShowDebugInfo = EditorField("Show debug info", Settings.ShowDebugInfo);

        Settings.BackupFiles = EditorField("Create .BAK backup files", Settings.BackupFiles);

        Settings.ScreenshotEnumeration = EditorField("Screenshot enumeration", Settings.ScreenshotEnumeration);

        Settings.FollowRaymanInMemoryMode = EditorField("Follow Rayman in memory mode", Settings.FollowRaymanInMemoryMode);

        Rect rect = GetNextRect(ref YPos);
        rect = EditorGUI.PrefixLabel(rect, new GUIContent("Serialization log"));
        bool log = Settings.Log;
        rect = PrefixToggle(rect, ref log);
        Settings.Log = log;

        if (Settings.Log)
            Settings.LogFile = FileField(rect, "Serialization log File", Settings.LogFile, true, "txt", includeLabel: false);

        // Editor Tools

        var em = LevelEditorData.EditorManager;

        if (em != null)
        {
            DrawHeader("Editor Tools");

            if (EditorButton("Copy localization"))
            {
                if (em.Level.Localization != null)
                {
                    TextEditor te = new TextEditor
                    {
                        text = JsonConvert.SerializeObject(em.Level.Localization, Formatting.Indented)
                    };
                    te.SelectAll();
                    te.Copy();
                }
            }
        }

        // Game Tools

        DrawHeader("Game Tools");

        // Only update if previous values don't match
        if (!PrevLvlValues.ComparePreviousValues())
        {
            //Debug.Log("Updated game actions");
            CurrentGameActions = Settings.GetGameManager.GetGameActions(Settings.GetGameSettings);
        }

        // Add every game action
        foreach (GameAction action in CurrentGameActions)
        {
            if (EditorButton(action.DisplayName))
            {
                // Get the directories
                string inputDir = action.RequiresInputDir ? EditorUtility.OpenFolderPanel("Select input directory", null, "") : null;

                if (string.IsNullOrEmpty(inputDir) && action.RequiresInputDir)
                    return;

                string outputDir = action.RequiresOutputDir ? EditorUtility.OpenFolderPanel("Select output directory", null, "") : null;

                if (string.IsNullOrEmpty(outputDir) && action.RequiresOutputDir)
                    return;

                // Run the action
                await action.GameActionFunc(inputDir, outputDir);
            }
        }

        // Global Tools

        DrawHeader("Global Tools");

        async UniTask AddGlobalActionAsync(string actionName)
        {
            if (EditorButton(actionName))
            {
                // Get the output directory
                string outputDir = EditorUtility.OpenFolderPanel("Select output directory", null, "");

                if (string.IsNullOrEmpty(outputDir))
                    return;

                //const GameModeSelection start = GameModeSelection.RaymanJaguar;
                //var reachedStart = false;

                // Run each action
                foreach (var mode in EnumHelpers.GetValues<GameModeSelection>())
                {
                    //if (!reachedStart)
                    //{
                    //    if (mode == start)
                    //        reachedStart = true;
                    //    else
                    //        continue;
                    //}

                    // Make sure the mode is valid
                    if (!Settings.GameDirectories.ContainsKey(mode))
                    {
                        Debug.LogWarning($"Mode {mode} was skipped due to not having a valid directory");
                        continue;
                    }

                    // Create settings
                    var settings = new GameSettings(mode, Settings.GameDirectories[mode], Settings.World, Settings.Level);

                    // Get manager
                    var manager = settings.GetGameManager;

                    // Set to default EDU volume
                    settings.EduVolume = manager.GetEduVolumes(settings).FirstOrDefault();

                    // Get action
                    var action = manager.GetGameActions(settings).FirstOrDefault(x => x.DisplayName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase));

                    // Make sure an action was found
                    if (action == null)
                    {
                        Debug.LogWarning($"Mode {mode} was skipped due to not having a matching action");
                        continue;
                    }

                    try
                    {
                        // Run the action
                        await action.GameActionFunc(null, Path.Combine(outputDir, mode.ToString()));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Mode {mode} failed with exception: {ex.Message}");
                    }
                    finally
                    {
                        // Unload textures
                        await Resources.UnloadUnusedAssets();
                    }
                }
            }
        }

        // Add special game actions
        await AddGlobalActionAsync("Export Sprites");
        await AddGlobalActionAsync("Export Animation Frames");
        await AddGlobalActionAsync("Export Vignette");

        // Update previous values
        PrevLvlValues.UpdatePreviousValues();

        // Randomizer

        DrawHeader("Randomizer");

        if (EditorButton("Run Batch Randomizer"))
            await BatchRandomizeAsync();

        RandomizerSeed = EditorField("Seed", RandomizerSeed);

        RandomizerFlags = (RandomizerFlags)EditorGUI.EnumFlagsField(GetNextRect(ref YPos), "Flags", RandomizerFlags);

        TotalyPos = YPos;
		GUI.EndScrollView();

		if (EditorGUI.EndChangeCheck() || Dirty)
		{
			Settings.Save();
			Dirty = false;
		}

        isFirstRun = false;
    }

    #region Randomizer

    private async UniTask BatchRandomizeAsync()
    {
        // Get the settings
        var settings = Settings.GetGameSettings;

        // Get the manager
        var manager = Settings.GetGameManager;

        // Get the flags
        var flag = RandomizerFlags;

        // Enumerate every world
        foreach (var world in manager.GetLevels(settings))
        {
            // Set the world
            settings.World = world.Key;

            // Enumerate every level
            foreach (var lvl in world.Value)
            {
                // Set the level
                settings.Level = lvl;

                // Create the context
                var context = new Context(settings);

                // Load the files
                await manager.LoadFilesAsync(context);

                // Load the level
                var editorManager = await manager.LoadAsync(context, false);

                // Randomize (only first map for now)
                Randomizer.Randomize(editorManager, flag, (int)world.Key + lvl + RandomizerSeed, 0);

                // Save the level
                manager.SaveLevel(context, editorManager);
            }
        }
    }

    private int RandomizerSeed { get; set; }

    private RandomizerFlags RandomizerFlags { get; set; }

    #endregion

    #region Available Options

    public string[] WorldOptions { get; set; } = new string[0];

    #endregion

    private PreviousValues PrevLvlValues { get; } = new PreviousValues();

    private int[] AvailableWorlds { get; set; } = new int[0];

    private KeyValuePair<int, KeyValuePair<int, string>[]>[] CurrentLevels { get; set; } = new KeyValuePair<int, KeyValuePair<int, string>[]>[0];

	private string[] CurrentEduVolumes { get; set; } = new string[0];

    private GameAction[] CurrentGameActions { get; set; } = new GameAction[0];

    private float TotalyPos { get; set; }

	private Vector2 ScrollPosition { get; set; } = Vector2.zero;

	public class PreviousValues
    {
        /// <summary>
        /// Updates saved previous values
        /// </summary>
        public void UpdatePreviousValues()
        {
            PrevDir = Settings.CurrentDirectory;
            PrevWorld = Settings.World;
            PrevEduVolume = Settings.EduVolume;
        }

        /// <summary>
        /// Compares previous values and returns true if they're the same
        /// </summary>
        /// <returns>True if they're the same, otherwise false</returns>
        public bool ComparePreviousValues()
        {
            return PrevDir == Settings.CurrentDirectory && PrevWorld == Settings.World && PrevEduVolume == Settings.EduVolume;
        }

        /// <summary>
        /// The previously saved current directory
        /// </summary>
        private string PrevDir { get; set; } = String.Empty;

        /// <summary>
        /// The previously saved world
        /// </summary>
        private int PrevWorld { get; set; } = 1;

        private string PrevEduVolume { get; set; } = String.Empty;
    }
}