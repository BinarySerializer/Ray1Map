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

    protected override async UniTask UpdateEditorFieldsAsync() 
    {
        if (TotalyPos == 0f)
            TotalyPos = position.height;

        scrollbarShown = TotalyPos > position.height;
        ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalyPos));

        if (Controller.LoadState == Controller.State.Finished)
        {
            var selectedObj = Controller.obj.levelController.controllerEvents.SelectedEvent?.ObjData;

            if (selectedObj is Unity_Object_R2 r2)
            {
                r2.EventData.IsFlippedHorizontally = EditorField("Is flipped", r2.EventData.IsFlippedHorizontally);

                if (r2.EventData.CollisionData != null)
                {
                    r2.EventData.CollisionData.ZDC.ZDCIndex = (byte)EditorField("ZDC index", r2.EventData.CollisionData.ZDC.ZDCIndex);
                    r2.EventData.CollisionData.ZDC.ZDCCount = (byte)EditorField("ZDC count", r2.EventData.CollisionData.ZDC.ZDCCount);
                }
            }
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