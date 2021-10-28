using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace R1Engine
{
    public class LegacyEditorUI_EventListItem : MonoBehaviour, IPointerDownHandler {
        public Unity_SpriteObjBehaviour ev;
        public Text evName;
        LegacyEditorUI_EventList list;
        Image bg;

        void Start() {
            list = GetComponentInParent<LegacyEditorUI_EventList>();
            bg = GetComponent<Image>();
            evName.text = ev.ObjData.Name;
        }

        public void OnPointerDown(PointerEventData eventData) {
            list.selection = ev;
        }

        void Update() {
            bg.enabled = list.selection == ev;
        }
    }
}