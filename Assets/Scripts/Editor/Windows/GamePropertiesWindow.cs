using Ray1Map;
using Ray1Map.Rayman1;
using UnityEditor;
using UnityEngine;

public class GamePropertiesWindow : UnityWindow
{
    [MenuItem("Ray1Map/Game Properties")]
    public static void ShowWindow()
	{
		GetWindow<GamePropertiesWindow>(false, "Game Properties", true);
	}

	private void OnEnable()
	{
		titleContent = EditorGUIUtility.IconContent("CustomTool");
		titleContent.text = "Game Properties";
    }

    protected UnityWindowSerializer Serializer { get; set; }

    protected override void UpdateEditorFields() 
    {
        if (TotalyPos == 0f)
            TotalyPos = position.height;

        scrollbarShown = TotalyPos > position.height;
        ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

        if (Application.isPlaying && Controller.LoadState == Controller.State.Finished)
        {
            if (Settings.LoadFromMemory)
            {
                var objManager = LevelEditorData.ObjManager;

                if (objManager is Unity_ObjectManager_R1 r1)
                {
                    if (Serializer == null)
                        Serializer = new UnityWindowSerializer(LevelEditorData.MainContext, this, r1.GlobalDataForceWrite);

                    EditorGUI.BeginChangeCheck();

                    r1.GlobalData.Update(Serializer);

                    if (EditorGUI.EndChangeCheck())
                        r1.GlobalPendingEdits = true;
                }
            }
            else
            {
                EditorGUI.HelpBox(GetNextRect(ref YPos, height: 40f), "Game properties are only available when loading from memory.", MessageType.Warning);
            }
        }
        else
        {
            EditorGUI.HelpBox(GetNextRect(ref YPos, height: 40f), "Run the project in order to edit game properties.", MessageType.Warning);
        }

        TotalyPos = YPos;
        GUI.EndScrollView();
    }

    #region Private Properties

    // Properties
    private float TotalyPos { get; set; }
    private Vector2 ScrollPosition { get; set; } = Vector2.zero;

    #endregion
}