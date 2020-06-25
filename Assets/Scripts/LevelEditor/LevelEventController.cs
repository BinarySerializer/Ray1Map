using System;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace R1Engine
{
    public class LevelEventController : MonoBehaviour 
    {
        #region Fields

        // TODO: Maybe get rid of this?
        /// <summary>
        /// The view model for the event editor
        /// </summary>
        public EventEditorViewModel ViewModel;

        // Prefabs
        public GameObject eventParent;
        public GameObject prefabEvent;
        public GameObject prefabCommandLine;

        public Editor editor;

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

        public bool hasLoaded;

        #endregion

        #region Field Changed Methods

        public void FieldXPosition() => ViewModel.XPosition = uint.TryParse(infoX.text, out var v) ? v : 0;
        public void FieldYPosition() => ViewModel.XPosition = uint.TryParse(infoY.text, out var v) ? v : 0;
        public void FieldDes() => ViewModel.DES = infoDes.options[infoDes.value].text;
        public void FieldEta() => ViewModel.ETA = infoEta.options[infoEta.value].text;
        public void FieldEtat() => ViewModel.Etat = (byte)infoEtat.value;
        public void FieldSubEtat() => ViewModel.SubEtat = (byte)infoSubEtat.value;
        public void FieldOffsetBx() => ViewModel.OffsetBX = byte.TryParse(infoOffsetBx.text, out var v) ? v : (byte)0;
        public void FieldOffsetBy() => ViewModel.OffsetBY = byte.TryParse(infoOffsetBy.text, out var v) ? v : (byte)0;
        public void FieldOffsetHy() => ViewModel.OffsetHY = byte.TryParse(infoOffsetHy.text, out var v) ? v : (byte)0;
        public void FieldFollowSprite() => ViewModel.FollowSprite = byte.TryParse(infoFollowSprite.text, out var v) ? v : (byte)0;
        public void FieldHitPoints() => ViewModel.HitPoints = byte.TryParse(infoHitPoints.text, out var v) ? v : (byte)0;
        public void FieldHitSprite() => ViewModel.HitSprite = byte.TryParse(infoHitSprite.text, out var v) ? v : (byte)0;
        public void FieldFollowEnabled() => ViewModel.FollowEnabled = infoFollow.isOn;
        public void FieldType() => ViewModel.Type = (Enum)Enum.Parse(Controller.obj.levelController.EditorManager.EventTypeEnumType, infoType.value.ToString());

        public void FieldAnimIndex() => throw new NotImplementedException("The animation index can not be updated");

        #endregion

        public void InitializeEvents() 
        {
            InitializeViewModel();

            // Initialize Rayman's animation as they're shared for small and dark Rayman
            ViewModel.EditorManager.InitializeRayAnim();

            // Setup events
            foreach (var e in Controller.obj.levelController.GetAllEvents)
                e.InitialSetup();

            // Initialize links
            InitializeEventLinks();

            // Fill eventinfo dropdown with the event types
            infoType.options.AddRange(ViewModel.EditorManager.EventTypes.Select(x => new Dropdown.OptionData
            {
                text = x
            }));

            // Fill the dropdown menu
            eventDropdown.options = ViewModel.EditorManager.GetEvents().Select(x => new Dropdown.OptionData
            {
                text = x
            }).ToList();

            // Default to the first event
            eventDropdown.captionText.text = eventDropdown.options.FirstOrDefault()?.text;

            // Fill Des and Eta dropdowns with their max values
            infoDes.options = ViewModel.EditorManager.DES.Select(x => new Dropdown.OptionData(x.Key)).ToList();
            infoEta.options = ViewModel.EditorManager.ETA.Select(x => new Dropdown.OptionData(x.Key)).ToList();

            hasLoaded = true;
        }

        protected void InitializeViewModel()
        {
            ViewModel = new EventEditorViewModel(Controller.obj.levelController.EditorManager);

            ViewModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ViewModel.XPosition):
                        infoX.text = ViewModel.XPosition?.ToString() ?? String.Empty;
                        break;

                    case nameof(ViewModel.YPosition):
                        infoY.text = ViewModel.YPosition?.ToString() ?? String.Empty;
                        break;

                    case nameof(ViewModel.DES):
                        infoDes.value = infoDes.options.FindIndex(x => x.text == ViewModel.DES);
                        break;

                    case nameof(ViewModel.ETA):
                        infoEta.value = infoEta.options.FindIndex(x => x.text == ViewModel.ETA);

                        UpdateInfoEtat();
                        UpdateInfoSubEtat();

                        if (ViewModel.Etat >= infoEtat.options.Count)
                            ViewModel.Etat = 0;

                        if (ViewModel.SubEtat >= infoSubEtat.options.Count)
                            ViewModel.SubEtat = 0;

                        break;

                    case nameof(ViewModel.Etat):
                        infoEtat.value = ViewModel.Etat ?? 0;

                        UpdateInfoSubEtat();

                        if (ViewModel.SubEtat >= infoSubEtat.options.Count)
                            ViewModel.SubEtat = 0;

                        break;

                    case nameof(ViewModel.SubEtat):
                        infoSubEtat.value = ViewModel.SubEtat ?? 0;
                        break;

                    case nameof(ViewModel.OffsetBX):
                        infoOffsetBx.text = (ViewModel.OffsetBX ?? 0).ToString();
                        break;

                    case nameof(ViewModel.OffsetBY):
                        infoOffsetBy.text = (ViewModel.OffsetBY ?? 0).ToString();
                        break;

                    case nameof(ViewModel.OffsetHY):
                        infoOffsetHy.text = (ViewModel.OffsetHY ?? 0).ToString();
                        break;

                    case nameof(ViewModel.FollowSprite):
                        infoFollowSprite.text = (ViewModel.FollowSprite ?? 0).ToString();
                        break;

                    case nameof(ViewModel.HitPoints):
                        infoHitPoints.text = (ViewModel.HitPoints ?? 0).ToString();
                        break;

                    case nameof(ViewModel.HitSprite):
                        infoHitSprite.text = (ViewModel.HitSprite ?? 0).ToString();
                        break;

                    case nameof(ViewModel.FollowEnabled):
                        infoFollow.isOn = ViewModel.FollowEnabled ?? false;
                        break;

                    case nameof(ViewModel.Type):
                        infoType.value = (ushort)Convert.ChangeType(ViewModel.Type ?? (object)0, typeof(ushort));
                        break;
                }
            };
        }

        protected void InitializeEventLinks()
        {
            var eventList = Controller.obj.levelController.Events;

            // Convert linkIndex of each event to linkId
            for (int i = 0; i < eventList.Count; i++)
            {
                // If X and Y are insane, clamp them
                const int border = 10;
                eventList[i].Data.EventData.XPosition = (uint)Mathf.Clamp(eventList[i].Data.EventData.XPosition, -border, (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Width * 16) + border);
                eventList[i].Data.EventData.YPosition = (uint)Mathf.Clamp(eventList[i].Data.EventData.YPosition, -border, (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Height * 16) + border);

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
                    int prevEvent = i;
                    while (nextEvent != i && nextEvent != prevEvent)
                    {
                        prevEvent = nextEvent;
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

            //Create empty list for commandlines
            commandLines = new List<CommandLine>();
        }

        public void ChangeEventsVisibility(object o, EventArgs e) {
            if (Controller.obj.levelController.currentLevel != null) {
                foreach (var eve in Controller.obj.levelController.GetAllEvents) {
                    if (editor.currentMode == Editor.EditMode.Links)
                        eve.ChangeLinksVisibility(true);
                }
            }
        }

        private float memoryLoadTimer = 0;

        private void Update() 
        {
            if (!hasLoaded)
                return;

            if (Settings.LoadFromMemory)
            {
                memoryLoadTimer += Time.deltaTime;
                if (memoryLoadTimer > 1.0f / 60.0f)
                {
                    UpdateFromMemory();
                    memoryLoadTimer = 0.0f;
                }
            }
            else
            {
                GameMemoryContext?.Dispose();
                GameMemoryContext = null;
            }

            // Only do this if in event/link mode
            bool modeEvents = editor.currentMode == Editor.EditMode.Events;
            bool modeLinks = editor.currentMode == Editor.EditMode.Links;

            if ( modeEvents || modeLinks ) 
            {
                selectedLineRend.enabled = true;
                
                // Add events with mmb
                if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject() && modeEvents) 
                {
                    Vector2 mousepo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var mox = mousepo.x * 16;
                    var moy = mousepo.y * 16;
                    
                    // Don't add if clicked outside of the level bounds
                    if (mox > 0 && -moy > 0 && mox < Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Width*16 && -moy < Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Height*16) 
                    {
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
                        
                        if (ViewModel?.SelectedEvent != null)
                            ViewModel.SelectedEvent.ChangeOffsetVisibility(false);

                        if (e != ViewModel?.SelectedEvent && ViewModel != null) {
                            ViewModel.SelectedEvent = e;
                            //Change event info if event is selected
                            infoName.text = ViewModel.DisplayName;
                            infoAnimIndex.text = ViewModel.SelectedEvent.Data.EventData.RuntimeCurrentAnimIndex.ToString();
                            infoLayer.text = ViewModel.SelectedEvent.Data.EventData.Layer.ToString();
                            //Clear old commands
                            ClearCommands();
                            //Fill out the commands
                            foreach (var c in ViewModel.SelectedEvent.Data.CommandCollection?.Commands ?? new Common_EventCommand[0]) {
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
                        ViewModel.SelectedEvent.ChangeOffsetVisibility(true);
                        //Change the link
                        if (modeLinks && ViewModel.SelectedEvent != Controller.obj.levelController.RaymanEvent) {
                            ViewModel.SelectedEvent.LinkID = 0;
                            ViewModel.SelectedEvent.ChangeLinksVisibility(true);
                        }
                    }
                    else {
                        if (ViewModel?.SelectedEvent != null)
                            ViewModel.SelectedEvent.ChangeOffsetVisibility(false);
                        selectedLineRend.enabled = false;
                        ViewModel.SelectedEvent = null;
                        //Clear info window
                        ClearInfoWindow();
                    }
                }
                //Drag and move the event
                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    if (ViewModel?.SelectedEvent != null) {
                        //Move event if in event mode
                        if (modeEvents) {
                            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                            ViewModel.XPosition = (uint)Mathf.Clamp(Mathf.RoundToInt((mousePos.x - selectedPosition.x) * 16), 0, UInt32.MaxValue);
                            ViewModel.YPosition = (uint)Mathf.Clamp(Mathf.RoundToInt(-(mousePos.y - selectedPosition.y) * 16), 0, UInt32.MaxValue);
                        }
                        //Else move links
                        if (modeLinks && ViewModel.SelectedEvent != Controller.obj.levelController.RaymanEvent) {
                            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            ViewModel.SelectedEvent.linkCube.position = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
                        }
                    }
                }
                //Confirm links with mmb
                if (Input.GetMouseButtonDown(2) && modeLinks && ViewModel.SelectedEvent.LinkID==0) {
                    bool alone = true;
                    foreach (var ee in Controller.obj.levelController.Events) {
                        if (ee.linkCube.position== ViewModel.SelectedEvent.linkCube.position) {
                            if (ee != ViewModel.SelectedEvent) {
                                ee.LinkID = currentId;
                                ee.ChangeLinksVisibility(true);
                                ee.linkCubeLockPosition = ee.linkCube.position;
                                alone = false;
                            }
                        }
                    }
                    if (!alone) {
                        ViewModel.SelectedEvent.LinkID = currentId;
                        ViewModel.SelectedEvent.ChangeLinksVisibility(true);
                        ViewModel.SelectedEvent.linkCubeLockPosition = ViewModel.SelectedEvent.linkCube.position;
                    }
                    currentId++;
                }
                //Delete selected event
                if (Input.GetKeyDown(KeyCode.Delete) && modeEvents) {
                    if (ViewModel?.SelectedEvent != null) {
                        ViewModel.SelectedEvent.ChangeOffsetVisibility(false);
                        ViewModel.SelectedEvent.Delete();
                        ViewModel.SelectedEvent = null;
                        ClearInfoWindow();
                    }
                }
            }
            else {
                selectedLineRend.enabled = false;
            }
        }

        public Context GameMemoryContext { get; set; }
        public Pointer GameMemoryOffset { get; set; }
        public Pointer EventArrayOffset { get; set; }
        public Pointer RayEventOffset { get; set; }

        public void UpdateFromMemory()
        {
            // TODO: Dispose when we stop program?
            if (GameMemoryContext == null)
            {
                GameMemoryContext = new Context(Controller.CurrentSettings);

                try
                {
                    var file = new ProcessMemoryStreamFile("MemStream", "DOSBox.exe", GameMemoryContext);

                    // TODO: Do not hard-code the process name
                    GameMemoryContext.AddFile(file);

                    // TODO: Do not hard-code game base pointer - have manager get this (it'll differ for different emulator versions too!)
                    var offset = file.StartPointer;
                    var s = GameMemoryContext.Deserializer;

                    // TODO: Have managers handle this - this is currently hard-coded for PC 1.21
                    // Get pointers
                    GameMemoryOffset = s.DoAt(offset + 0x01D3A1A0, () => s.SerializePointer(default, name: nameof(GameMemoryOffset)));
                    EventArrayOffset = s.DoAt(GameMemoryOffset + 0x16DDF0, () => s.SerializePointer(EventArrayOffset, anchor: GameMemoryOffset, name: nameof(EventArrayOffset)));
                    RayEventOffset = GameMemoryOffset + 0x16F650;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    GameMemoryContext = null;
                }
            }

            if (GameMemoryContext != null)
            {
                Pointer currentOffset = EventArrayOffset;
                SerializerObject s;
                foreach (Editor_EventData ed in Controller.obj.levelController.EditorManager.Level.EventData)
                {
                    s = ed.HasPendingEdits ? (SerializerObject)GameMemoryContext.Serializer : GameMemoryContext.Deserializer;
                    s.Goto(currentOffset);
                    ed.EventData.Init(s.CurrentPointer);
                    ed.EventData.Serialize(s);
                    ed.DebugText = $"Pos: {ed.EventData.XPosition}, {ed.EventData.YPosition}{Environment.NewLine}" +
                                   $"RuntimePos: {ed.EventData.RuntimeXPosition}, {ed.EventData.RuntimeYPosition}{Environment.NewLine}" +
                                   $"Unk_48: {ed.EventData.Unk_48}{Environment.NewLine}" +
                                   $"Unk_70: {ed.EventData.RuntimeCMDOffset}{Environment.NewLine}" +
                                   $"Unk_112: {ed.EventData.Unk_112}{Environment.NewLine}" +
                                   $"Flags: {Convert.ToString((byte)ed.EventData.PC_Flags, 2).PadLeft(8, '0')}{Environment.NewLine}";

                    ed.HasPendingEdits = false;
                    currentOffset = s.CurrentPointer;
                }
                currentOffset = RayEventOffset;
                var ray = Controller.obj.levelController.EditorManager.Level.Rayman;
                s = ray.HasPendingEdits ? (SerializerObject)GameMemoryContext.Serializer : GameMemoryContext.Deserializer;
                s.Goto(currentOffset);
                ray.EventData.Init(s.CurrentPointer);
                ray.EventData.Serialize(s);

                ray.HasPendingEdits = false;
            }
        }

        private void LateUpdate() {
            //Update selection square lines
            if (ViewModel?.SelectedEvent != null) {
                selectedLineRend.SetPosition(0, new Vector2(ViewModel.SelectedEvent.midpoint.x - ViewModel.SelectedEvent.boxCollider.size.x / 2f, ViewModel.SelectedEvent.midpoint.y - ViewModel.SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(1, new Vector2(ViewModel.SelectedEvent.midpoint.x + ViewModel.SelectedEvent.boxCollider.size.x / 2f, ViewModel.SelectedEvent.midpoint.y - ViewModel.SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(2, new Vector2(ViewModel.SelectedEvent.midpoint.x + ViewModel.SelectedEvent.boxCollider.size.x / 2f, ViewModel.SelectedEvent.midpoint.y + ViewModel.SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(3, new Vector2(ViewModel.SelectedEvent.midpoint.x - ViewModel.SelectedEvent.boxCollider.size.x / 2f, ViewModel.SelectedEvent.midpoint.y + ViewModel.SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(4, new Vector2(ViewModel.SelectedEvent.midpoint.x - ViewModel.SelectedEvent.boxCollider.size.x / 2f, ViewModel.SelectedEvent.midpoint.y - ViewModel.SelectedEvent.boxCollider.size.y / 2f));
            }
            else {
                selectedLineRend.SetPosition(0, Vector2.zero);
                selectedLineRend.SetPosition(1, Vector2.zero);
                selectedLineRend.SetPosition(2, Vector2.zero);
                selectedLineRend.SetPosition(3, Vector2.zero);
                selectedLineRend.SetPosition(4, Vector2.zero);
            }
        }

        public void UpdateInfoEtat() 
        {
            // Clear old options
            infoEtat.options.Clear();

            // Populate new options
            var max = ViewModel.ETA == null ? 0 : ViewModel.EditorManager.ETA.TryGetItem(ViewModel.ETA)?.Length ?? 0;
            for (int i = 0; i < max; i++) {
                Dropdown.OptionData dat = new Dropdown.OptionData {
                    text = i.ToString()
                };
                infoEtat.options.Add(dat);
            }
        }
        private void UpdateInfoSubEtat() 
        {
            // Clear old options
            infoSubEtat.options.Clear();

            // Populate new options
            var max = ViewModel.ETA == null ? 0 : Controller.obj.levelController.EditorManager.ETA.TryGetItem(ViewModel.ETA)?.ElementAtOrDefault(ViewModel.Etat ?? -1)?.Length ?? 0;
            for (int i = 0; i < max; i++) {
                Dropdown.OptionData dat = new Dropdown.OptionData {
                    text = i.ToString()
                };
                infoSubEtat.options.Add(dat);
            }
        }

        private void ClearInfoWindow() {
            infoName.text = "";
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
            if (Controller.obj.levelController.currentLevel != null && areLinksVisible != t) {
                areLinksVisible = t;
                foreach (var e in Controller.obj.levelController.Events) {
                    e.ChangeLinksVisibility(t);
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
        public Common_Event AddEvent(Editor_EventData eventData)
        {
            // Instantiate prefab
            Common_Event newEvent = Instantiate(prefabEvent, new Vector3(eventData.EventData.XPosition / 16f, -(eventData.EventData.YPosition / 16f), eventData.EventData.Layer), Quaternion.identity).GetComponent<Common_Event>();

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
