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
        public LineRenderer selectedLineRend;

        public Dropdown eventDropdown;

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
            var events = Settings.GetGameManager.GetEvents(Settings.GetGameSettings);
            
            foreach (var e in events) {
                Dropdown.OptionData dat = new Dropdown.OptionData
                {
                    text = e
                };
                eventDropdown.options.Add(dat);
            }
        }

        private void Update() {
            //Only do this if in event mode
            if (editor.currentMode == Editor.EditMode.Events) {
                selectedLineRend.enabled = true;
                //Add events if right clicked
                if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject()) {

                    Vector2 mousepo = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    var eve = Settings.GetGameManager.AddEvent(Settings.GetGameSettings, this, eventDropdown.value, (uint)mousepo.x * 16, (uint)-mousepo.y * 16);

                    Controller.obj.levelController.currentLevel.Events.Add(eve);
                }
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
                        selectedLineRend.enabled = false;
                        currentlySelected = null;
                        eventInfoWindow.SetActive(false);
                    }
                }
                //Drag and move the event
                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    if (currentlySelected != null) {
                        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        eventInfoX.text = Mathf.Clamp(Mathf.RoundToInt((mousePos.x-selectedPosition.x) * 16),0,Controller.obj.levelController.currentLevel.Width*16).ToString();
                        eventInfoY.text = Mathf.Clamp(Mathf.RoundToInt(-(mousePos.y-selectedPosition.y) * 16),0,Controller.obj.levelController.currentLevel.Height*16).ToString();
                    }
                }
                //Update event's values when the fields are modified
                if (currentlySelected != null) {
                    uint.TryParse(eventInfoX.text, out var new_x);
                    currentlySelected.XPosition = new_x;

                    uint.TryParse(eventInfoY.text, out var new_y);
                    currentlySelected.YPosition = new_y;

                    int.TryParse(eventInfoDes.text, out var new_des);
                    currentlySelected.DES = new_des;

                    int.TryParse(eventInfoEta.text, out var new_eta);
                    currentlySelected.ETA = new_eta;

                    int.TryParse(eventInfoEtat.text, out var new_etat);
                    currentlySelected.Etat = new_etat;

                    int.TryParse(eventInfoSubEtat.text, out var new_subetat);
                    currentlySelected.SubEtat = new_subetat;

                    int.TryParse(eventInfoOffsetBx.text, out var new_offbx);
                    currentlySelected.OffsetBX = new_offbx;

                    int.TryParse(eventInfoOffsetBy.text, out var new_offby);
                    currentlySelected.OffsetBY = new_offby;

                    int.TryParse(eventInfoOffsetHy.text, out var new_offhy);
                    currentlySelected.OffsetHY = new_offhy;

                    int.TryParse(eventInfoFollowSprite.text, out var new_fsprite);
                    currentlySelected.FollowSprite = new_fsprite;

                    int.TryParse(eventInfoHitPoints.text, out var new_hp);
                    currentlySelected.HitPoints = new_hp;

                    int.TryParse(eventInfoHitSprite.text, out var new_hsprite);
                    currentlySelected.HitSprite = new_hsprite;

                    currentlySelected.FollowEnabled = eventInfoFollow.text == "TRUE";

                    int.TryParse(eventInfoType.text, out var new_type);
                    currentlySelected.Type = new_type;
                }
                //Delete selected event
                if (Input.GetKeyDown(KeyCode.Delete)) {
                    if (currentlySelected != null) {
                        currentlySelected.Delete();

                        currentlySelected = null;
                        eventInfoWindow.SetActive(false);
                    }
                }
            }
            else {
                selectedLineRend.enabled = false;
            }
        }

        private void LateUpdate() {
            //Update link lines
            if (currentlySelected != null) {
                selectedLineRend.SetPosition(0, new Vector2(currentlySelected.midpoint.x - currentlySelected.boxCollider.size.x / 2f, currentlySelected.midpoint.y - currentlySelected.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(1, new Vector2(currentlySelected.midpoint.x + currentlySelected.boxCollider.size.x / 2f, currentlySelected.midpoint.y - currentlySelected.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(2, new Vector2(currentlySelected.midpoint.x + currentlySelected.boxCollider.size.x / 2f, currentlySelected.midpoint.y + currentlySelected.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(3, new Vector2(currentlySelected.midpoint.x - currentlySelected.boxCollider.size.x / 2f, currentlySelected.midpoint.y + currentlySelected.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(4, new Vector2(currentlySelected.midpoint.x - currentlySelected.boxCollider.size.x / 2f, currentlySelected.midpoint.y - currentlySelected.boxCollider.size.y / 2f));
            }
            else {
                selectedLineRend.SetPosition(0, Vector2.zero);
                selectedLineRend.SetPosition(1, Vector2.zero);
                selectedLineRend.SetPosition(2, Vector2.zero);
                selectedLineRend.SetPosition(3, Vector2.zero);
                selectedLineRend.SetPosition(4, Vector2.zero);
            }
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
