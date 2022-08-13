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
            return fileManager.EnumerateFiles(FileLocation.Any).
                Where(x => x.FilePath.EndsWith(".plb", StringComparison.OrdinalIgnoreCase) ||
                           x.FilePath.EndsWith(".pl2", StringComparison.OrdinalIgnoreCase)).
                Select(x => x.FilePath).
                ToArray();
        }

        public override void OnInspectorGUI()
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
                comp.LoadMesh();

            if (comp.Manager is Psychonauts_Manager_PS2 && EditorGUILayout.LinkButton("Convert PL2 to PLB"))
                comp.ConvertPL2ToPLB();
        }
    }
}