using UnityEngine;

namespace R1Engine {
    public class Unity_ObjGroupBehaviour : MonoBehaviour {
        public int ObjGroup;
        public GameObject Content;

        void Update() {
            // Make sure the events have loaded
            if (!Controller.obj.levelEventController.hasLoaded)
                return;
            if (LevelEditorData.Level?.ObjectGroups != null && ObjGroup < LevelEditorData.Level.ObjectGroups.Length) {
                bool visible = LevelEditorData.SelectedObjectGroup == ObjGroup;
                if (visible != Content.activeSelf) {
                    Content.SetActive(visible);
                }
            }
        }
    }
}
