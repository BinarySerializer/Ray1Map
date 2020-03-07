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
            // Fill the dropdown menu
            var info = EventInfoManager.LoadEventInfo();
            availableEvents = info.Where(x => x.Names.ContainsKey(Settings.World)).ToArray();

            foreach (var e in availableEvents) {
                if (e.Names[Settings.World].CustomName!=null && e.Names[Settings.World].DesignerName != null) {
                    Dropdown.OptionData dat = new Dropdown.OptionData();
                    dat.text = e.Names[Settings.World].CustomName == null ? e.Names[Settings.World].CustomName : e.Names[Settings.World].DesignerName;
                    eventDropdown.options.Add(dat);
                }
            }

            eventDropdown.value = 1;
            eventDropdown.value = 0;
        }

        // Add event which matches the dropdown string
        public void AddSelectedEvent() {
            foreach (var e in availableEvents) {
                if (e.Names[Settings.World].CustomName==eventDropdown.options[eventDropdown.value].text || e.Names[Settings.World].DesignerName == eventDropdown.options[eventDropdown.value].text) {
                    AddEvent(e);
                }
            }
        }

        // Add events to the list via the managers
        public Common_Event AddEvent(EventInfoData e, uint xpos, uint ypos, int offbx, int offby, int link, uint des, uint eta) {
            // Instantiate prefab
            Common_Event newEvent = Instantiate(prefabEvent, new Vector3(xpos / 16f, -(ypos / 16f), 5f), Quaternion.identity).GetComponent<Common_Event>();
            newEvent.EventInfoData = e;
            newEvent.XPosition = xpos;
            newEvent.YPosition = ypos;
            newEvent.OffsetBX = offbx;
            newEvent.OffsetBY = offby;
            newEvent.LinkIndex = link;
            newEvent.Des = des;
            newEvent.Eta = eta;

            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;
            // Add to list
            return newEvent;
        }

        // Add events from the list with eventinfo
        public void AddEvent(EventInfoData e) {
            // Instantiate prefab
            Common_Event newEvent = Instantiate(prefabEvent, new Vector3(0, 0, 5f), Quaternion.identity).GetComponent<Common_Event>();
            newEvent.EventInfoData = e;
            newEvent.XPosition = 0;
            newEvent.YPosition = 0;
            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;
            // Add to list
            Controller.obj.levelController.currentLevel.Events.Add(newEvent);
            // Set link to be the index
            newEvent.LinkIndex = Controller.obj.levelController.currentLevel.Events.IndexOf(newEvent);
        }
    }
}
