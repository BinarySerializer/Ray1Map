using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R1Engine;
using R1Engine.Serialize;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
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

    protected override async UniTask UpdateEditorFields() {
        FileSystem.Mode fileMode = FileSystem.Mode.Normal;
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL) {
            fileMode = FileSystem.Mode.Web;
        }

        bool refreshGameActions = (GameModeDropdown?.HasChanged ?? false) || (MapSelectionDropdown?.HasChanged ?? false);

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

        if (GameModeDropdown == null)
            GameModeDropdown = new GameModeSelectionDropdown(new AdvancedDropdownState());
        var rectTemp = GetNextRect(ref YPos);
        var rbutton = EditorGUI.PrefixLabel(rectTemp, new GUIContent("Game"));
        rectTemp = new Rect(rbutton.x + rbutton.width - Mathf.Max(400f, rbutton.width), rbutton.y, Mathf.Max(400f, rbutton.width), rbutton.height);
        if (EditorGUI.DropdownButton(rbutton, new GUIContent(GameModeDropdown.SelectionName), FocusType.Passive))
            GameModeDropdown.Show(rectTemp);

        Settings.LoadFromMemory = EditorField("Load from memory", Settings.LoadFromMemory);

        // Memory

        if (Settings.LoadFromMemory) {
            DrawHeader("Memory");

            DefaultMemoryOptionsIndex = EditorField("Default memory options", DefaultMemoryOptionsIndex, DefaultMemoryOptionNames);

            if (DefaultMemoryOptionsIndex != PreviousDefaultMemoryOptionsIndex) {
                if (PreviousDefaultMemoryOptionsIndex == -1) {
                    var match = Enumerable.Range(0, DefaultMemoryOptionNames.Length).FirstOrDefault(x => DefaultMemoryOptionProcessNames[x] == Settings.ProcessName && DefaultMemoryOptionPointers[x] == Settings.GameBasePointer);

                    if (match > 0)
                        DefaultMemoryOptionsIndex = match;
                }

                PreviousDefaultMemoryOptionsIndex = DefaultMemoryOptionsIndex;

                if (DefaultMemoryOptionsIndex != 0) {
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

        if (fileMode == FileSystem.Mode.Normal) {
            rectTemp = GetNextRect(ref YPos);
            rbutton = EditorGUI.PrefixLabel(rectTemp, new GUIContent("Map"));
            rectTemp = new Rect(rbutton.x + rbutton.width - Mathf.Max(400f, rbutton.width), rbutton.y, Mathf.Max(400f, rbutton.width), rbutton.height);

            if (MapSelectionDropdown == null || GameModeDropdown.HasChanged) {
                GameInfo_Volume[] volumes;

                try {
                    var manager = Settings.GetGameManager;
                    var settings = Settings.GetGameSettings;

                    volumes = manager.GetLevels(settings);
                } catch (Exception ex) {
                    volumes = new GameInfo_Volume[0];
                    Debug.LogWarning(ex.Message);
                }

                MapSelectionDropdown = new MapSelectionDropdown(new AdvancedDropdownState(), volumes, Settings.SelectedGameMode.GetAttribute<GameModeAttribute>().Game);

                // Debug.Log($"Map selection updated with {volumes.Length} volumes");
            }

            if (EditorGUI.DropdownButton(rbutton, new GUIContent($"{(!string.IsNullOrEmpty(Settings.EduVolume) ? $"{Settings.EduVolume} - " : String.Empty)}{MapSelectionDropdown.GetLevelName(Settings.World, Settings.Level)}"), FocusType.Passive))
                MapSelectionDropdown.Show(rectTemp);
        } else if (fileMode == FileSystem.Mode.Web) {
            if (GameModeDropdown.HasChanged) {
                Settings.SelectedGameMode = GameModeDropdown.Selection;
                GameModeDropdown.HasChanged = false;
                Dirty = true;
            }
            Settings.World = EditorField("World", Settings.World);
            Settings.Level = EditorField("Level", Settings.Level);
            Settings.EduVolume = EditorField("Volume", Settings.EduVolume);
        }

        // Directories
        DrawHeader("Directories" + (fileMode == FileSystem.Mode.Web ? " (Web)" : ""));

        Settings.HideDirSettings = EditorField("Hide directory fields", Settings.HideDirSettings);

        if (!Settings.HideDirSettings) {
            var modes = EnumHelpers.GetValues<GameModeSelection>();
            if (fileMode == FileSystem.Mode.Web) {
                foreach (var mode in modes) {
                    Settings.GameDirectoriesWeb[mode] = EditorField(mode.GetAttribute<GameModeAttribute>()?.DisplayName ?? "N/A", Settings.GameDirectoriesWeb.TryGetItem(mode, String.Empty));
                }
            } else {
                foreach (var mode in modes) {
                    Settings.GameDirectories[mode] = DirectoryField(GetNextRect(ref YPos), mode.GetAttribute<GameModeAttribute>()?.DisplayName ?? "N/A", Settings.GameDirectories.TryGetItem(mode, String.Empty));
                }
            }
        }

        // Miscellaneous

        DrawHeader("Miscellaneous");

        Settings.StateSwitchingMode = EditorField("State switching", Settings.StateSwitchingMode);

        Settings.UseHDCollisionSheet = EditorField("Use HD collision sheet", Settings.UseHDCollisionSheet);

        Settings.AnimateSprites = EditorField("Animate sprites", Settings.AnimateSprites);

        Settings.AnimateTiles = EditorField("Animate tiles", Settings.AnimateTiles);

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
        if (Application.isPlaying && Controller.LoadState == Controller.State.Finished) {
            var lvl = LevelEditorData.Level;

            if (lvl != null) {
                DrawHeader("Editor Tools");

                if (EditorButton("Copy localization")) {
                    if (lvl.Localization != null) {
                        TextEditor te = new TextEditor {
                            text = JsonConvert.SerializeObject(lvl.Localization, Formatting.Indented)
                        };
                        te.SelectAll();
                        te.Copy();
                    }
                }

                if (Controller.obj?.levelController?.controllerTilemap?.GraphicsTilemaps != null) {
                    for (int i = 0; i < lvl.Maps.Length; i++) {
                        var tilemaps = Controller.obj.levelController.controllerTilemap.GraphicsTilemaps;

                        var isActive = EditorField($"Show layer {i}", tilemaps[i].gameObject.activeSelf);

                        if (isActive != tilemaps[i].gameObject.activeSelf)
                            tilemaps[i].gameObject.SetActive(isActive);
                    }
                }

                if (PalOptions == null)
                    PalOptions = new string[]
                    {
                        "Auto"
                    }.Concat(Enumerable.Range(0, LevelEditorData.Level.Maps.Max(x => x.TileSet.Length)).Select(x => x.ToString())).ToArray();

                if (Controller.obj?.levelController?.controllerTilemap != null) {
                    Controller.obj.levelController.controllerTilemap.currentPalette = EditorField("Palette", Controller.obj.levelController.controllerTilemap.currentPalette, PalOptions);
                }
            }
        }
        else
        {
            PalOptions = null;
        }

        // Game Tools

        DrawHeader("Game Tools");

        // Only update if previous values don't match
        if (CurrentGameActions == null || refreshGameActions)
        {
            if (fileMode != FileSystem.Mode.Web) {
                MapSelectionDropdown.HasChanged = false;
                Settings.EduVolume = MapSelectionDropdown.SelectedVolume;
                Settings.World = MapSelectionDropdown.SelectedWorld;
                Settings.Level = MapSelectionDropdown.SelectedMap;
            }
            Dirty = true;
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
                try {
                    Controller.StartStopwatch();
                    // Run the action
                    await action.GameActionFunc(inputDir, outputDir);
                } finally {
                    Controller.StopStopwatch();
                }
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
                    settings.EduVolume = manager.GetLevels(settings).FirstOrDefault()?.Name;

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
        foreach (var world in manager.GetLevels(settings).First().Worlds)
        {
            // Set the world
            settings.World = world.Index;

            // Enumerate every level
            foreach (var lvl in world.Maps)
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
                //Randomizer.Randomize(editorManager, flag, (int)world.Index + lvl + RandomizerSeed, 0);

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
    public string[] PalOptions { get; set; } = null;

    #endregion

    private GameModeSelectionDropdown GameModeDropdown { get; set; }
    private MapSelectionDropdown MapSelectionDropdown { get; set; }

    private GameAction[] CurrentGameActions { get; set; }

    private float TotalyPos { get; set; }

	private Vector2 ScrollPosition { get; set; } = Vector2.zero;
}