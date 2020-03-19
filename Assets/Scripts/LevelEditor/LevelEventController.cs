using UnityEngine;
using UnityEngine.UI;

namespace R1Engine
{
    public class LevelEventController : MonoBehaviour {
        // Prefabs
        public GameObject eventParent;
        public GameObject prefabEvent;

        public Editor editor;
        public Common_Event currentlySelected;

        // Event info things for the ui
        public GameObject eventInfoWindow;
        public Text eventInfoName;
        public InputField eventInfoX;
        public InputField eventInfoY;
        public InputField eventInfoDes;
        public InputField eventInfoEta;
        public InputField eventInfoEtat;
        public InputField eventInfoSubEtat;
        public InputField eventInfoOffsetBx;
        public InputField eventInfoOffsetBy;
        public InputField eventInfoOffsetHy;
        public InputField eventInfoFollowSprite;
        public InputField eventInfoHitPoints;
        public InputField eventInfoHitSprite;
        public InputField eventInfoFollow;
        public InputField eventInfoType;

        //public Dropdown eventDropdown;

        public bool areLinksVisible = false;

        public void InitializeEvents() {
            // Convert linkindex of each event to linkid
            var eventList = Controller.obj.levelController.currentLevel.Events;
            int currentId = 1;
            for (int i=0; i < eventList.Count; i++) {
                // No link
                if (eventList[i].LinkIndex == i) {
                    eventList[i].LinkID = 0;
                }
                else {
                    //Ignore already assigned ones
                    if (eventList[i].LinkID == 0) {
                        // Link found, loop through everyone on the link chain
                        int nextEvent = eventList[i].LinkIndex;
                        eventList[i].LinkID = currentId;
                        while (nextEvent != i) {
                            eventList[nextEvent].LinkID = currentId;
                            nextEvent = eventList[nextEvent].LinkIndex;
                        }
                        currentId++;
                    }
                }
            }

            // Fill the dropdown menu
            //var info = EventInfoManager.LoadEventInfo();
            //availableEvents = info.Where(x => x.Names.ContainsKey(Settings.World)).ToArray();

            //foreach (var e in availableEvents) {
            //    if (e.Names[Settings.World].CustomName!=null && e.Names[Settings.World].DesignerName != null) {
            //        Dropdown.OptionData dat = new Dropdown.OptionData();
            //        dat.text = e.Names[Settings.World].CustomName == null ? e.Names[Settings.World].CustomName : e.Names[Settings.World].DesignerName;
            //        eventDropdown.options.Add(dat);
            //    }
            //}

            //eventDropdown.value = 1;
            //eventDropdown.value = 0;
        }

        private void Update() {
            //Only do this if in event mode
            if (editor.currentMode == Editor.EditMode.Events) {
                //Detect event under mouse when clicked
                if (Input.GetMouseButtonDown(0)) {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    var e = hit.collider?.GetComponentInParent<Common_Event>();
                    if (e != null) {
                        currentlySelected = e;
                        //Change event info if event is selected
                        eventInfoWindow.SetActive(true);
                        eventInfoName.text = currentlySelected.name;
                        eventInfoX.text = currentlySelected.XPosition.ToString();
                        eventInfoY.text = currentlySelected.YPosition.ToString();
                        eventInfoDes.text = currentlySelected.DES.ToString();
                        eventInfoEta.text = currentlySelected.ETA.ToString();
                        eventInfoEtat.text = currentlySelected.Etat.ToString();
                        eventInfoSubEtat.text = currentlySelected.SubEtat.ToString();
                        eventInfoOffsetBx.text = currentlySelected.OffsetBX.ToString();
                        eventInfoOffsetBy.text = currentlySelected.OffsetBY.ToString();
                        eventInfoOffsetHy.text = currentlySelected.OffsetHY.ToString();
                        eventInfoFollowSprite.text = currentlySelected.FollowSprite.ToString();
                        eventInfoHitPoints.text = currentlySelected.HitPoints.ToString();
                        eventInfoHitSprite.text = currentlySelected.HitSprite.ToString();
                        eventInfoFollow.text = currentlySelected.FollowEnabled?"TRUE":"FALSE";
                        eventInfoType.text = currentlySelected.Type.ToString();
                    }
                    else {
                        currentlySelected = null;
                        eventInfoWindow.SetActive(false);
                    }
                }
            }
        }

        public void FieldValueChanged(int property, int newVal) {
            if (currentlySelected != null) {
                switch (property) {
                    //X position
                    case 0: currentlySelected.XPosition = (uint)newVal; break;
                    //Y position
                    case 1: currentlySelected.YPosition = (uint)newVal; break;
                }
            }
        }

        // Add event which matches the dropdown string
        public void AddSelectedEvent() {
            //foreach (var e in availableEvents) {
            //    if (e.Names[Settings.World].CustomName==eventDropdown.options[eventDropdown.value].text || e.Names[Settings.World].DesignerName == eventDropdown.options[eventDropdown.value].text) {
            //        AddEvent(e);
            //    }
            //}
        }

        // Show/Hide links
        public void ToggleLinks(bool t) {
            if (Controller.obj.levelController.currentLevel != null) {
                if (areLinksVisible != t) {
                    areLinksVisible = t;
                    foreach (var e in Controller.obj.levelController.currentLevel.Events) {
                        e.lineRend.enabled = t;
                    }
                }
            }
        }

        // Add events to the list via the managers
        public Common_Event AddEvent(int type, int etat, int subEtat, uint xpos, uint ypos, int des, int eta, int offsetBX, int offsetBY, int offsetHY, int followSprite, int hitpoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, byte[] commands, int link, int animSpeed) {
            // Instantiate prefab
            Common_Event newEvent = Instantiate(prefabEvent, new Vector3(xpos / 16f, -(ypos / 16f), 5f), Quaternion.identity).GetComponent<Common_Event>();

            newEvent.Type = type;
            newEvent.Etat = etat;
            newEvent.SubEtat = subEtat;

            newEvent.XPosition = xpos;
            newEvent.YPosition = ypos;
            
            newEvent.DES = des;
            newEvent.ETA = eta;
            
            newEvent.OffsetBX = offsetBX;
            newEvent.OffsetBY = offsetBY;
            newEvent.OffsetHY = offsetHY;
            
            newEvent.FollowSprite = followSprite;
            newEvent.HitPoints = hitpoints;
            newEvent.HitSprite = hitSprite;
            newEvent.FollowEnabled = followEnabled;
            
            newEvent.LabelOffsets = labelOffsets;
            newEvent.Commands = commands;

            newEvent.LinkIndex = link;
            newEvent.AnimSpeed = animSpeed;

            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;

            // Refresh
            newEvent.RefreshEditorInfo();

            // Add to list
            return newEvent;
        }
    }
}
