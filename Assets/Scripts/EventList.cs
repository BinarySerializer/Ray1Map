using UnityEngine;
using UnityEngine.UI;

namespace R1Engine
{
    public class EventList : MonoBehaviour {
        public RectTransform list;
        public InputField search;
        public Common_Event selection;

        static GameObject listItemRes;
        LevelMainController lvlCtrl;
        bool loaded;

        void Awake() {
            listItemRes = Resources.Load<GameObject>("UI/EventListItem");
            lvlCtrl = FindObjectOfType<LevelMainController>();
        }

        // Update is called once per frame
        void Update() {
            if (!loaded && lvlCtrl.currentLevel != null) {
                loaded = true;
                foreach (var e in FindObjectOfType<LevelMainController>().currentLevel.Events) {
                    Instantiate<GameObject>(listItemRes, list).GetComponent<EventListItem>().ev = e;
                }
            }
        }
    }
}