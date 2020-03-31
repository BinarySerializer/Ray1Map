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
        public GameObject infoWindow;
        public Text infoName;
        public InputField infoX;
        public InputField infoY;
        public Dropdown infoDes;
        public Dropdown infoEta;
        public Dropdown infoEtat;
        public Dropdown infoSubEtat;
        public InputField infoOffsetBx;
        public InputField infoOffsetBy;
        public InputField infoOffsetHy;
        public InputField infoFollowSprite;
        public InputField infoHitPoints;
        public InputField infoHitSprite;
        public InputField infoAnimIndex;
        public InputField infoLayer;
        public Toggle infoFollow;
        public Dropdown infoType;

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
                eventList[i].RefreshFlag();
                eventList[i].RefreshEditorInfo();

                // If X and Y are insane, clamp them
                var border = 10;
                eventList[i].XPosition = (uint)Mathf.Clamp(eventList[i].XPosition, -border, (Controller.obj.levelController.currentLevel.Width*16)+border);
                eventList[i].YPosition = (uint)Mathf.Clamp(eventList[i].YPosition, -border, (Controller.obj.levelController.currentLevel.Height*16)+border);
                eventList[i].UpdateXAndY();

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

            //Fill Des and Eta dropdowns with their max values
            for (int i = 0; i < Controller.obj.levelController.EditorManager.GetMaxDES; i++) {
                Dropdown.OptionData dat = new Dropdown.OptionData {
                    text = i.ToString()
                };
                infoDes.options.Add(dat);
            }
            for (int i = 0; i < Controller.obj.levelController.EditorManager.GetMaxETA; i++) {
                Dropdown.OptionData dat = new Dropdown.OptionData {
                    text = i.ToString()
                };
                infoEta.options.Add(dat);
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
            //Fill eventinfo dropdown with the event types
            var all = Enum.GetValues(typeof(EventType));
            foreach(var e in all) {
                Dropdown.OptionData dat = new Dropdown.OptionData {
                    text = e.ToString()
                };
                infoType.options.Add(dat);
            }
        }

        public void ChangeEventsVisibility(object o, EventArgs e) {
            if (Controller.obj.levelController.currentLevel != null) {
                foreach (var eve in Controller.obj.levelController.currentLevel.Events) {
                    eve.RefreshVisuals();
                    if (editor.currentMode == Editor.EditMode.Links)
                        eve.ChangeLinksVisibility(true);
                }
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
                        if (e != currentlySelected) {
                            currentlySelected = e;
                            //Change event info if event is selected
                            infoName.text = currentlySelected.name;
                            infoX.text = currentlySelected.XPosition.ToString();
                            infoY.text = currentlySelected.YPosition.ToString();
                            infoDes.value = currentlySelected.DES;
                            infoEta.value = currentlySelected.ETA;
                            UpdateInfoEtat();
                            infoEtat.value = currentlySelected.Etat;
                            infoSubEtat.value = currentlySelected.SubEtat;
                            infoOffsetBx.text = currentlySelected.OffsetBX.ToString();
                            infoOffsetBy.text = currentlySelected.OffsetBY.ToString();
                            infoOffsetHy.text = currentlySelected.OffsetHY.ToString();
                            infoFollowSprite.text = currentlySelected.FollowSprite.ToString();
                            infoHitPoints.text = currentlySelected.HitPoints.ToString();
                            infoHitSprite.text = currentlySelected.HitSprite.ToString();
                            infoFollow.isOn = currentlySelected.FollowEnabled;
                            infoType.value = (int)currentlySelected.Type;
                            infoAnimIndex.text = currentlySelected.AnimationIndex.ToString();
                            infoLayer.text = currentlySelected.Layer.ToString();
                        }
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

                            infoX.text = Mathf.RoundToInt((mousePos.x - selectedPosition.x) * 16).ToString();
                            infoY.text = Mathf.RoundToInt(-(mousePos.y - selectedPosition.y) * 16).ToString();

                            uint.TryParse(infoX.text, out var new_x);
                            currentlySelected.XPosition = new_x;
                            uint.TryParse(infoY.text, out var new_y);
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

        private void UpdateInfoEtat() {
            //Clear old options
            infoEtat.options.Clear();
            //Populate new options
            var max = Controller.obj.levelController.EditorManager.GetMaxEtat(currentlySelected.ETA);
            for (int i = 0; i <= max; i++) {
                Dropdown.OptionData dat = new Dropdown.OptionData {
                    text = i.ToString()
                };
                infoEtat.options.Add(dat);
            }
            UpdateInfoSubEtat();
        }
        private void UpdateInfoSubEtat() {
            //Clear old options
            infoSubEtat.options.Clear();
            //Populate new options
            var max = Controller.obj.levelController.EditorManager.GetMaxSubEtat(currentlySelected.ETA, currentlySelected.Etat);
            for (int i = 0; i <= max; i++) {
                Dropdown.OptionData dat = new Dropdown.OptionData {
                    text = i.ToString()
                };
                infoSubEtat.options.Add(dat);
            }
        }

        //-----OnChanged methods for each field-----

        public void FieldXPosition() {
            if (currentlySelected != null) {
                uint.TryParse(infoX.text, out var new_x);
                if (new_x != currentlySelected.XPosition) {
                    currentlySelected.XPosition = new_x;
                    currentlySelected.UpdateXAndY();
                }
            }
        }
        public void FieldYPosition() {
            if (currentlySelected != null) {
                uint.TryParse(infoY.text, out var new_y);
                if (new_y != currentlySelected.YPosition) {
                    currentlySelected.YPosition = new_y;
                    currentlySelected.UpdateXAndY();
                }
            }
        }
        public void FieldDes() {
            if (currentlySelected != null) {
                if (infoDes.value != currentlySelected.DES) {
                    currentlySelected.DES = infoDes.value;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();
                }
            }
        }
        public void FieldEta() {
            if (currentlySelected != null) {
                if (infoEta.value != currentlySelected.ETA) {
                    currentlySelected.ETA = infoEta.value;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();
                }
            }
        }
        public void FieldEtat() {
            if (currentlySelected != null) {
                if (infoEtat.value != currentlySelected.Etat) {
                    currentlySelected.Etat = infoEtat.value;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();

                    UpdateInfoEtat();
                }
            }
        }
        public void FieldSubEtat() {
            if (currentlySelected != null) {
                if (infoSubEtat.value != currentlySelected.SubEtat) {
                    currentlySelected.SubEtat = infoSubEtat.value;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();

                    UpdateInfoSubEtat();
                }
            }
        }
        public void FieldOffsetBx() {
            if (currentlySelected != null) {
                int.TryParse(infoOffsetBx.text, out var new_offbx);
                if (new_offbx != currentlySelected.OffsetBX) {
                    currentlySelected.OffsetBX = new_offbx;

                    currentlySelected.RefreshName();
                    currentlySelected.UpdateOffsetPoints();
                }
            }
        }
        public void FieldOffsetBy() {
            if (currentlySelected != null) {
                int.TryParse(infoOffsetBy.text, out var new_offby);
                if (new_offby != currentlySelected.OffsetBY) {
                    currentlySelected.OffsetBY = new_offby;

                    currentlySelected.RefreshName();
                    currentlySelected.UpdateOffsetPoints();
                }
            }
        }
        public void FieldOffsetHy() {
            if (currentlySelected != null) {
                int.TryParse(infoOffsetHy.text, out var new_offhy);
                if (new_offhy != currentlySelected.OffsetHY) {
                    currentlySelected.OffsetHY = new_offhy;

                    currentlySelected.RefreshName();
                    currentlySelected.UpdateOffsetPoints();
                }
            }
        }
        public void FieldFollowSprite() {
            if (currentlySelected != null) {
                int.TryParse(infoFollowSprite.text, out var new_fsprite);
                if (new_fsprite != currentlySelected.FollowSprite) {
                    currentlySelected.FollowSprite = new_fsprite;

                    currentlySelected.RefreshName();
                }
            }
        }
        public void FieldHitPoints() {
            if (currentlySelected != null) {
                int.TryParse(infoHitPoints.text, out var new_hp);
                if (new_hp != currentlySelected.HitPoints) {
                    currentlySelected.HitPoints = new_hp;

                    currentlySelected.RefreshName();
                    currentlySelected.ChangeFlip();
                }
            }
        }
        public void FieldHitSprite() {
            if (currentlySelected != null) {
                int.TryParse(infoHitSprite.text, out var new_hsprite);
                if (new_hsprite != currentlySelected.HitSprite) {
                    currentlySelected.HitSprite = new_hsprite;

                    currentlySelected.RefreshName();
                }
            }
        }
        public void FieldFollowEnabled() {
            if (currentlySelected != null) {
                if (infoFollow.isOn != currentlySelected.FollowEnabled) {
                    currentlySelected.FollowEnabled = infoFollow.isOn;

                    currentlySelected.RefreshName();
                }
            }
        }
        public void FieldType() {
            if (currentlySelected != null) {
                if ((EventType)infoType.value != currentlySelected.Type) {
                    currentlySelected.Type = (EventType)infoType.value;

                    currentlySelected.RefreshFlag();
                    currentlySelected.RefreshName();
                }
            }
        }
        public void FieldAnimIndex() {
            if (currentlySelected != null) {
                int.TryParse(infoAnimIndex.text, out var new_anim);
                if (new_anim != currentlySelected.AnimationIndex) {
                    currentlySelected.AnimationIndex = new_anim;
                    currentlySelected.ChangeAnimation(new_anim);
                }
            }
        }

        //--------------------

        private void ClearInfoWindow() {
            infoName.text = "";
            infoX.text = "";
            infoY.text = "";
            infoDes.value = 0;
            infoEta.value = 0;
            infoEtat.value = 0;
            infoSubEtat.value = 0;
            infoOffsetBx.text = "";
            infoOffsetBy.text = "";
            infoOffsetHy.text = "";
            infoFollowSprite.text = "";
            infoHitPoints.text = "";
            infoHitSprite.text = "";
            infoFollow.isOn = false;
            infoType.value = 0;
            infoAnimIndex.text = "";
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
        public Common_Event AddEvent(EventType type, int etat, int subEtat, uint xpos, uint ypos, int des, int eta, int offsetBX, int offsetBY, int offsetHY, int followSprite, int hitpoints, int layer, int hitSprite, bool followEnabled, ushort[] labelOffsets, Common_EventCommandCollection commands, int link) {
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
            newEvent.Layer = layer;
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
