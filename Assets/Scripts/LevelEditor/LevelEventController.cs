using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace R1Engine {
    public class LevelEventController : MonoBehaviour {

        public GameObject eventParent;
        public GameObject prefabEvent;

        public Dropdown eventDropdown;
        public EventInfoData[] availableEvents;

        public void InitializeEvents() {
            // Make a copy of the original events
            List<Common_Event> prefabList = new List<Common_Event>();

            // Instantiate event prefabs under the parent class
            foreach (var eve in Controller.obj.levelController.currentLevel.Events) {
                // Instantiate prefab
                Common_Event newEvent = Instantiate(prefabEvent, new Vector3(eve.XPosition/16f, -(eve.YPosition/16f), 5f), Quaternion.identity).GetComponent<Common_Event>();
                newEvent.EventInfoData = eve.EventInfoData;
                newEvent.XPosition = eve.XPosition;
                newEvent.YPosition = eve.YPosition;
                newEvent.LinkIndex = eve.LinkIndex;
                // Offset the child sprite a bit offsetX and offsetY
                newEvent.transform.GetChild(0).transform.localPosition = new Vector3(eve.OffsetBX / 16f, -(eve.OffsetBY / 16f), 5f);
                // Set as child of events gameobject
                newEvent.gameObject.transform.parent = eventParent.transform;
                // Add to list
                prefabList.Add(newEvent);
            }

            // Replace the original list with the prefab list
            Controller.obj.levelController.currentLevel.Events.Clear();
            Controller.obj.levelController.currentLevel.Events = prefabList;

            // Fill the dropdown menu
            var info = EventInfoManager.LoadEventInfo();
            availableEvents = info.Where(x => x.Worlds.Contains(Settings.World)).ToArray();

            foreach (var e in availableEvents) {
                if (e.CustomName!=null && e.DesignerName != null) {
                    Dropdown.OptionData dat = new Dropdown.OptionData();
                    dat.text = e.CustomName == null ? e.CustomName : e.DesignerName;
                    eventDropdown.options.Add(dat);
                }
            }

            eventDropdown.value = 1;
            eventDropdown.value = 0;
        }

        // Add event which matches the dropdown string
        public void AddSelectedEvent() {
            foreach (var e in availableEvents) {
                if (e.CustomName==eventDropdown.options[eventDropdown.value].text || e.DesignerName == eventDropdown.options[eventDropdown.value].text) {
                    AddEvent(e);
                }
            }
        }

        public void AddEvent(EventInfoData e) {
            // Instantiate prefab
            Common_Event newEvent = Instantiate(prefabEvent, new Vector3(0, 0, 5f), Quaternion.identity).GetComponent<Common_Event>();
            newEvent.EventInfoData = e;
            newEvent.XPosition = 0;
            newEvent.YPosition = 0;
            newEvent.LinkIndex = Controller.obj.levelController.currentLevel.Events.Max(t => t.LinkIndex) + 1;
            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;
            // Add to list
            Controller.obj.levelController.currentLevel.Events.Add(newEvent);
        }
    }
}
