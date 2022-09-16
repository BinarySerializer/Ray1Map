using System;
using System.Linq;
using PsychoPortal.Unity;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Ray1Map.Psychonauts
{
    [CustomEditor(typeof(MeshLoaderComponent))]
    [CanEditMultipleObjects]
    public class MeshLoaderEditor : Editor
    {
        private FilePathDropDown _filePathDropDown;

        private string[] GetMeshFilePaths(FileManager fileManager)
        {
            return fileManager.EnumerateFiles(FileLocation.Any, includeUnnamedPS2Files: true).
                Where(x => x.FilePath.StartsWith("0x") || // Include every unnamed file for now
                           x.FilePath.EndsWith(".plb", StringComparison.OrdinalIgnoreCase) ||
                           x.FilePath.EndsWith(".pl2", StringComparison.OrdinalIgnoreCase)).
                Select(x => x.FilePath).
                ToArray();
        }

        public override async void OnInspectorGUI()
        {
            MeshLoaderComponent comp = (MeshLoaderComponent)serializedObject.targetObject;

            _filePathDropDown ??= new FilePathDropDown(
                new AdvancedDropdownState(), "Mesh File", GetMeshFilePaths(comp.Loader.FileManager), comp.MeshFilePath);

            UnityEngine.Rect rect = GUILayoutUtility.GetRect(new GUIContent(_filePathDropDown.SelectedFilePath), EditorStyles.toolbarButton);

            var dropButton = EditorGUI.PrefixLabel(rect, new GUIContent("Mesh File"));
            rect = new UnityEngine.Rect(dropButton.x + dropButton.width - Mathf.Max(400f, dropButton.width), dropButton.y, Mathf.Max(400f, dropButton.width), dropButton.height);

            if (EditorGUI.DropdownButton(dropButton, new GUIContent(_filePathDropDown.SelectedFilePath), FocusType.Passive))
                _filePathDropDown.Show(rect);

            if (_filePathDropDown.IsDirty)
            {
                _filePathDropDown.IsDirty = false;
                comp.MeshFilePath = _filePathDropDown.SelectedFilePath;
            }

            EditorGUILayout.Separator();

            if (EditorGUILayout.LinkButton("Load"))
                await comp.LoadMeshAsync();

            // PS2 specific
            if (comp.Manager is Psychonauts_Manager_PS2)
            {
                if (EditorGUILayout.LinkButton("Convert PL2 to PLB"))
                    await comp.ConvertPL2ToPLBAsync();

                comp.PS2_CreateDummyColors = EditorGUILayout.Toggle("Create dummy colors", comp.PS2_CreateDummyColors);
                comp.PS2_IgnoreColorsForFlag19 = EditorGUILayout.Toggle("Ignore colors for Flag_19", comp.PS2_IgnoreColorsForFlag19);
                comp.PS2_IgnoreColors = EditorGUILayout.Toggle("Ignore colors", comp.PS2_IgnoreColors);
                comp.PS2_IgnoreBlackColors = EditorGUILayout.Toggle("Ignore black colors", comp.PS2_IgnoreBlackColors);
                comp.PS2_RemoveFlag19 = EditorGUILayout.Toggle("Remove Flag_19", comp.PS2_RemoveFlag19);
                comp.PS2_InvertNormalsForTexture = EditorGUILayout.IntField("Invert normals for texture", comp.PS2_InvertNormalsForTexture);
            }

            EditorGUILayout.Separator();

            comp.ScreenshotMeshFilePaths = EditorGUILayout.TextArea(comp.ScreenshotMeshFilePaths);
            
            if (EditorGUILayout.LinkButton("Screenshot meshes"))
                await comp.ScreenshotMeshesAsync();
        }
    }
}