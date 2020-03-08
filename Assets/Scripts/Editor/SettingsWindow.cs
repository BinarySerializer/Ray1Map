using R1Engine;
using System;
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
        // Increase label width due to it being cut off otherwise
		EditorGUIUtility.labelWidth = 180;

		float yPos = 0f;

		if (TotalyPos == 0f)
            TotalyPos = position.height;

		scrollbarShown = TotalyPos > position.height;
		ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

		EditorGUI.BeginChangeCheck();

		// Mode

		DrawHeader(ref yPos, "Mode");

		Settings.SelectedGameMode = (GameModeSelection)EditorGUI.EnumPopup(GetNextRect(ref yPos), "Game", Settings.SelectedGameMode);

		// Map

		DrawHeader(ref yPos, "Map");

        Settings.World = (World)EditorGUI.EnumPopup(GetNextRect(ref yPos), "World", Settings.World);

        try
        {
			// Only update if previous values don't match
			if (!ComparePreviousValues())
            {
                Debug.Log("Updated levels");
                CurrentLevels = Settings.GetGameManager.GetLevels(Settings.GetGameSettings);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }

		var levels = Directory.Exists(Settings.CurrentDirectory) ? CurrentLevels : new int[0];

		if (!levels.Contains(Settings.Level))
			Settings.Level = 1;

		var lvlIndex = EditorGUI.Popup(GetNextRect(ref yPos), "Map", levels.FindItemIndex(x => x == Settings.Level), levels.Select(x => x.ToString()).ToArray());

        if (levels.Length > lvlIndex && lvlIndex != -1)
            Settings.Level = levels[lvlIndex];

        // Update previous values
        UpdatePreviousValues();

		try
		{
            // Only update if previous values don't match
            if (!ComparePreviousValues())
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

		// Directories

		DrawHeader(ref yPos, "Directories");

        foreach (var mode in EnumHelpers.GetValues<GameModeSelection>())
        {
            Settings.GameDirectories[mode] = DirectoryField(GetNextRect(ref yPos), mode.GetAttribute<GameModeAttribute>()?.DisplayName ?? "N/A", Settings.GameDirectories.TryGetValue(mode, out var dir) ? dir : String.Empty);
        }

        // Miscellaneous

        DrawHeader(ref yPos, "Miscellaneous");

        Settings.UseHDCollisionSheet = EditorGUI.Toggle(GetNextRect(ref yPos), "Use HD collision sheet", Settings.UseHDCollisionSheet);

        Settings.AnimateSprites = EditorGUI.Toggle(GetNextRect(ref yPos), "Animate sprites", Settings.AnimateSprites);

        Settings.ShowAlwaysEvents = EditorGUI.Toggle(GetNextRect(ref yPos), "Show always events", Settings.ShowAlwaysEvents);

		TotalyPos = yPos;
		GUI.EndScrollView();

		if (EditorGUI.EndChangeCheck() || Dirty)
		{
			Settings.Save();
			Dirty = false;
		}
	}

	/// <summary>
	/// Updates saved previous values
	/// </summary>
	private void UpdatePreviousValues()
    {
        PrevDir = Settings.CurrentDirectory;
        PrevWorld = Settings.World;
        PrevEduVolume = Settings.EduVolume;
    }

	/// <summary>
	/// Compares previous values and returns true if they're the same
	/// </summary>
	/// <returns>True if they're the same, otherwise false</returns>
	private bool ComparePreviousValues()
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

	private int[] CurrentLevels { get; set; } = new int[0];

	private string[] CurrentEduVolumes { get; set; } = new string[0];

    private float TotalyPos { get; set; }

	private Vector2 ScrollPosition { get; set; } = Vector2.zero;
}