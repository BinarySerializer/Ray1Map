using UnityEditor;
using UnityEngine;

public class SettingsWindow : EditorWindow {
    [MenuItem("Ray1Map/Settings")]
    static void Init() {
        ((SettingsWindow)GetWindow(typeof(SettingsWindow))).Show();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
