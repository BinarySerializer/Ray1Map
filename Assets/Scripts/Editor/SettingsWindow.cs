using Newtonsoft.Json;
using R1Engine;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

		Settings.Mode = (GameMode)EditorGUI.EnumPopup(GetNextRect(ref yPos), "Game", Settings.Mode);

		// Map

		DrawHeader(ref yPos, "Map");

        Settings.World = (World)EditorGUI.EnumPopup(GetNextRect(ref yPos), "World", Settings.World);

        try
        {
			// Only update if previous values don't match
			if (!ComparePreviousValues())
                CurrentLevelCount = Settings.GetManager().GetLevelCount(Settings.CurrentDirectory, Settings.World);
        }
        catch (Exception ex)
        {
            // TODO: Log
        }

		var levels = Directory.Exists(Settings.CurrentDirectory) ? Enumerable.Range(1, CurrentLevelCount).ToArray() : new int[0];

		Settings.Level = EditorGUI.IntPopup(GetNextRect(ref yPos), "Map", Settings.Level, levels.Select(x => x.ToString()).ToArray(), levels);

		// Directories

		DrawHeader(ref yPos, "Directories");

        foreach (var mode in EnumHelpers.GetValues<GameMode>())
        {
            Settings.GameDirectories[mode] = DirectoryField(GetNextRect(ref yPos), mode.GetAttribute<DescriptionAttribute>()?.Description, Settings.GameDirectories.TryGetValue(mode, out var dir) ? dir : String.Empty);
        }

        // Miscellaneous

        DrawHeader(ref yPos, "Miscellaneous");

        Settings.UseHDCollisionSheet = EditorGUI.Toggle(GetNextRect(ref yPos), "Use HD collision sheet", Settings.UseHDCollisionSheet);

        // Update previous values
		UpdatePreviousValues();

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
    }

	/// <summary>
	/// Compares previous values and returns true if they're the same
	/// </summary>
	/// <returns>True if they're the same, otherwise false</returns>
	private bool ComparePreviousValues()
    {
		return PrevDir == Settings.CurrentDirectory && PrevWorld == Settings.World;
    }

	/// <summary>
	/// The previously saved current directory
	/// </summary>
	private string PrevDir { get; set; }

	/// <summary>
	/// The previously saved world
	/// </summary>
	private World PrevWorld { get; set; }

	private int CurrentLevelCount { get; set; }

    private float TotalyPos { get; set; }

	private Vector2 ScrollPosition { get; set; } = Vector2.zero;
}

public class FileSerializerWindow : UnityWindow
{
	[MenuItem("Ray1Map/File Serializer")]
	public static void ShowWindow()
	{
		GetWindow<FileSerializerWindow>("File Serializer");
	}
	private void OnEnable()
	{
		titleContent = EditorGUIUtility.IconContent("TextAsset Icon");
		titleContent.text = "File Serializer";
	}

	void OnGUI()
	{
		float yPos = 0f;

        if (TotalyPos == 0f)
            TotalyPos = position.height;

        scrollbarShown = TotalyPos > position.height;

        ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

        DrawHeader(ref yPos, "File Serializer");

        SelectedInputFile = FileField(GetNextRect(ref yPos), "Input File", SelectedInputFile, false, "*");
        SelectedOutputFile = FileField(GetNextRect(ref yPos), "Output File", SelectedOutputFile, true, "*");

		SelectedDataTypeIndex = EditorGUI.Popup(GetNextRect(ref yPos), "Data Type", SelectedDataTypeIndex, FileFactory.SerializableDataTypes.Select(x => x.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "Unknown").ToArray());

		if (GUI.Button(GetNextRect(ref yPos), new GUIContent("Serialize to JSON")))
        {
			if (!File.Exists(SelectedInputFile))
				throw new Exception("Input file doesn't exist");

            // Open the file
            using (var file = File.OpenRead(SelectedInputFile))
            {
                // Create the file
                var fileData = (ISerializableFile)Activator.CreateInstance(FileFactory.SerializableDataTypes[SelectedDataTypeIndex]);

				// Deserialize the file
				fileData.Deserialize(file);

				// Serialize to JSON
				var json = JsonConvert.SerializeObject(fileData, Formatting.Indented, new ByteArrayHexConverter());

				// Write to output
				File.WriteAllText(SelectedOutputFile, json);
            }
        }

		if (GUI.Button(GetNextRect(ref yPos), new GUIContent("Deserialize from JSON")))
        {
            if (!File.Exists(SelectedInputFile))
                throw new Exception("Input file doesn't exist");

			// Deserialize the file
			var fileData = (ISerializableFile)JsonConvert.DeserializeObject(File.ReadAllText(SelectedInputFile), FileFactory.SerializableDataTypes[SelectedDataTypeIndex], new ByteArrayHexConverter());

			// Create the file
            using (var file = File.Create(SelectedOutputFile))
            {
                // Serialize the file
                file.Write(fileData);
            }
        }

		TotalyPos = yPos;
        GUI.EndScrollView();
    }

	private string SelectedInputFile { get; set; }

	private string SelectedOutputFile { get; set; }

	private int SelectedDataTypeIndex { get; set; }

    private float TotalyPos { get; set; }

	private Vector2 ScrollPosition { get; set; } = Vector2.zero;
}