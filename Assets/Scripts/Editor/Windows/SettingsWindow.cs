using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R1Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerializer;
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

    protected override void UpdateEditorFields() {
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

        if (CategorizedGameModes == null)
            CategorizedGameModes = EnumHelpers.GetValues<GameModeSelection>().
                GroupBy(x => x.GetAttribute<GameModeAttribute>().MajorEngineVersion).
                ToDictionary(x => x.Key, x => 
                    x.GroupBy(y => y.GetAttribute<GameModeAttribute>().EngineVersion).ToDictionary(y => y.Key, y => y.ToArray()));

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

        if (Settings.LoadFromMemory) 
        {
            DrawHeader("Memory");

            DefaultMemoryOptionsIndex = EditorField("Default memory options", DefaultMemoryOptionsIndex, DefaultMemoryOptionDisplayNames);

            if (DefaultMemoryOptionsIndex != PreviousDefaultMemoryOptionsIndex) 
            {
                if (PreviousDefaultMemoryOptionsIndex == -1) {
                    var match = Enumerable.Range(0, DefaultMemoryOptionDisplayNames.Length - 1).FirstOrDefault(x => 
                        DefaultMemoryConfigs[x].IsPointer == Settings.IsGameBaseAPointer &&
                        DefaultMemoryConfigs[x].ProcessName == Settings.ProcessName &&
                        DefaultMemoryConfigs[x].ModuleName == Settings.ModuleName &&
                        DefaultMemoryConfigs[x].Offset == Settings.GameBasePointer);

                    DefaultMemoryOptionsIndex = match + 1;
                }
                
                PreviousDefaultMemoryOptionsIndex = DefaultMemoryOptionsIndex;

                if (DefaultMemoryOptionsIndex != 0) 
                {
                    Settings.IsGameBaseAPointer = DefaultMemoryConfigs[DefaultMemoryOptionsIndex - 1].IsPointer;
                    Settings.ProcessName = DefaultMemoryConfigs[DefaultMemoryOptionsIndex - 1].ProcessName;
                    Settings.ModuleName = DefaultMemoryConfigs[DefaultMemoryOptionsIndex - 1].ModuleName;
                    Settings.GameBasePointer = DefaultMemoryConfigs[DefaultMemoryOptionsIndex - 1].Offset;
                }
            }

            EditorGUI.BeginDisabledGroup(DefaultMemoryOptionsIndex != 0);

            Settings.ProcessName = EditorField("Process name", Settings.ProcessName);
            Settings.ModuleName = EditorField("Module name", Settings.ModuleName);
            Settings.GameBasePointer = EditorField("Game memory offset", Settings.GameBasePointer);
            Settings.IsGameBaseAPointer = EditorField("Is offset a pointer", Settings.IsGameBaseAPointer);

            EditorGUI.EndDisabledGroup();

            Settings.FindPointerAutomatically = EditorField("Find pointer automatically", Settings.FindPointerAutomatically);
        }

        // Map

        DrawHeader("Map");

        if (fileMode == FileSystem.Mode.Normal) 
        {
            rectTemp = GetNextRect(ref YPos);
            rbutton = EditorGUI.PrefixLabel(rectTemp, new GUIContent("Map"));
            rectTemp = new Rect(rbutton.x + rbutton.width - Mathf.Max(400f, rbutton.width), rbutton.y, Mathf.Max(400f, rbutton.width), rbutton.height);

            // Map selection dropdown
            if (MapSelectionDropdown == null || GameModeDropdown.HasChanged) {
                if (GameModeDropdown.HasChanged) {
                    Settings.SelectedGameMode = GameModeDropdown.Selection;
                    GameModeDropdown.HasChanged = false;
                    Dirty = true;
                }

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

            // Next & previous map buttons
            BrowseButton(rbutton, "Next map", EditorGUIUtility.IconContent("Profiler.NextFrame"), () => {
                var vol = MapSelectionDropdown.GameVolumes.First();
                var world = vol.Worlds[MapSelectionDropdown.SelectedWorld];
                var curMap = world.Maps.FindItemIndex(m => m == MapSelectionDropdown.SelectedMap);

                if (curMap + 1 >= world.Maps.Length) {
                    MapSelectionDropdown.SelectedWorld++;
                    if (MapSelectionDropdown.SelectedWorld >= vol.Worlds.Length) MapSelectionDropdown.SelectedWorld = 0;
                    MapSelectionDropdown.SelectedMap = vol.Worlds[MapSelectionDropdown.SelectedWorld].Maps[0];
                } else {
                    MapSelectionDropdown.SelectedMap = world.Maps[curMap+1];
                }

                MapSelectionDropdown.HasChanged = true;
            }, ButtonWidth);
            rbutton = new Rect(rbutton.x, rbutton.y, rbutton.width - ButtonWidth, rbutton.height);
            BrowseButton(rbutton, "Previous map", EditorGUIUtility.IconContent("Profiler.PrevFrame"), () => {
                var vol = MapSelectionDropdown.GameVolumes.First();
                var world = vol.Worlds[MapSelectionDropdown.SelectedWorld];
                var curMap = world.Maps.FindItemIndex(m => m == MapSelectionDropdown.SelectedMap);

                if (curMap <= 0) {
                    MapSelectionDropdown.SelectedWorld--;
                    if (MapSelectionDropdown.SelectedWorld < 0) MapSelectionDropdown.SelectedWorld = vol.Worlds.Length - 1;
                    world = vol.Worlds[MapSelectionDropdown.SelectedWorld];
                    MapSelectionDropdown.SelectedMap = world.Maps[world.Maps.Length-1];
                } else {
                    MapSelectionDropdown.SelectedMap = world.Maps[curMap - 1];
                }

                MapSelectionDropdown.HasChanged = true;
            }, ButtonWidth);
            rbutton = new Rect(rbutton.x, rbutton.y, rbutton.width - ButtonWidth, rbutton.height);

            // Map selection dropdown button
            if (EditorGUI.DropdownButton(rbutton, new GUIContent($"{(!string.IsNullOrEmpty(Settings.EduVolume) ? $"{Settings.EduVolume} - " : String.Empty)}{MapSelectionDropdown.GetWorldName(Settings.World)} {MapSelectionDropdown.GetLevelName(Settings.World, Settings.Level)}"), FocusType.Passive))
                MapSelectionDropdown.Show(rectTemp);
        } 
        else if (fileMode == FileSystem.Mode.Web)
        {
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

        foreach (var modeCategory in CategorizedGameModes)
        {
            Settings.HideDirectories[modeCategory.Key] = !EditorGUI.Foldout(GetNextRect(ref YPos), !Settings.HideDirectories.TryGetItem(modeCategory.Key, true), $"Directories ({modeCategory.Key})", true);

            if (!Settings.HideDirectories[modeCategory.Key])
            {
                foreach (var engine in modeCategory.Value)
                {
                    YPos += 8;

                    var modes = engine.Value;
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

                YPos += 8;
            }
        }

        // Visibility

        DrawHeader("Visibility");

        Settings.ShowObjects = EditorField("Show objects (O)", Settings.ShowObjects);

        Settings.ShowLinks = EditorField("Show links (L)", Settings.ShowLinks);

        Settings.ShowTiles = EditorField("Show tiles (T)", Settings.ShowTiles);

        Settings.ShowCollision = EditorField("Show tile collision (C)", Settings.ShowCollision);

        Settings.ShowGridMap = EditorField("Show grid map", Settings.ShowGridMap);

        Settings.ShowObjCollision = EditorField("Show object collision (B)", Settings.ShowObjCollision);

        Settings.ShowAlwaysObjects = EditorField("Show always objects (G)", Settings.ShowAlwaysObjects);

        Settings.ShowEditorObjects = EditorField("Show editor objects (E)", Settings.ShowEditorObjects);

        Settings.ShowDefaultObjIcons = EditorField("Show gizmos", Settings.ShowDefaultObjIcons);

        Settings.ShowObjOffsets = EditorField("Show object offsets (X)", Settings.ShowObjOffsets);

        Settings.ShowRayman = EditorField("Show Rayman object (R)", Settings.ShowRayman);

        Settings.HideUnusedLinks = EditorField("Hide unused links", Settings.HideUnusedLinks);

        // Miscellaneous

        DrawHeader("Miscellaneous");

        Settings.UseHDCollisionSheet = EditorField("Use HD collision sheet", Settings.UseHDCollisionSheet);

        Settings.AnimateSprites = EditorField("Animate sprites (P)", Settings.AnimateSprites);

        Settings.AnimateTiles = EditorField("Animate tiles (Y)", Settings.AnimateTiles);

        Settings.ShowDebugInfo = EditorField("Show debug info", Settings.ShowDebugInfo);

        Settings.FollowRaymanInMemoryMode = EditorField("Follow Rayman in memory mode", Settings.FollowRaymanInMemoryMode);

        // Game Settings

        DrawHeader("Game Settings");

        Settings.StateSwitchingMode = EditorField("R1: State switching", Settings.StateSwitchingMode);

        Settings.LoadIsometricMapLayer = EditorField("Isometric: Load map layer", Settings.LoadIsometricMapLayer);

        Settings.GBAVV_Crash_TimeTrialMode = EditorField("GBAVV Crash: Time trial mode", Settings.GBAVV_Crash_TimeTrialMode);

        // Editor Tools
        if (Application.isPlaying && Controller.LoadState == Controller.State.Finished) {
            var lvl = LevelEditorData.Level;

            if (lvl != null) {
                DrawHeader("Editor Tools");

                if (EditorButton("Copy localization")) 
                {
                    if (lvl.Localization != null)
                        JsonConvert.SerializeObject(lvl.Localization.ToDictionary(x => x.Key, x => x.Value), Formatting.Indented).CopyToClipboard();
                }

                if (LevelEditorData.ObjManager is Unity_ObjectManager_R1 r1 && r1.EventFlags != null) {
                    if (EditorButton("Copy event flag info"))
                        r1.GetEventFlagsDebugInfo().CopyToClipboard();
                }
                
                if (LevelEditorData.ObjManager is Unity_ObjectManager_GBAVV vv && vv.Scripts != null) {
                    if (EditorButton("Copy scripts"))
                    {
                        var str = new StringBuilder();

                        // Enumerate every script
                        foreach (var script in vv.Scripts)
                        {
                            foreach (var line in script.TranslatedString(vv.Graphics?.FirstOrDefault()?.AnimSets, vv.LocPointerTable))
                                str.AppendLine(line);

                            str.AppendLine();
                        }

                        str.ToString().CopyToClipboard();
                    }
                }

                if (LevelEditorData.Level?.IsometricData != null) {
                    var cam = Controller.obj?.levelController?.editor?.cam;
                    if (cam != null) {
                        cam.ToggleFreeCameraMode(EditorField($"Free camera mode (F)", cam.FreeCameraMode));
                    }
                }

                if (LevelEditorData.Level.CanMoveAlongTrack) 
                {
                    var cam = Controller.obj?.levelController?.editor?.cam;
                    if (cam != null) {
                        cam.ToggleTrackMoving(EditorField($"Move along track", cam.IsTrackMovingEnabled));
                    }
                }

                if (Controller.obj?.levelController?.controllerTilemap != null) {
                    var tilemapController = Controller.obj.levelController.controllerTilemap;

                    var layerVisibilities = tilemapController.IsLayerVisible;
                    if (layerVisibilities != null) {
                        for (int i = 0; i < layerVisibilities.Length; i++) {
                            layerVisibilities[i] = EditorField($"Show {LevelEditorData.Level.Layers[i].Name}", layerVisibilities[i]);
                        }
                    }
                    if (LevelEditorData.Level?.Layers != null) {
                        for (int i = 0; i < LevelEditorData.Level.Layers.Length; i++) {
                            if (LevelEditorData.Level.Layers[i] != null) {
                                bool canBeTiled = false;
                                Sprite spr = null;
                                bool wasTiled = false;
                                var l = LevelEditorData.Level.Layers[i];
                                switch (l) {
                                    case Unity_Layer_Map lm:
                                        canBeTiled = true;
                                        spr = lm.Graphics?.sprite;
                                        wasTiled = lm.Graphics?.drawMode == SpriteDrawMode.Tiled;
                                        break;
                                    case Unity_Layer_Texture lt:
                                        canBeTiled = true;
                                        spr = lt.Graphics?.sprite;
                                        wasTiled = lt.Graphics?.drawMode == SpriteDrawMode.Tiled;
                                        break;
                                }
                                if(!canBeTiled) continue;
                                if (spr == null
                                    || spr.rect.width / spr.pixelsPerUnit != LevelEditorData.MaxWidth * tilemapController.CellSizeInUnits
                                    || spr.rect.height / spr.pixelsPerUnit != LevelEditorData.MaxHeight * tilemapController.CellSizeInUnits) {
                                    bool setTiled = EditorField($"Tile {l.Name}", wasTiled);

                                    if (setTiled != wasTiled) {
                                        tilemapController.SetGraphicsLayerTiled(i, setTiled);
                                    }
                                }
                            }
                        }
                    }
                }

                if (LevelEditorData.ShowEventsForMaps != null)
                    for (int i = 0; i < LevelEditorData.ShowEventsForMaps.Length; i++)
                        LevelEditorData.ShowEventsForMaps[i] = EditorField($"Show objects for layer {i}", LevelEditorData.ShowEventsForMaps[i]);

                if (LevelEditorData.Level.ObjectGroups != null)
                    LevelEditorData.SelectedObjectGroup = EditorField("Object group", LevelEditorData.SelectedObjectGroup, LevelEditorData.Level.ObjectGroups);

                if (LevelEditorData.Level?.Sectors != null) {
                    LevelEditorData.ShowOnlyActiveSector = EditorField("Show only active sector", LevelEditorData.ShowOnlyActiveSector);

                    LevelEditorData.ActiveSector = EditorField("Active sector", LevelEditorData.ActiveSector, LevelEditorData.Level.Sectors.Select((x, i) => i.ToString()).ToArray(), isVisible: LevelEditorData.ShowOnlyActiveSector);
                }

                if (PalOptions == null) {
                    if (Controller.obj.levelController.controllerTilemap.HasAutoPaletteOption) {
                        PalOptions = new string[]
                        {
                            "Auto"
                        }.Concat(Enumerable.Range(0, LevelEditorData.Level.Maps?.Max(x => x.TileSet.Length) ?? 0).Select(x => x.ToString())).ToArray();
                    } else {
                        PalOptions = Enumerable.Range(0, LevelEditorData.Level.Maps?.Max(x => x.TileSet.Length) ?? 0).Select(x => x.ToString()).ToArray();
                    }
                }

                if (Controller.obj?.levelController?.controllerTilemap != null) {
                    if (Controller.obj.levelController.controllerTilemap.HasAutoPaletteOption) {
                        Controller.obj.levelController.controllerTilemap.currentPalette = EditorField("Palette", Controller.obj.levelController.controllerTilemap.currentPalette, PalOptions);
                    } else {
                        Controller.obj.levelController.controllerTilemap.currentPalette = EditorField("Palette", Controller.obj.levelController.controllerTilemap.currentPalette - 1, PalOptions) + 1;
                    }
                }
            }
        } else {
            PalOptions = null;
        }

        // Screenshots

        DrawHeader("Screenshots");

        Settings.ScreenshotEnumeration = EditorField("Screenshot enumeration", Settings.ScreenshotEnumeration);

        Settings.Screenshot_FileName = EditorField("File name", Settings.Screenshot_FileName);

        Settings.Screenshot_ShowDefaultObj = EditorField("Show default object", Settings.Screenshot_ShowDefaultObj);

        Settings.Screenshot_RayWikiMode = EditorField("RayWiki mode", Settings.Screenshot_RayWikiMode);

        // Serialization

        DrawHeader("Serialization");

        Settings.BackupFiles = EditorField("Create .BAK backup files", Settings.BackupFiles);

        Rect rect = GetNextRect(ref YPos);
        rect = EditorGUI.PrefixLabel(rect, new GUIContent("Serialization log"));
        bool log = Settings.Log;
        rect = PrefixToggle(rect, ref log);
        Settings.Log = log;

        if (Settings.Log)
            Settings.LogFile = FileField(rect, "Serialization log File", Settings.LogFile, true, "txt", includeLabel: false);

        // External Tools

        DrawHeader("External Tools");

        Settings.Tool_mkpsxiso_filePath = FileField(GetNextRect(ref YPos), "mkpsxiso path", Settings.Tool_mkpsxiso_filePath, false, "exe");

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

                async UniTask ExecuteGameAction(GameAction action) {
                    try {
                        Controller.StartStopwatch();
                        // Run the action
                        await action.GameActionFunc(inputDir, outputDir);
                    } catch (Exception ex) {
                        Debug.LogError(ex.ToString());
                    } finally {
                        Controller.StopStopwatch();
                    }
                }
                _ = ExecuteGameAction(action);
            }
        }

        // Global Tools

        DrawHeader("Global Tools");

        void AddGlobalAction(string actionName)
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

                    async UniTask ExecuteGameAction(GameAction action) {
                        try {
                            Controller.StartStopwatch();
                            // Run the action
                            await action.GameActionFunc(null, Path.Combine(outputDir, mode.ToString()));
                        } catch (Exception ex) {
                            Debug.LogWarning($"Mode {mode} failed with exception: {ex.Message}");
                        } finally {
                            Controller.StopStopwatch();

                            // Unload textures
                            await Resources.UnloadUnusedAssets();
                        }
                    }
                    _ = ExecuteGameAction(action);
                }
            }
        }

        // Add special game actions
        AddGlobalAction("Export Sprites");
        AddGlobalAction("Export Animation Frames");
        AddGlobalAction("Export Vignette");

        // Randomizer

        DrawHeader("Randomizer");

        if (EditorButton("Run Batch Randomizer"))
            _ = BatchRandomizeAsync();

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
        try
        {
            // Get the settings
            var settings = Settings.GetGameSettings;

            // Init
            await LevelEditorData.InitAsync(settings);

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
                    var context = new R1Context(settings);

                    // Load the files
                    await manager.LoadFilesAsync(context);

                    // Load the level
                    var level = await manager.LoadAsync(context);

                    // Randomize (only first map for now)
                    Randomizer.Randomize(level, flag, world.Index + lvl + RandomizerSeed, 0);

                    // Save the level
                    await manager.SaveLevelAsync(context, level);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private int RandomizerSeed { get; set; }
    private RandomizerFlags RandomizerFlags { get; set; }

    #endregion

    #region Default Memory Options

    public string[] DefaultMemoryOptionDisplayNames { get; } = new string[]
    {
        "Custom"
    }.Concat(DefaultMemoryConfigs.Select(x => x.DisplayName)).ToArray();

    protected static DefaultMemoryConfig[] DefaultMemoryConfigs = new DefaultMemoryConfig[]
    {
        new DefaultMemoryConfig("DOSBox 0.74", 0x01D3A1A0, true, "DOSBox.exe", String.Empty), 
        new DefaultMemoryConfig("BizHawk 2.4.0", 0x0011D880, false, "EmuHawk.exe", "octoshock.dll"),
    };

    public class DefaultMemoryConfig
    {
        public DefaultMemoryConfig(string displayName, int offset, bool isPointer, string processName, string moduleName)
        {
            DisplayName = displayName;
            Offset = offset;
            IsPointer = isPointer;
            ProcessName = processName;
            ModuleName = moduleName;
        }

        public string DisplayName { get; }
        public int Offset { get; }
        public bool IsPointer { get; }
        public string ProcessName { get; }
        public string ModuleName { get; }
    }

    #endregion

    #region Private Properties

    // Properties
    private float TotalyPos { get; set; }
    private Vector2 ScrollPosition { get; set; } = Vector2.zero;

    // Default memory options
    public int DefaultMemoryOptionsIndex { get; set; }
    public int PreviousDefaultMemoryOptionsIndex { get; set; } = -1;

    // Available options
    private GameAction[] CurrentGameActions { get; set; }
    private string[] PalOptions { get; set; }

    // Categorized game modes
    public Dictionary<MajorEngineVersion, Dictionary<EngineVersion, GameModeSelection[]>> CategorizedGameModes { get; set; }

    // Dropdowns
    private GameModeSelectionDropdown GameModeDropdown { get; set; }
    private MapSelectionDropdown MapSelectionDropdown { get; set; }

    #endregion
}