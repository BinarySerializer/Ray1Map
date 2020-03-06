using R1Engine;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
// ReSharper disable UnusedMember.Local

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
                var fileData = (IBinarySerializable)Activator.CreateInstance(FileFactory.SerializableDataTypes[SelectedDataTypeIndex]);

                // Create the deserializer
                var deserializer = new BinaryDeserializer(file, SelectedInputFile, Settings.GetGameSettings);

                // Deserialize the file
                fileData.Deserialize(deserializer);

                // Serialize to JSON
                JsonHelpers.SerializeToFile(fileData, SelectedOutputFile);

                Debug.Log("File has been serialized");
            }
        }

        if (GUI.Button(GetNextRect(ref yPos), new GUIContent("Deserialize from JSON")))
        {
            if (!File.Exists(SelectedInputFile))
                throw new Exception("Input file doesn't exist");

            // Deserialize the file
            var fileData = JsonHelpers.DeserializeFromFile<IBinarySerializable>(SelectedInputFile, FileFactory.SerializableDataTypes[SelectedDataTypeIndex]);

            // Create the file
            using (var file = File.Create(SelectedOutputFile))
            {
                // Create the serializer
                var serializer = new BinarySerializer(file, SelectedInputFile, Settings.GetGameSettings);

                // Serialize the file
                serializer.Write(fileData);
            }

            Debug.Log("File has been deserialized");
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