using System;
using R1Engine;
using System.ComponentModel;
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

		Settings.Mode = (GameMode)EditorGUI.EnumPopup(GetNextRect(ref yPos), new GUIContent("Game"), Settings.Mode);

		// Map

		DrawHeader(ref yPos, "Map");

        Settings.World = (World)EditorGUI.EnumPopup(GetNextRect(ref yPos), new GUIContent("World"), Settings.World);

		var levels = Directory.Exists(Settings.CurrentDirectory) ? Enumerable.Range(1, Settings.GetManager().GetLevelCount(Settings.CurrentDirectory, Settings.World)).ToArray() : new int[0];

		Settings.Level = EditorGUI.IntPopup(GetNextRect(ref yPos), "Map", Settings.Level, levels.Select(x => x.ToString()).ToArray(), levels);

		// Directories

		DrawHeader(ref yPos, "Directories");

        foreach (var mode in EnumHelpers.GetValues<GameMode>())
        {
            Settings.GameDirectories[mode] = DirectoryField(GetNextRect(ref yPos), mode.GetAttribute<DescriptionAttribute>()?.Description, Settings.GameDirectories.TryGetValue(mode, out var dir) ? dir : String.Empty);
        }

        TotalyPos = yPos;
		GUI.EndScrollView();

		if (EditorGUI.EndChangeCheck() || Dirty)
		{
			Settings.Save();
			Dirty = false;
		}
	}

    private float TotalyPos { get; set; }

	private Vector2 ScrollPosition { get; set; } = Vector2.zero;
}