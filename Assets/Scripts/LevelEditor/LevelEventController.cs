using System;
using System.Collections.Generic;
using System.Linq;
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

        public Color linkColorActive;
        public Color linkColorDeactive;

        //Keeping track of used linkIds
        public int currentId = 1;

        public void InitializeEvents() {
            // Convert linkIndex of each event to linkId
            var eventList = Controller.obj.levelController.currentLevel.Events;
            for (int i=0; i < eventList.Count; i++) 
            {
                // Refresh
                eventList[i].RefreshEditorInfo();

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
                        eventList[i].linkCubeLockPosition = eventList[i].linkCube.position;
                        while (nextEvent != i) {
                            eventList[nextEvent].LinkID = currentId;

                            //Stack the link cubes
                            eventList[nextEvent].linkCube.position = eventList[i].linkCube.position;
                            eventList[nextEvent].linkCubeLockPosition = eventList[nextEvent].linkCube.position;

                            nextEvent = eventList[nextEvent].LinkIndex;
                        }
                        currentId++;
                    }
                }
            }

            // Fill the dropdown menu
            var events = Controller.obj.levelController.EditorManager.GetEvents();
            
            foreach (var e in events) {
                Dropdown.OptionData dat = new Dropdown.OptionData
                {
                    text = e
                };
                eventDropdown.options.Add(dat);
            }

            // TODO: Have some flag for if current game mode supports editing
            if (eventDropdown.options.Any<Dropdown.OptionData>())
                // Default to the first string
                eventDropdown.captionText.text = eventDropdown.options[0].text;
        }

        private void Start() {
            //Assign visibility refresh for the settings booleans
            Settings.OnShowAlwaysEventsChanged += ChangeEventsVisibility;
            Settings.OnShowEditorEventsChanged += ChangeEventsVisibility;
        }

        public void ChangeEventsVisibility(object o, EventArgs e) {
            foreach(var eve in Controller.obj.levelController.currentLevel.Events) {
                eve.RefreshVisuals();
                if (editor.currentMode == Editor.EditMode.Links)
                    eve.ChangeLinksVisibility(true);
            }
        }

        private void Update() {
            //Only do this if in event/link mode
            bool modeEvents = editor.currentMode == Editor.EditMode.Events;
            bool modeLinks = editor.currentMode == Editor.EditMode.Links;

            if ( modeEvents || modeLinks ) {
                selectedLineRend.enabled = true;
                //Add events with mmb
                if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject() && modeEvents) {
                    Vector2 mousepo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var mox = mousepo.x * 16;
                    var moy = mousepo.y * 16;
                    //Don't add if clicked outside of the level bounds
                    if (mox > 0 && -moy > 0 && mox < Controller.obj.levelController.currentLevel.Width*16 && -moy < Controller.obj.levelController.currentLevel.Height*16) {

                        var eve = Controller.obj.levelController.EditorManager.AddEvent(this, eventDropdown.value, (uint)mox, (uint)-moy);

                        // Refresh the event
                        eve.RefreshEditorInfo();

                        Controller.obj.levelController.currentLevel.Events.Add(eve);
                    }
                }
                //Detect event under mouse when clicked
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    var e = hit.collider?.GetComponentInParent<Common_Event>();
                    if (e != null) {
                        if (currentlySelected != null)
                            currentlySelected.ChangeOffsetVisibility(false);
                        currentlySelected = e;
                        //Change event info if event is selected
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
                        //Record selected position
                        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        selectedPosition = new Vector2(mousePos.x - e.transform.position.x, mousePos.y - e.transform.position.y);
                        //Update offset visibility
                        currentlySelected.ChangeOffsetVisibility(true);
                        //Change the link
                        if (modeLinks) {
                            currentlySelected.LinkID = 0;
                            currentlySelected.ChangeLinksVisibility(true);
                        }
                    }
                    else {
                        if (currentlySelected!=null)
                            currentlySelected.ChangeOffsetVisibility(false);
                        selectedLineRend.enabled = false;
                        currentlySelected = null;
                        //Clear info window
                        ClearInfoWindow();
                    }
                }
                //Drag and move the event
                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    if (currentlySelected != null) {
                        //Move event if in event mode
                        if (modeEvents) {
                            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                            eventInfoX.text = Mathf.Clamp(Mathf.RoundToInt((mousePos.x - selectedPosition.x) * 16), 0, Controller.obj.levelController.currentLevel.Width * 16).ToString();
                            eventInfoY.text = Mathf.Clamp(Mathf.RoundToInt(-(mousePos.y - selectedPosition.y) * 16), 0, Controller.obj.levelController.currentLevel.Height * 16).ToString();

                            uint.TryParse(eventInfoX.text, out var new_x);
                            currentlySelected.XPosition = new_x;
                            uint.TryParse(eventInfoY.text, out var new_y);
                            currentlySelected.YPosition = new_y;

                            currentlySelected.UpdateXAndY();
                        }
                        //Else move links
                        if (modeLinks) {
                            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            currentlySelected.linkCube.position = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
                        }
                    }
                }
                //Confirm links with mmb
                if (Input.GetMouseButtonDown(2) && modeLinks && currentlySelected.LinkID==0) {
                    bool alone = true;
                    foreach (var ee in Controller.obj.levelController.currentLevel.Events) {
                        if (ee.linkCube.position==currentlySelected.linkCube.position) {
                            if (ee != currentlySelected) {
                                ee.LinkID = currentId;
                                ee.ChangeLinksVisibility(true);
                                ee.linkCubeLockPosition = ee.linkCube.position;
                                alone = false;
                            }
                        }
                    }
                    if (!alone) {
                        currentlySelected.LinkID = currentId;
                        currentlySelected.ChangeLinksVisibility(true);
                        currentlySelected.linkCubeLockPosition = currentlySelected.linkCube.position;
                    }
                    currentId++;
                }
                //Delete selected event
                if (Input.GetKeyDown(KeyCode.Delete) && modeEvents) {
                    if (currentlySelected != null) {
                        currentlySelected.ChangeOffsetVisibility(false);
                        currentlySelected.Delete();
                        currentlySelected = null;

                        ClearInfoWindow();
                    }
                }
            }
            else {
                selectedLineRend.enabled = false;
            }
        }

        private void LateUpdate() {
            //Update selection square lines
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

        //-----OnChanged methods for each field-----

        public void FieldXPosition() {
            if (currentlySelected != null) {
                uint.TryParse(eventInfoX.text, out var new_x);
                currentlySelected.XPosition = new_x;
                currentlySelected.UpdateXAndY();
            }
        }
        public void FieldYPosition() {
            if (currentlySelected != null) {
                uint.TryParse(eventInfoY.text, out var new_y);
                currentlySelected.YPosition = new_y;
                currentlySelected.UpdateXAndY();
            }
        }
        public void FieldDes() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoDes.text, out var new_des);
                currentlySelected.DES = new_des;

                currentlySelected.RefreshName();
                currentlySelected.RefreshVisuals();
            }
        }
        public void FieldEta() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoEta.text, out var new_eta);
                currentlySelected.ETA = new_eta;

                currentlySelected.RefreshName();
                currentlySelected.RefreshVisuals();
            }
        }
        public void FieldEtat() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoEtat.text, out var new_etat);
                currentlySelected.Etat = new_etat;

                currentlySelected.RefreshName();
                currentlySelected.RefreshVisuals();
            }
        }
        public void FieldSubEtat() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoSubEtat.text, out var new_subetat);
                currentlySelected.SubEtat = new_subetat;

                currentlySelected.RefreshName();
                currentlySelected.RefreshVisuals();
            }
        }
        public void FieldOffsetBx() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoOffsetBx.text, out var new_offbx);
                currentlySelected.OffsetBX = new_offbx;

                currentlySelected.RefreshName();
                currentlySelected.UpdateOffsetPoints();
            }
        }
        public void FieldOffsetBy() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoOffsetBy.text, out var new_offby);
                currentlySelected.OffsetBY = new_offby;

                currentlySelected.RefreshName();
                currentlySelected.UpdateOffsetPoints();
            }
        }
        public void FieldOffsetHy() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoOffsetHy.text, out var new_offhy);
                currentlySelected.OffsetHY = new_offhy;

                currentlySelected.RefreshName();
                currentlySelected.UpdateOffsetPoints();
            }
        }
        public void FieldFollowSprite() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoFollowSprite.text, out var new_fsprite);
                currentlySelected.FollowSprite = new_fsprite;

                currentlySelected.RefreshName();
            }
        }
        public void FieldHitPoints() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoHitPoints.text, out var new_hp);
                currentlySelected.HitPoints = new_hp;

                currentlySelected.RefreshName();
            }
        }
        public void FieldHitSprite() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoHitSprite.text, out var new_hsprite);
                currentlySelected.HitSprite = new_hsprite;

                currentlySelected.RefreshName();
            }
        }
        public void FieldFollowEnabled() {
            if (currentlySelected != null) {
                currentlySelected.FollowEnabled = eventInfoFollow.text == "TRUE";

                currentlySelected.RefreshName();
            }
        }
        public void FieldType() {
            if (currentlySelected != null) {
                int.TryParse(eventInfoType.text, out var new_type);
                currentlySelected.Type = (EventType)new_type;

                currentlySelected.RefreshName();
            }
        }

        //--------------------

        private void ClearInfoWindow() {
            eventInfoName.text = "";
            eventInfoX.text = "";
            eventInfoY.text = "";
            eventInfoDes.text = "";
            eventInfoEta.text = "";
            eventInfoEtat.text = "";
            eventInfoSubEtat.text = "";
            eventInfoOffsetBx.text = "";
            eventInfoOffsetBy.text = "";
            eventInfoOffsetHy.text = "";
            eventInfoFollowSprite.text = "";
            eventInfoHitPoints.text = "";
            eventInfoHitSprite.text = "";
            eventInfoFollow.text = "";
            eventInfoType.text = "";
        }

        // Show/Hide links
        public void ToggleLinks(bool t) {
            if (Controller.obj.levelController.currentLevel != null) {
                if (areLinksVisible != t) {
                    areLinksVisible = t;
                    foreach (var e in Controller.obj.levelController.currentLevel.Events) {
                        e.ChangeLinksVisibility(t);
                    }
                }
            }
        }

        // Converts linkID to linkIndex when saving
        public void CalculateLinkIndexes() {

            List<int> alreadyChained = new List<int>();
            foreach (Common_Event ee in Controller.obj.levelController.currentLevel.Events) {
                // No link
                if (ee.LinkID == 0) {
                    ee.LinkIndex = Controller.obj.levelController.currentLevel.Events.IndexOf(ee);
                }
                else {
                    // Skip if already chained
                    if (alreadyChained.Contains(Controller.obj.levelController.currentLevel.Events.IndexOf(ee)))
                        continue;

                    // Find all the events with the same linkId and store their indexes
                    List<int> indexesOfSameId = new List<int>();
                    int cur = ee.LinkID;
                    foreach (Common_Event e in Controller.obj.levelController.currentLevel.Events.Where<Common_Event>(e => e.LinkID == cur)) {
                        indexesOfSameId.Add(Controller.obj.levelController.currentLevel.Events.IndexOf(e));
                        alreadyChained.Add(Controller.obj.levelController.currentLevel.Events.IndexOf(e));
                    }
                    // Loop through and chain them
                    for (int j = 0; j < indexesOfSameId.Count; j++) {
                        int next = j + 1;
                        if (next == indexesOfSameId.Count)
                            next = 0;

                        Controller.obj.levelController.currentLevel.Events[indexesOfSameId[j]].LinkIndex = indexesOfSameId[next];
                    }
                }
            }
        }

        // Add events to the list via the managers
        public Common_Event AddEvent(EventType type, int etat, int subEtat, uint xpos, uint ypos, int des, int eta, int offsetBX, int offsetBY, int offsetHY, int followSprite, int hitpoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, Common_EventCommandCollection commands, int link) {
            // Instantiate prefab
            Common_Event newEvent = Instantiate<GameObject>(prefabEvent, new Vector3(xpos / 16f, -(ypos / 16f), 5f), Quaternion.identity).GetComponent<Common_Event>();

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
            newEvent.CommandCollection = commands;

            newEvent.LinkIndex = link;

            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;

            // Add to list
            return newEvent;
        }
    }
}
