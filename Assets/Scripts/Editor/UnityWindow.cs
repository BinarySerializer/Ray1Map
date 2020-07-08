using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnityWindow : EditorWindow
{
    protected Rect GetNextRect(ref float yPos, float padding = 4f, float height = 0f, float vPadding = 0f, float vPaddingBottom = 0f)
    {
        if (height == 0f) 
            height = EditorGUIUtility.singleLineHeight;

        Rect rect = new Rect(padding, yPos + vPadding, Mathf.Max(0f, EditorGUIUtility.currentViewWidth - padding * 2f - (scrollbarShown ? scrollbarWidth : 0f)), height);

        yPos += height + vPadding * 2f + vPaddingBottom;

        return rect;
    }
    protected void DrawHeader(ref float yPos, string title)
    {
        if (yPos > 0)
        {
            Rect rect = GetNextRect(ref yPos, padding: 0f, height: EditorStyles.toolbarButton.fixedHeight, vPadding: 4f);
            EditorGUI.LabelField(rect, new GUIContent(title), EditorStyles.toolbarButton);
        }
        else
        {
            Rect rect = GetNextRect(ref yPos, padding: 0f, height: EditorStyles.toolbarButton.fixedHeight, vPaddingBottom: 4f);
            EditorGUI.LabelField(rect, new GUIContent(title), EditorStyles.toolbarButton);
        }
    }
    protected Rect PrefixToggle(Rect rect, ref bool value) {
        value = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.height, rect.height), value);
        return new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height);
    }

    protected void BrowseButton(Rect rect, string name, GUIContent content, Action action, int width)
    {
        GUIStyle butStyle = EditorStyles.miniButtonRight;
        Rect buttonRect = new Rect(rect.x + rect.width - width, rect.y, width, rect.height);
        GUI.SetNextControlName("Button " + name);

        if (GUI.Button(buttonRect, content, butStyle))
            action();
    }

    protected string DirectoryField(Rect rect, string title, string value, bool includeLabel = true)
    {
        BrowseButton(rect, title, EditorGUIUtility.IconContent("Folder Icon"), () => 
        {
            string selectedFolder = EditorUtility.OpenFolderPanel(title, value, "");
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                GUI.FocusControl("Button " + title);
                value = selectedFolder;
                Dirty = true;
            }
        }, ButtonWidth);

        rect = new Rect(rect.x, rect.y, rect.width - ButtonWidth, rect.height);

        BrowseButton(rect, title, EditorGUIUtility.IconContent("UpArrow"), () =>
        {
            if (Directory.Exists(value))
                Process.Start(value);
        }, ButtonWidth);

        rect = new Rect(rect.x, rect.y, rect.width - ButtonWidth, rect.height);

        value = !includeLabel ? EditorGUI.TextField(rect, value) : EditorGUI.TextField(rect, new GUIContent(title), value);

        return value;
    }

    protected string FileField(Rect rect, string title, string value, bool save, string extension, bool includeLabel = true)
    {
        BrowseButton(rect, title, EditorGUIUtility.IconContent("Folder Icon"), () => 
        {
            string directory = "";
            string defaultName = "";

            if (!string.IsNullOrEmpty(value))
            {
                directory = Path.GetFileName(Path.GetFullPath(value));
                defaultName = Path.GetFileName(value);
            }

            var file = save ? EditorUtility.SaveFilePanel(title, directory, defaultName, extension) : EditorUtility.OpenFilePanel(title, directory, extension);

            if (!string.IsNullOrEmpty(file))
            {
                GUI.FocusControl("Button " + title);
                value = file;
                Dirty = true;
            }
        }, ButtonWidth);

        rect = new Rect(rect.x, rect.y, rect.width - ButtonWidth, rect.height);

        value = !includeLabel ? EditorGUI.TextField(rect, value) : EditorGUI.TextField(rect, new GUIContent(title), value);
        return value;
    }

    /// <summary>
    /// Any changes made?
    /// </summary>
    protected bool Dirty { get; set; }

    protected bool scrollbarShown { get; set; }

    protected float scrollbarWidth { get; set; } = 16f;

    protected const int ButtonWidth = 24;

    protected void OnInspectorUpdate()
    {
        if (EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            Repaint();
        }
    }
}