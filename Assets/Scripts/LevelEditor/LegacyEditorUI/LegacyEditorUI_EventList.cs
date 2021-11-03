using UnityEngine;
using UnityEngine.UI;

namespace Ray1Map
{
    public class LegacyEditorUI_EventList : MonoBehaviour {
        public RectTransform list;
        public InputField search;
        public Unity_SpriteObjBehaviour selection;

        bool loaded;

        // Update is called once per frame
        void Update() {
            if (!loaded && LevelEditorData.Level != null && Controller.LoadState == Controller.State.Finished) {
                loaded = true;
                foreach (var e in FindObjectOfType<LevelMainController>().Objects) {
                    Instantiate<GameObject>(Controller.obj?.Prefabs.LegacyEditorUI_EventListItem, list).GetComponent<LegacyEditorUI_EventListItem>().ev = e;
                }
            }
        }
    }
}