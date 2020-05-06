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
        public GameObject prefabCommandLine;

        public Editor editor;

        public Common_Event currentlySelected;
        public Vector2 selectedPosition;
        public LineRenderer selectedLineRend;

        public Dropdown eventDropdown;

        // Event info things for the ui
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

        //Command ui stuff
        public List<CommandLine> commandLines;
        public Transform commandListParent;

        public bool areLinksVisible = false;

        public Color linkColorActive;
        public Color linkColorDeactive;

        //Keeping track of used linkIds
        public int currentId = 1;

        public int lastUsedLayer = 0;

        public void InitializeEvents() 
        {
            // TODO: Scale events here

            // Initialize Rayman's animation as they're shared for small and dark Rayman
            InitializeRayAnim();

            InitializeEventLinks();

            // Fill the dropdown menu
            eventDropdown.options = Controller.obj.levelController.EditorManager.GetEvents().Select(x => new Dropdown.OptionData
            {
                text = x
            }).ToList();

            // Fill Des and Eta dropdowns with their max values
            infoDes.options = Controller.obj.levelController.EditorManager.DES.Select(x => new Dropdown.OptionData(x.Key)).ToList();
            infoEta.options = Controller.obj.levelController.EditorManager.ETA.Select(x => new Dropdown.OptionData(x.Key)).ToList();

            // Default to the first event
            eventDropdown.captionText.text = eventDropdown.options.FirstOrDefault()?.text;
        }

        public void InitializeRayAnim()
        {
            var eventList = Controller.obj.levelController.Events;

            // Hard-code event animations for the different Rayman types
            Common_Design rayDes = null;

            var rayEvent = eventList.Find(x => x.Data.Type == EventType.TYPE_RAY_POS);

            if (rayEvent != null)
                rayDes = Controller.obj.levelController.EditorManager.DES.TryGetItem(rayEvent.Data.DESKey);

            if (rayDes != null)
            {
                var miniRay = eventList.Find(x => x.Data.Type == EventType.TYPE_DEMI_RAYMAN);

                if (miniRay != null)
                {
                    var des = Controller.obj.levelController.EditorManager.DES.TryGetItem(miniRay.Data.DESKey);

                    if (des != null)
                    {
                        des.Animations = rayDes.Animations.Select(anim =>
                        {
                            var newAnim = new Common_Animation
                            {
                                Frames = anim.Frames.Select(x => new Common_AnimFrame()
                                {
                                    FrameData = new Common_AnimationFrame
                                    {
                                        XPosition = (byte)(x.FrameData.XPosition / 2),
                                        YPosition = (byte)(x.FrameData.YPosition / 2),
                                        Width = (byte)(x.FrameData.Width / 2),
                                        Height = (byte)(x.FrameData.Height / 2)
                                    },
                                    Layers = x.Layers.Select(l => new Common_AnimationPart()
                                    {
                                        SpriteIndex = l.SpriteIndex,
                                        X = l.X / 2,
                                        Y = l.Y / 2,
                                        Flipped = l.Flipped
                                    }).ToArray()
                                }).ToArray()
                            };

                            return newAnim;
                        }).ToList();
                    }
                }

                var badRay = eventList.Find(x => x.Data.Type == EventType.TYPE_BLACK_RAY);

                if (badRay != null)
                {
                    var des = Controller.obj.levelController.EditorManager.DES.TryGetItem(badRay.Data.DESKey);

                    if (des != null)
                        des.Animations = rayDes.Animations;
                }
            }
        }

        public void InitializeEventLinks()
        {
            var eventList = Controller.obj.levelController.Events;

            // Convert linkIndex of each event to linkId
            for (int i = 0; i < eventList.Count; i++)
            {
                // Refresh
                eventList[i].RefreshFlag();
                eventList[i].RefreshEditorInfo();

                // If X and Y are insane, clamp them
                const int border = 10;
                eventList[i].Data.XPosition = (uint)Mathf.Clamp(eventList[i].Data.XPosition, -border, (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Width * 16) + border);
                eventList[i].Data.YPosition = (uint)Mathf.Clamp(eventList[i].Data.YPosition, -border, (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Height * 16) + border);
                eventList[i].UpdateXAndY();

                // No link
                if (eventList[i].Data.LinkIndex == i)
                {
                    eventList[i].LinkID = 0;
                }
                else
                {
                    // Ignore already assigned ones
                    if (eventList[i].LinkID != 0) 
                        continue;
                    
                    // Link found, loop through everyone on the link chain
                    int nextEvent = eventList[i].Data.LinkIndex;
                    eventList[i].LinkID = currentId;
                    eventList[i].linkCubeLockPosition = eventList[i].linkCube.position;
                    while (nextEvent != i)
                    {
                        eventList[nextEvent].LinkID = currentId;

                        // Stack the link cubes
                        eventList[nextEvent].linkCube.position = eventList[i].linkCube.position;
                        eventList[nextEvent].linkCubeLockPosition = eventList[nextEvent].linkCube.position;

                        nextEvent = eventList[nextEvent].Data.LinkIndex;
                    }
                    currentId++;
                }
            }
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
            //Create empty list for commandlines
            commandLines = new List<CommandLine>();
        }

        public void ChangeEventsVisibility(object o, EventArgs e) {
            if (Controller.obj.levelController.currentLevel != null) {
                foreach (var eve in Controller.obj.levelController.Events) {
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
                    if (mox > 0 && -moy > 0 && mox < Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Width*16 && -moy < Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Height*16) {

                        var eventData = Controller.obj.levelController.EditorManager.AddEvent(eventDropdown.value, (uint)mox, (uint)-moy);

                        Controller.obj.levelController.currentLevel.EventData.Add(eventData);
                        var eve = AddEvent(eventData);

                        // Refresh the event
                        eve.RefreshEditorInfo();

                        Controller.obj.levelController.Events.Add(eve);
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
                            infoX.text = currentlySelected.Data.XPosition.ToString();
                            infoY.text = currentlySelected.Data.YPosition.ToString();
                            infoDes.value = infoDes.options.FindIndex(x => x.text == currentlySelected.Data.DESKey);
                            infoEta.value = infoEta.options.FindIndex(x => x.text == currentlySelected.Data.ETAKey);
                            UpdateInfoEtat();
                            infoEtat.value = currentlySelected.Data.Etat;
                            infoSubEtat.value = currentlySelected.Data.SubEtat;
                            infoOffsetBx.text = currentlySelected.Data.OffsetBX.ToString();
                            infoOffsetBy.text = currentlySelected.Data.OffsetBY.ToString();
                            infoOffsetHy.text = currentlySelected.Data.OffsetHY.ToString();
                            infoFollowSprite.text = currentlySelected.Data.FollowSprite.ToString();
                            infoHitPoints.text = currentlySelected.Data.HitPoints.ToString();
                            infoHitSprite.text = currentlySelected.Data.HitSprite.ToString();
                            infoFollow.isOn = currentlySelected.Data.FollowEnabled;
                            infoType.value = (int)currentlySelected.Data.Type;
                            infoAnimIndex.text = currentlySelected.AnimationIndex.ToString();
                            infoLayer.text = currentlySelected.Data.Layer.ToString();
                            //Clear old commands
                            ClearCommands();
                            //Fill out the commands
                            foreach (var c in currentlySelected.Data.CommandCollection?.Commands ?? new Common_EventCommand[0]) {
                                CommandLine cmd = Instantiate<GameObject>(prefabCommandLine, new Vector3(0,0,0), Quaternion.identity).GetComponent<CommandLine>();
                                cmd.command = c;
                                cmd.transform.SetParent(commandListParent, false);
                                commandLines.Add(cmd);
                            }
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
                            currentlySelected.Data.XPosition = new_x;
                            uint.TryParse(infoY.text, out var new_y);
                            currentlySelected.Data.YPosition = new_y;

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
                    foreach (var ee in Controller.obj.levelController.Events) {
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
            var max = Controller.obj.levelController.EditorManager.ETA.TryGetItem(currentlySelected.Data.ETAKey)?.Length ?? 0;
            for (int i = 0; i < max; i++) {
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
            var max = Controller.obj.levelController.EditorManager.ETA.TryGetItem(currentlySelected.Data.ETAKey)?.ElementAtOrDefault(currentlySelected.Data.Etat)?.Length ?? 0;
            for (int i = 0; i < max; i++) {
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
                if (new_x != currentlySelected.Data.XPosition) {
                    currentlySelected.Data.XPosition = new_x;
                    currentlySelected.UpdateXAndY();
                }
            }
        }
        public void FieldYPosition() {
            if (currentlySelected != null) {
                uint.TryParse(infoY.text, out var new_y);
                if (new_y != currentlySelected.Data.YPosition) {
                    currentlySelected.Data.YPosition = new_y;
                    currentlySelected.UpdateXAndY();
                }
            }
        }
        public void FieldDes() {
            if (currentlySelected != null) {
                if (infoDes.options[infoDes.value].text != currentlySelected.Data.DESKey) {
                    currentlySelected.Data.DESKey = infoDes.options[infoDes.value].text;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();
                }
            }
        }
        public void FieldEta() {
            if (currentlySelected != null) {
                if (infoEta.options[infoEta.value].text != currentlySelected.Data.ETAKey) {
                    currentlySelected.Data.ETAKey = infoEta.options[infoEta.value].text;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();

                    UpdateInfoEtat();
                }
            }
        }
        public void FieldEtat() {
            if (currentlySelected != null) {
                if (infoEtat.value != currentlySelected.Data.Etat) {
                    currentlySelected.Data.Etat = infoEtat.value;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();

                    UpdateInfoSubEtat();
                }
            }
        }
        public void FieldSubEtat() {
            if (currentlySelected != null) {
                if (infoSubEtat.value != currentlySelected.Data.SubEtat) {
                    currentlySelected.Data.SubEtat = infoSubEtat.value;

                    currentlySelected.RefreshName();
                    currentlySelected.RefreshVisuals();                  
                }
            }
        }
        public void FieldOffsetBx() {
            if (currentlySelected != null) {
                int.TryParse(infoOffsetBx.text, out var new_offbx);
                if (new_offbx != currentlySelected.Data.OffsetBX) {
                    currentlySelected.Data.OffsetBX = new_offbx;

                    currentlySelected.RefreshName();
                    currentlySelected.UpdateOffsetPoints();
                }
            }
        }
        public void FieldOffsetBy() {
            if (currentlySelected != null) {
                int.TryParse(infoOffsetBy.text, out var new_offby);
                if (new_offby != currentlySelected.Data.OffsetBY) {
                    currentlySelected.Data.OffsetBY = new_offby;

                    currentlySelected.RefreshName();
                    currentlySelected.UpdateOffsetPoints();
                }
            }
        }
        public void FieldOffsetHy() {
            if (currentlySelected != null) {
                int.TryParse(infoOffsetHy.text, out var new_offhy);
                if (new_offhy != currentlySelected.Data.OffsetHY) {
                    currentlySelected.Data.OffsetHY = new_offhy;

                    currentlySelected.RefreshName();
                    currentlySelected.UpdateOffsetPoints();
                    currentlySelected.UpdateFollowSpriteLine();
                }
            }
        }
        public void FieldFollowSprite() {
            if (currentlySelected != null) {
                int.TryParse(infoFollowSprite.text, out var new_fsprite);
                if (new_fsprite != currentlySelected.Data.FollowSprite) {
                    currentlySelected.Data.FollowSprite = new_fsprite;

                    currentlySelected.RefreshName();
                    currentlySelected.UpdateFollowSpriteLine();
                }
            }
        }
        public void FieldHitPoints() {
            if (currentlySelected != null) {
                int.TryParse(infoHitPoints.text, out var new_hp);
                if (new_hp != currentlySelected.Data.HitPoints) {
                    currentlySelected.Data.HitPoints = new_hp;

                    currentlySelected.RefreshVisuals();
                    currentlySelected.RefreshName();
                }
            }
        }
        public void FieldHitSprite() {
            if (currentlySelected != null) {
                int.TryParse(infoHitSprite.text, out var new_hsprite);
                if (new_hsprite != currentlySelected.Data.HitSprite) {
                    currentlySelected.Data.HitSprite = new_hsprite;

                    currentlySelected.RefreshName();
                }
            }
        }
        public void FieldFollowEnabled() {
            if (currentlySelected != null) {
                if (infoFollow.isOn != currentlySelected.Data.FollowEnabled) {
                    currentlySelected.Data.FollowEnabled = infoFollow.isOn;

                    currentlySelected.RefreshName();
                    currentlySelected.ChangeOffsetVisibility(true);
                }
            }
        }
        public void FieldType() {
            if (currentlySelected != null) {
                if ((EventType)infoType.value != currentlySelected.Data.Type) {
                    currentlySelected.Data.Type = (EventType)infoType.value;

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
            infoLayer.text = "";

            ClearCommands();
        }

        private void ClearCommands() {
            foreach(var c in commandLines) {
                Destroy(c.gameObject);
            }
            commandLines.Clear();
        }

        // Show/Hide links
        public void ToggleLinks(bool t) {
            if (Controller.obj.levelController.currentLevel != null) {
                if (areLinksVisible != t) {
                    areLinksVisible = t;
                    foreach (var e in Controller.obj.levelController.Events) {
                        e.ChangeLinksVisibility(t);
                    }
                }
            }
        }

        // Converts linkID to linkIndex when saving
        public void CalculateLinkIndexes() {

            List<int> alreadyChained = new List<int>();
            foreach (Common_Event ee in Controller.obj.levelController.Events) {
                // No link
                if (ee.LinkID == 0) {
                    ee.Data.LinkIndex = Controller.obj.levelController.Events.IndexOf(ee);
                }
                else {
                    // Skip if already chained
                    if (alreadyChained.Contains(Controller.obj.levelController.Events.IndexOf(ee)))
                        continue;

                    // Find all the events with the same linkId and store their indexes
                    List<int> indexesOfSameId = new List<int>();
                    int cur = ee.LinkID;
                    foreach (Common_Event e in Controller.obj.levelController.Events.Where<Common_Event>(e => e.LinkID == cur)) {
                        indexesOfSameId.Add(Controller.obj.levelController.Events.IndexOf(e));
                        alreadyChained.Add(Controller.obj.levelController.Events.IndexOf(e));
                    }
                    // Loop through and chain them
                    for (int j = 0; j < indexesOfSameId.Count; j++) {
                        int next = j + 1;
                        if (next == indexesOfSameId.Count)
                            next = 0;

                        Controller.obj.levelController.Events[indexesOfSameId[j]].Data.LinkIndex = indexesOfSameId[next];
                    }
                }
            }
        }

        // Add events to the list via the managers
        public Common_Event AddEvent(Common_EventData eventData)
        {
            // Instantiate prefab
            Common_Event newEvent = Instantiate(prefabEvent, new Vector3(eventData.XPosition / 16f, -(eventData .YPosition / 16f), eventData.Layer), Quaternion.identity).GetComponent<Common_Event>();

            newEvent.Data = eventData;

            newEvent.UniqueLayer = -lastUsedLayer;
            lastUsedLayer++;

            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;

            // Add to list
            return newEvent;
        }
    }
}
