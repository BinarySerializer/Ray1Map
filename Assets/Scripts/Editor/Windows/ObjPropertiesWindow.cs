using System;
using System.Linq;
using Ray1Map;
using Ray1Map.GBAVV;
using Ray1Map.PSKlonoa;
using Ray1Map.Rayman1;
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

    public bool CmdPanelOpen { get; set; }
    public bool LocPanelOpen { get; set; }

    protected override void UpdateEditorFields() 
    {
        if (TotalyPos == 0f)
            TotalyPos = position.height;

        scrollbarShown = TotalyPos > position.height;
        ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

        if (Application.isPlaying && Controller.LoadState == Controller.State.Finished)
        {
            if (Serializer == null)
                Serializer = new UnityWindowSerializer(LevelEditorData.MainContext, this, null);

            var selectedObj = Controller.obj.levelController.controllerEvents.SelectedEvent;

            if (selectedObj != null)
            {
                DrawHeader("Editor fields");

                if (selectedObj.ObjData.LegacyWrapper != null)
                {
                    var des = EditorField("DES", selectedObj.ObjData.LegacyWrapper.DES, LevelEditorData.ObjManager.LegacyDESNames);

                    if (des != selectedObj.ObjData.LegacyWrapper.DES)
                        selectedObj.ObjData.LegacyWrapper.DES = des;
                }

                selectedObj.ObjData.CurrentUIState = EditorField("State", selectedObj.ObjData.CurrentUIState, selectedObj.ObjData.UIStateNames);

                selectedObj.IsEnabled = EditorField("Is Enabled", selectedObj.IsEnabled);

                DrawHeader("Object data");

                var selectedObjData = selectedObj.ObjData;

                try
                {
                    LocPanelOpen = EditorGUI.Foldout(GetNextRect(ref YPos), LocPanelOpen, "Localization");

                    if (LocPanelOpen)
                    {
                        foreach (var l in selectedObjData.GetLocIndices)
                            EditorGUI.LabelField(GetNextRect(ref YPos), $"{l:000}: {LevelEditorData.Level.Localization?.FirstOrDefault().Value?.ElementAtOrDefault(l)}");
                    }

                    if (selectedObjData is Unity_Object_R1 r1)
                    {
                        CmdPanelOpen = EditorGUI.Foldout(GetNextRect(ref YPos), CmdPanelOpen, "Commands");

                        if (CmdPanelOpen)
                        {
                            // TODO: Cache the commands, but make sure to update if modified
                            var cmdLines = r1.EventData.Commands?.ToTranslatedStrings(r1.EventData.LabelOffsets);

                            if (cmdLines?.Any() == true)
                                // TODO: Better way to get height?
                                EditorGUI.TextArea(GetNextRect(ref YPos, height: cmdLines.Length * 15 + 2), String.Join(Environment.NewLine, cmdLines));
                        }
                    }
                    if (selectedObjData is Unity_Object_GBAVV vv && vv.Script != null)
                    {
                        CmdPanelOpen = EditorGUI.Foldout(GetNextRect(ref YPos), CmdPanelOpen, "Script");

                        if (CmdPanelOpen)
                        {
                            // TODO: Cache the commands, but make sure to update if modified
                            var lines = vv.GetTranslatedScript;

                            // TODO: Better way to get height?
                            EditorGUI.TextArea(GetNextRect(ref YPos, height: lines.Length * 15 + 2), String.Join(Environment.NewLine, lines));
                        }
                    }
                    if (selectedObjData is Unity_Object_PSKlonoa_DTP_CutsceneSprite dtpCut && dtpCut.CutsceneScript != null)
                    {
                        CmdPanelOpen = EditorGUI.Foldout(GetNextRect(ref YPos), CmdPanelOpen, "Cutscene Script");

                        if (CmdPanelOpen)
                        {
                            // TODO: Cache the commands, but make sure to update if modified
                            var lines = dtpCut.CutsceneScript.FormattedScript;

                            // TODO: Better way to get height?
                            EditorGUI.TextArea(GetNextRect(ref YPos, height: lines.Length * 15 + 2), String.Join(Environment.NewLine, lines));
                        }
                    }

                    EditorGUI.BeginChangeCheck();

                    DrawHeader("Object");

                    selectedObj?.ObjData?.SerializableData?.SerializeImpl(Serializer);

                    if (selectedObj?.ObjData?.AdditionalSerializableDatas != null)
                    {
                        foreach (var data in selectedObj.ObjData.AdditionalSerializableDatas)
                        {
                            DrawHeader($"Additional Object ({data.GetType().Name})");

                            data.SerializeImpl(Serializer);
                        }
                    }

                    if (EditorGUI.EndChangeCheck())
                        selectedObjData.HasPendingEdits = true;
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
    }

    #region Private Properties

    // Properties
    private float TotalyPos { get; set; }
    private Vector2 ScrollPosition { get; set; } = Vector2.zero;

    #endregion
}