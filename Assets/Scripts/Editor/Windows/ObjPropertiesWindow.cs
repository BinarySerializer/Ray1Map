using System;
using Cysharp.Threading.Tasks;
using R1Engine;
using UnityEditor;
using UnityEngine;

public class ObjPropertiesWindow : UnityWindow
{
    [MenuItem("Ray1Map/Object Properties")]
    public static void ShowWindow()
	{
		GetWindow<ObjPropertiesWindow>(false, "Object Properties", true);
	}

	private void OnEnable()
	{
		titleContent = EditorGUIUtility.IconContent("SceneViewTools");
		titleContent.text = "Object Properties";
    }

    protected UnityWindowSerializer Serializer { get; set; }

    protected override UniTask UpdateEditorFieldsAsync() 
    {
        if (TotalyPos == 0f)
            TotalyPos = position.height;

        scrollbarShown = TotalyPos > position.height;
        ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

        if (Application.isPlaying && Controller.LoadState == Controller.State.Finished)
        {
            if (Serializer == null)
                Serializer = new UnityWindowSerializer(LevelEditorData.MainContext, this);

            var selectedObj = Controller.obj.levelController.controllerEvents.SelectedEvent;

            if (selectedObj != null)
            {
                DrawHeader("Editor fields");

                selectedObj.ObjData.CurrentUIState = EditorField("State", selectedObj.ObjData.CurrentUIState, selectedObj.ObjData.UIStateNames);

                // TODO: Add DES/ETA drop-down, add state drop-down, show cmds as string

                DrawHeader("Object data");

                var selectedObjData = selectedObj.ObjData;

                try
                {
                    if (selectedObjData is Unity_Object_R1 r1)
                        r1.EventData.SerializeImpl(Serializer);
                    else if (selectedObjData is Unity_Object_R2 r2)
                        r2.EventData.SerializeImpl(Serializer);
                    else if (selectedObjData is Unity_Object_R1Jaguar jag)
                        jag.Instance.SerializeImpl(Serializer);
                    else if (selectedObjData is Unity_Object_GBA gba)
                        gba.Actor.SerializeImpl(Serializer);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{ex.Message}{Environment.NewLine}{ex}");
                }
            }
            else
            {
                EditorGUI.HelpBox(GetNextRect(ref YPos, height: 40f), "No object selected", MessageType.Info);
            }
        }
        else
        {
            EditorGUI.HelpBox(GetNextRect(ref YPos, height: 40f), "Run the project in order to edit object properties.", MessageType.Warning);
        }

        TotalyPos = YPos;
        GUI.EndScrollView();

        return UniTask.CompletedTask;
    }

    #region Private Properties

    // Properties
    private float TotalyPos { get; set; }
    private Vector2 ScrollPosition { get; set; } = Vector2.zero;

    #endregion
}