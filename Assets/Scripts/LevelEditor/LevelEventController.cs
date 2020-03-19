using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace R1Engine
{
    public class LevelEventController : MonoBehaviour {
        // Prefabs
        public GameObject eventParent;
        public GameObject prefabEvent;

        public Editor editor;

        public Common_Event currentlySelected;
        public Vector2 selectedPosition;

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
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
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
                        //Recor selected position
                        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        selectedPosition = new Vector2(mousePos.x - e.transform.position.x, mousePos.y - e.transform.position.y);
                    }
                    else {
                        currentlySelected = null;
                        eventInfoWindow.SetActive(false);
                    }
                }
                //Drag and move the event
                if (Input.GetMouseButton(0)) {
                    if (currentlySelected != null) {
                        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        eventInfoX.text = Mathf.Clamp(Mathf.RoundToInt((mousePos.x-selectedPosition.x) * 16),0,Controller.obj.levelController.currentLevel.Width*16).ToString();
                        eventInfoY.text = Mathf.Clamp(Mathf.RoundToInt(-(mousePos.y-selectedPosition.y) * 16),0,Controller.obj.levelController.currentLevel.Height*16).ToString();
                    }
                }
                //Update event's values when the fields are modified
                if (currentlySelected != null) {
                    uint new_x = 0;
                    uint.TryParse(eventInfoX.text, out new_x);
                    currentlySelected.XPosition = new_x;

                    uint new_y = 0;
                    uint.TryParse(eventInfoY.text, out new_y);
                    currentlySelected.YPosition = new_y;

                    int new_des = 0;
                    int.TryParse(eventInfoDes.text, out new_des);
                    currentlySelected.DES = new_des;

                    int new_eta = 0;
                    int.TryParse(eventInfoEta.text, out new_eta);
                    currentlySelected.ETA = new_eta;

                    int new_etat = 0;
                    int.TryParse(eventInfoEtat.text, out new_etat);
                    currentlySelected.Etat = new_etat;

                    int new_subetat = 0;
                    int.TryParse(eventInfoSubEtat.text, out new_subetat);
                    currentlySelected.SubEtat = new_subetat;

                    int new_offbx = 0;
                    int.TryParse(eventInfoOffsetBx.text, out new_offbx);
                    currentlySelected.OffsetBX = new_offbx;

                    int new_offby = 0;
                    int.TryParse(eventInfoOffsetBy.text, out new_offby);
                    currentlySelected.OffsetBY = new_offby;

                    int new_offhy = 0;
                    int.TryParse(eventInfoOffsetHy.text, out new_offhy);
                    currentlySelected.OffsetHY = new_offhy;

                    int new_fsprite = 0;
                    int.TryParse(eventInfoFollowSprite.text, out new_fsprite);
                    currentlySelected.FollowSprite = new_fsprite;

                    int new_hp = 0;
                    int.TryParse(eventInfoHitPoints.text, out new_hp);
                    currentlySelected.HitPoints = new_hp;

                    int new_hsprite = 0;
                    int.TryParse(eventInfoHitSprite.text, out new_hsprite);
                    currentlySelected.HitSprite = new_hsprite;

                    currentlySelected.FollowEnabled = eventInfoFollow.text=="TRUE"?true:false;

                    int new_type = 0;
                    int.TryParse(eventInfoType.text, out new_type);
                    currentlySelected.Type = new_type;
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
