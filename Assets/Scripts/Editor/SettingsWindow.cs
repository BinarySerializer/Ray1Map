using R1Engine;
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

	public void OnGUI()
	{
        FileSystem.Mode fileMode = FileSystem.Mode.Normal;
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL) {
            fileMode = FileSystem.Mode.Web;
        }

        // Increase label width due to it being cut off otherwise
        EditorGUIUtility.labelWidth = 180;

		float yPos = 0f;

		if (TotalyPos == 0f)
            TotalyPos = position.height;

		scrollbarShown = TotalyPos > position.height;
		ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

		EditorGUI.BeginChangeCheck();

        if (fileMode == FileSystem.Mode.Web) {
            EditorGUI.HelpBox(GetNextRect(ref yPos, height: 40f), "Your build target is configured as WebGL. Ray1Map will attempt to load from the server.", MessageType.Warning);
        }

        // Mode

        DrawHeader(ref yPos, "Mode");

		Settings.SelectedGameMode = (GameModeSelection)EditorGUI.Popup(GetNextRect(ref yPos), "Game", (int)Settings.SelectedGameMode, GameModeNames);

		// Map

		DrawHeader(ref yPos, "Map");

        Settings.World = AvailableWorlds.ElementAtOrDefault(EditorGUI.Popup(GetNextRect(ref yPos), "World", AvailableWorlds.FindItemIndex(x => x == Settings.World), AvailableWorldNames));

        try
        {
			// Only update if previous values don't match
			if (!PrevLvlValues.ComparePreviousValues())
            {
                Debug.Log("Updated levels");

                var manager = Settings.GetGameManager;
                var settings = Settings.GetGameSettings;

                CurrentLevels = manager.GetLevels(settings)
                    .Select(x => new KeyValuePair<World, KeyValuePair<int, string>[]>(x.Key, x.Value.OrderBy(i => i)
                        .Select(i => new KeyValuePair<int, string>(i, MapNames.GetMapNames(settings.Game)?.TryGetItem(Settings.World)?.TryGetItem(i)))
                        .ToArray()))
                    .ToArray();
                AvailableWorlds = CurrentLevels.Where(x => x.Value.Any()).Select(x => x.Key).ToArray();
                AvailableWorldNames = AvailableWorlds.Select(x => x.ToString()).ToArray();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }

		var levels = Directory.Exists(Settings.CurrentDirectory) ? CurrentLevels : new KeyValuePair<World, KeyValuePair<int, string>[]>[0];

        var currentLevels = levels.FindItem(x => x.Key == Settings.World).Value ?? new KeyValuePair<int, string>[0];

        if (currentLevels.All(x => x.Key != Settings.Level))
			Settings.Level = currentLevels.FirstOrDefault().Key;

        // Helper method for getting the level name
        string GetLvlName(int lvlNum, string lvlName)
        {
            if (lvlName != null)
                return $"{lvlNum:00} - {lvlName}";
            else
                return $"{lvlNum}";
        }

		var lvlIndex = EditorGUI.Popup(GetNextRect(ref yPos), "Map", currentLevels.FindItemIndex(x => x.Key == Settings.Level), currentLevels.Select(x => GetLvlName(x.Key, x.Value)).ToArray());

        if (currentLevels.Length > lvlIndex && lvlIndex != -1)
            Settings.Level = currentLevels[lvlIndex].Key;

        // Update previous values
        PrevLvlValues.UpdatePreviousValues();

        EditorGUI.BeginDisabledGroup(Settings.SelectedGameMode != GameModeSelection.RaymanEducationalPC && Settings.SelectedGameMode != GameModeSelection.RaymanQuizPC && Settings.SelectedGameMode != GameModeSelection.RaymanEducationalPS1);

        try
		{
            // Only update if previous values don't match
            if (!PrevVolumeValues.ComparePreviousValues())
            {
				Debug.Log("Updated EDU volumes");
                CurrentEduVolumes = Settings.GetGameManager.GetEduVolumes(Settings.GetGameSettings);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }

		var eduIndex = EditorGUI.Popup(GetNextRect(ref yPos), "Volume", CurrentEduVolumes.FindItemIndex(x => x == Settings.EduVolume), CurrentEduVolumes);

		if (CurrentEduVolumes.Length > eduIndex && eduIndex != -1)
			Settings.EduVolume = CurrentEduVolumes[eduIndex];

        // Update previous values
        PrevVolumeValues.UpdatePreviousValues();

        EditorGUI.EndDisabledGroup();

        // Directories
        DrawHeader(ref yPos, "Directories" + (fileMode == FileSystem.Mode.Web ? " (Web)" : ""));

        var modes = EnumHelpers.GetValues<GameModeSelection>();
        if (fileMode == FileSystem.Mode.Web) {
            foreach (var mode in modes) {
                Settings.GameDirectoriesWeb[mode] = EditorGUI.TextField(GetNextRect(ref yPos), mode.GetAttribute<GameModeAttribute>()?.DisplayName ?? "N/A", Settings.GameDirectoriesWeb.TryGetItem(mode, String.Empty));
            }
        } else {
            foreach (var mode in modes) {
                Settings.GameDirectories[mode] = DirectoryField(GetNextRect(ref yPos), mode.GetAttribute<GameModeAttribute>()?.DisplayName ?? "N/A", Settings.GameDirectories.TryGetItem(mode, String.Empty));
            }
        }

        // Miscellaneous

        DrawHeader(ref yPos, "Miscellaneous");

        Settings.UseHDCollisionSheet = EditorGUI.Toggle(GetNextRect(ref yPos), "Use HD collision sheet", Settings.UseHDCollisionSheet);

        Settings.AnimateSprites = EditorGUI.Toggle(GetNextRect(ref yPos), "Animate sprites", Settings.AnimateSprites);

        Settings.ShowAlwaysEvents = EditorGUI.Toggle(GetNextRect(ref yPos), "Show always events", Settings.ShowAlwaysEvents);

        Settings.ShowEditorEvents = EditorGUI.Toggle(GetNextRect(ref yPos), "Show editor events", Settings.ShowEditorEvents);

        Settings.BackupFiles = EditorGUI.Toggle(GetNextRect(ref yPos), "Create .BAK backup files", Settings.BackupFiles);

        Settings.ScreenshotEnumeration = EditorGUI.Toggle(GetNextRect(ref yPos), "Screenshot enumeration", Settings.ScreenshotEnumeration);

        Rect rect = GetNextRect(ref yPos);
        rect = EditorGUI.PrefixLabel(rect, new GUIContent("Serialization log"));
        bool log = Settings.Log;
        rect = PrefixToggle(rect, ref log);
        Settings.Log = log;
        if (Settings.Log) {
            Settings.LogFile = FileField(rect, "Serialization log File", Settings.LogFile, true, "txt", includeLabel: false);
        }

        // Tools

        DrawHeader(ref yPos, "Game Tools");

        // Only update if previous values don't match
        if (!PrevGameActionValues.ComparePreviousValues())
        {
            Debug.Log("Updated game actions");
            CurrentGameActions = Settings.GetGameManager.GetGameActions(Settings.GetGameSettings);
        }

        // Add every export option
        for (var i = 0; i < CurrentGameActions.Length; i++)
        {
            // Get the option
            var action = CurrentGameActions[i];

            if (GUI.Button(GetNextRect(ref yPos), action.DisplayName))
            {
                // Get the directories
                string inputDir = action.RequiresInputDir ? EditorUtility.OpenFolderPanel("Select input directory", null, "") : null;

                if (string.IsNullOrEmpty(inputDir) && action.RequiresInputDir)
                    return;

                string outputDir = action.RequiresOutputDir ? EditorUtility.OpenFolderPanel("Select output directory", null, "") : null;

                if (string.IsNullOrEmpty(outputDir) && action.RequiresOutputDir)
                    return;

                Settings.GetGameManager.RunAction(i, inputDir, outputDir, Settings.GetGameSettings);
            }
        }

        // Update previous values
        PrevGameActionValues.UpdatePreviousValues();

        TotalyPos = yPos;
		GUI.EndScrollView();

		if (EditorGUI.EndChangeCheck() || Dirty)
		{
			Settings.Save();
			Dirty = false;
		}
	}

    private PrevValues PrevLvlValues { get; } = new PrevValues();

    private PrevValues PrevVolumeValues { get; } = new PrevValues();

    private PrevValues PrevGameActionValues { get; } = new PrevValues();

    private string[] GameModeNames { get; } = EnumHelpers.GetValues<GameModeSelection>().Select(x => x.GetAttribute<GameModeAttribute>().DisplayName).ToArray();

    private World[] AvailableWorlds { get; set; } = EnumHelpers.GetValues<World>();

    private string[] AvailableWorldNames { get; set; } = EnumHelpers.GetValues<World>().Select(x => x.ToString()).ToArray();

    private KeyValuePair<World, KeyValuePair<int, string>[]>[] CurrentLevels { get; set; } = new KeyValuePair<World, KeyValuePair<int, string>[]>[0];

	private string[] CurrentEduVolumes { get; set; } = new string[0];

    private GameAction[] CurrentGameActions { get; set; } = new GameAction[0];

    private float TotalyPos { get; set; }

	private Vector2 ScrollPosition { get; set; } = Vector2.zero;

	public class PrevValues
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
        private World PrevWorld { get; set; } = World.Jungle;

        private string PrevEduVolume { get; set; } = String.Empty;
    }
}