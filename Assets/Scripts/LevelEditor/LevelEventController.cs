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

        /// <summary>
        /// The currently selected event
        /// </summary>
        public Common_Event SelectedEvent { get; set; }
        public Common_Event PrevSelectedEvent { get; set; }

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

        public BaseEditorManager EditorManager => Controller.obj.levelController.EditorManager;
        public bool LogModifications => false;

        #endregion

        #region Field Changed Methods

        private void FieldUpdated<T>(Action<T> updateAction, T newValue, Func<T> currentValue, string logName, bool refreshName = true)
            where T : IComparable
        {
            if (SelectedEvent != null && !newValue.Equals(currentValue()))
            {
                updateAction(newValue);
                SelectedEvent.Data.HasPendingEdits = true;

                if (refreshName)
                    SelectedEvent.RefreshName();

                if (LogModifications)
                    Debug.Log($"{logName} has been modified");
            }
        }

        public void FieldXPosition() => FieldUpdated(x => SelectedEvent.Data.Data.XPosition = x, Int16.TryParse(infoX.text, out var v) ? v : 0, () => SelectedEvent.Data.Data.XPosition, "XPos", false);
        public void FieldYPosition() => FieldUpdated(x => SelectedEvent.Data.Data.YPosition = x, Int16.TryParse(infoY.text, out var v) ? v : 0, () => SelectedEvent.Data.Data.YPosition, "YPos", false);
        public void FieldDes() => FieldUpdated(x => SelectedEvent.Data.DESKey = x, infoDes.options[infoDes.value].text, () => SelectedEvent.Data.DESKey, "DES");
        public void FieldEta() => FieldUpdated(x => SelectedEvent.Data.ETAKey = x, infoEta.options[infoEta.value].text, () => SelectedEvent.Data.ETAKey, "ETA");
        public void FieldEtat() => FieldUpdated(x => SelectedEvent.Data.Data.Etat = SelectedEvent.Data.Data.RuntimeEtat = x, (byte)infoEtat.value, () => SelectedEvent.Data.Data.Etat, "Etat");
        public void FieldSubEtat() => FieldUpdated(x => SelectedEvent.Data.Data.SubEtat = SelectedEvent.Data.Data.RuntimeSubEtat = x, (byte)infoSubEtat.value, () => SelectedEvent.Data.Data.SubEtat, "SubEtat");
        public void FieldOffsetBx() => FieldUpdated(x => SelectedEvent.Data.Data.OffsetBX = x, byte.TryParse(infoOffsetBx.text, out var v) ? v : (byte)0, () => SelectedEvent.Data.Data.OffsetBX, "BX");
        public void FieldOffsetBy() => FieldUpdated(x => SelectedEvent.Data.Data.OffsetBY = x, byte.TryParse(infoOffsetBy.text, out var v) ? v : (byte)0, () => SelectedEvent.Data.Data.OffsetBY, "BY");
        public void FieldOffsetHy() => FieldUpdated(x => SelectedEvent.Data.Data.OffsetHY = x, byte.TryParse(infoOffsetHy.text, out var v) ? v : (byte)0, () => SelectedEvent.Data.Data.OffsetHY, "HY");
        public void FieldFollowSprite() => FieldUpdated(x => SelectedEvent.Data.Data.FollowSprite = x, byte.TryParse(infoFollowSprite.text, out var v) ? v : (byte)0, () => SelectedEvent.Data.Data.FollowSprite, "FollowSprite");
        public void FieldHitPoints() => FieldUpdated(x =>
        {
            SelectedEvent.Data.Data.ActualHitPoints = x;
            SelectedEvent.Data.Data.RuntimeHitPoints = (byte)(x % 256);
        }, UInt32.TryParse(infoHitPoints.text, out var v) ? v : 0, () => SelectedEvent.Data.Data.ActualHitPoints, "HitPoints");
        public void FieldHitSprite() => FieldUpdated(x => SelectedEvent.Data.Data.HitSprite = x, byte.TryParse(infoHitSprite.text, out var v) ? v : (byte)0, () => SelectedEvent.Data.Data.HitSprite, "HitSprite");
        public void FieldFollowEnabled() => FieldUpdated(x => SelectedEvent.Data.Data.SetFollowEnabled(Controller.CurrentSettings, x), infoFollow.isOn, () => SelectedEvent.Data.Data.GetFollowEnabled(Controller.CurrentSettings), "FollowEnabled");

        public void FieldType() => FieldUpdated(x =>
        {
            SelectedEvent.Data.Type = x;

            if (x is EventType et)
                SelectedEvent.Data.Data.Type = et;
        }, (Enum)Enum.Parse(Controller.obj.levelController.EditorManager.EventTypeEnumType, infoType.value.ToString()), () => SelectedEvent.Data.Type, "Type");

        public void FieldAnimIndex() => throw new NotImplementedException("The animation index can not be updated");
     
        #endregion

        public void InitializeEvents() 
        {
            // Initialize Rayman's animation as they're shared for small and dark Rayman
            EditorManager.InitializeRayAnim();

            // Setup events
            foreach (var e in Controller.obj.levelController.GetAllEvents)
                e.InitialSetup();

            // Initialize links
            InitializeEventLinks();

            // Fill eventinfo dropdown with the event types
            infoType.options.AddRange(EditorManager.EventTypes.Select(x => new Dropdown.OptionData
            {
                text = x
            }));

            // Fill the dropdown menu
            eventDropdown.options = EditorManager.GetEvents().Select(x => new Dropdown.OptionData
            {
                text = x
            }).ToList();

            // Default to the first event
            eventDropdown.captionText.text = eventDropdown.options.FirstOrDefault()?.text;

            // Fill Des and Eta dropdowns with their max values
            infoDes.options = EditorManager.DES.Select(x => new Dropdown.OptionData(x.Key)).ToList();
            infoEta.options = EditorManager.ETA.Select(x => new Dropdown.OptionData(x.Key)).ToList();

            hasLoaded = true;
        }

        protected void InitializeEventLinks()
        {
            var eventList = Controller.obj.levelController.Events;

            // Convert linkIndex of each event to linkId
            for (int i = 0; i < eventList.Count; i++)
            {
                // If X and Y are insane, clamp them
                const int allowedBorder = 200;
                const int border = 10;

                if (eventList[i].Data.Data.XPosition > (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Width * 16) + allowedBorder || eventList[i].Data.Data.XPosition < -allowedBorder)
                    eventList[i].Data.Data.XPosition = (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Width * 16) + border;

                if (eventList[i].Data.Data.YPosition > (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Height * 16) + allowedBorder || eventList[i].Data.Data.YPosition < -allowedBorder)
                    eventList[i].Data.Data.YPosition = (Controller.obj.levelController.currentLevel.Maps[editor.currentMap].Height * 16) + border;

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

        private Common_EventState[] lastSubEtat;

        private void UpdateEventFields()
        {
            // Update Etat and SubEtat drop downs
            var etatLength = SelectedEvent?.Data.ETAKey == null ? 0 : EditorManager.ETA.TryGetItem(SelectedEvent.Data.ETAKey)?.Length ?? 0;
            var subEtatArray = SelectedEvent == null ? null : EditorManager.ETA.TryGetItem(SelectedEvent.Data.ETAKey)?.ElementAtOrDefault(SelectedEvent.Data.Data.Etat);

            if (infoEtat.options.Count != etatLength)
            {
                // Clear old options
                infoEtat.options.Clear();

                // Set new options
                infoEtat.options.AddRange(Enumerable.Range(0, etatLength).Select(x => new Dropdown.OptionData
                {
                    text = x.ToString()
                }));

                //Debug.Log($"Etat array updated to size {etatLength}");
            }
            if (lastSubEtat != subEtatArray)
            {
                var length = subEtatArray?.Length ?? 0;

                // Clear old options
                infoSubEtat.options.Clear();

                // Set new options
                infoSubEtat.options.AddRange(Enumerable.Range(0, length).Select(x => new Dropdown.OptionData
                {
                    text = EditorManager.ETANames?.TryGetItem(SelectedEvent.Data.ETAKey)?.ElementAtOrDefault(SelectedEvent.Data.Data.Etat)?.ElementAtOrDefault(x) ?? x.ToString()
                }));

                lastSubEtat = subEtatArray;

                // Force refresh
                int? subEtat = SelectedEvent?.Data.Data.SubEtat;
                infoSubEtat.value = (subEtat != null && subEtat.Value < subEtatArray.Length ? subEtat.Value : 0);
                infoSubEtat.RefreshShownValue();

                //Debug.Log($"SubEtat array updated to size {length}");
            }

            // Make sure Etat and SubEtat indexes are not out of range
            if (!Settings.LoadFromMemory)
            {
                if (SelectedEvent?.Data.Data.Etat >= infoEtat.options.Count)
                    FieldUpdated(x => SelectedEvent.Data.Data.Etat = SelectedEvent.Data.Data.RuntimeEtat = x, (byte)0, () => SelectedEvent.Data.Data.Etat, "Etat");

                if (SelectedEvent?.Data.Data.SubEtat >= infoSubEtat.options.Count)
                    FieldUpdated(x => SelectedEvent.Data.Data.SubEtat = SelectedEvent.Data.Data.RuntimeSubEtat = x, (byte)0, () => SelectedEvent.Data.Data.SubEtat, "SubEtat");
            }

            // Helper method for updating a field
            void updateInputField<T>(InputField field, T value, Func<string, T> parser)
                where T : IComparable
            {
                T parsed = parser(field.text);

                if ((field.isFocused || EqualityComparer<T>.Default.Equals(parsed, value)) && !String.IsNullOrWhiteSpace(field.text) && PrevSelectedEvent == SelectedEvent) 
                    return;
                
                field.text = value.ToString();
            }

            // Helper method for updating a drop down
            void updateDropDown<T>(Dropdown field, T value)
                where T : IComparable
            {
                var selectedIndex = field.value;
                var currentIndex = field.options.FindIndex(x => x.text == value?.ToString());

                if (currentIndex == -1)
                    currentIndex = 0;

                if (selectedIndex == currentIndex && PrevSelectedEvent == SelectedEvent) 
                    return;
                
                field.value = currentIndex;
            }

            // Helper method for updating a toggle
            void updateToggle(Toggle field, bool value)
            {
                if (field.isOn == value && PrevSelectedEvent == SelectedEvent) 
                    return;
                
                field.isOn = value;
            }

            // X Position
            updateInputField<short>(infoX, (short)(SelectedEvent?.Data.Data.XPosition ?? -1), x => Int16.TryParse(x, out var r) ? r : (short)0);

            // Y Position
            updateInputField<short>(infoY, (short)(SelectedEvent?.Data.Data.YPosition ?? -1), x => Int16.TryParse(x, out var r) ? r : (short)0);

            // DES
            updateDropDown<string>(infoDes, SelectedEvent?.Data.DESKey);

            // ETA
            updateDropDown<string>(infoEta, SelectedEvent?.Data.ETAKey);

            // Etat
            updateDropDown<byte>(infoEtat, SelectedEvent?.Data.Data.Etat ?? 0);

            // SubEtat
            var seName = SelectedEvent != null ? EditorManager.ETANames?.TryGetItem(SelectedEvent.Data.ETAKey)?.ElementAtOrDefault(SelectedEvent.Data.Data.Etat)?.ElementAtOrDefault(SelectedEvent.Data.Data.SubEtat) : null;

            if (seName != null)
                updateDropDown<string>(infoSubEtat, seName);
            else
                updateDropDown<byte>(infoSubEtat, SelectedEvent?.Data.Data.SubEtat ?? 0);

            // OffsetBX
            updateInputField<byte>(infoOffsetBx, SelectedEvent?.Data.Data.OffsetBX ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);
            
            // OffsetBY
            updateInputField<byte>(infoOffsetBy, SelectedEvent?.Data.Data.OffsetBY ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // OffsetHY
            updateInputField<byte>(infoOffsetHy, SelectedEvent?.Data.Data.OffsetHY ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // FollowSprite
            updateInputField<byte>(infoFollowSprite, SelectedEvent?.Data.Data.FollowSprite ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // HitPoints
            updateInputField<uint>(infoHitPoints, SelectedEvent?.Data.Data.ActualHitPoints ?? 0, x => UInt32.TryParse(x, out var r) ? r : 0);

            // HitSprite
            updateInputField<byte>(infoHitSprite, SelectedEvent?.Data.Data.HitSprite ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // FollowEnabled
            updateToggle(infoFollow, SelectedEvent?.Data.Data.GetFollowEnabled(EditorManager.Settings) ?? false);

            // Type
            updateDropDown<Enum>(infoType, SelectedEvent?.Data.Type);

            // Set other fields
            infoName.text = SelectedEvent?.DisplayName ?? String.Empty;

            PrevSelectedEvent = SelectedEvent;
        }

        private void Update() 
        {
            if (!hasLoaded)
                return;

            // Update the fields
            UpdateEventFields();

            bool makingChanges = false;
            if (Settings.LoadFromMemory)
            {
                memoryLoadTimer += Time.deltaTime;
                if (memoryLoadTimer > 1.0f / 60.0f)
                {
                    makingChanges = UpdateFromMemory();
                    if(!makingChanges) memoryLoadTimer = 0.0f;
                }
            }
            else
            {
                GameMemoryContext?.Dispose();
                GameMemoryContext = null;
            }
            if (makingChanges) return;
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
                        var eventData = Controller.obj.levelController.EditorManager.AddEvent(eventDropdown.value, (short)mox, (short)-moy);

                        Controller.obj.levelController.currentLevel.EventData.Add(eventData);
                        var eve = AddEvent(eventData);

                        // Refresh the event
                        eve.RefreshEditorInfo();

                        Controller.obj.levelController.Events.Add(eve);
                    }
                }
                //Detect event under mouse when clicked
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) 
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    
                    var e = hit.collider?.GetComponentInParent<Common_Event>();
                    
                    if (e != null) 
                    {
                        if (SelectedEvent != null)
                            SelectedEvent.DisplayOffsets = false;

                        if (e != SelectedEvent) 
                        {
                            SelectedEvent = e;

                            // Change event info if event is selected
                            infoAnimIndex.text = SelectedEvent.Data.Data.RuntimeCurrentAnimIndex.ToString();
                            infoLayer.text = SelectedEvent.Data.Data.Layer.ToString();
                            
                            // Clear old commands
                            ClearCommands();

                            // Fill out the commands
                            foreach (var c in SelectedEvent.Data.CommandCollection?.Commands ?? new Common_EventCommand[0]) {
                                CommandLine cmd = Instantiate<GameObject>(prefabCommandLine, new Vector3(0,0,0), Quaternion.identity).GetComponent<CommandLine>();
                                cmd.command = c;
                                cmd.transform.SetParent(commandListParent, false);
                                commandLines.Add(cmd);
                            }
                        }

                        // Record selected position
                        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        selectedPosition = new Vector2(mousePos.x - e.transform.position.x, mousePos.y - e.transform.position.y);

                        // Update offset visibility
                        if (SelectedEvent != null)
                            SelectedEvent.DisplayOffsets = true;

                        // Change the link
                        if (modeLinks && SelectedEvent != Controller.obj.levelController.RaymanEvent && SelectedEvent != null) 
                        {
                            SelectedEvent.LinkID = 0;
                            SelectedEvent.ChangeLinksVisibility(true);
                        }
                    }
                    else 
                    {
                        if (SelectedEvent != null)
                            SelectedEvent.DisplayOffsets = false;

                        selectedLineRend.enabled = false;
                        SelectedEvent = null;

                        // Clear info window
                        ClearInfoWindow();
                    }
                }

                // Drag and move the event
                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) 
                {
                    if (SelectedEvent != null) 
                    {
                        // Move event if in event mode
                        if (modeEvents) {
                            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                            FieldUpdated(x => SelectedEvent.Data.Data.XPosition = x, (short)Mathf.Clamp(Mathf.RoundToInt((mousePos.x - selectedPosition.x) * 16), Int16.MinValue, Int16.MaxValue), () => SelectedEvent.Data.Data.XPosition, "XPos");
                            FieldUpdated(x => SelectedEvent.Data.Data.YPosition = x, (short)Mathf.Clamp(Mathf.RoundToInt(-(mousePos.y - selectedPosition.y) * 16), Int16.MinValue, Int16.MaxValue), () => SelectedEvent.Data.Data.YPosition, "YPos");
                        }

                        // Else move links
                        if (modeLinks && SelectedEvent != Controller.obj.levelController.RaymanEvent) {
                            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            SelectedEvent.linkCube.position = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
                        }
                    }
                }

                //Confirm links with mmb
                if (Input.GetMouseButtonDown(2) && modeLinks && SelectedEvent?.LinkID == 0)
                {
                    bool alone = true;
                    
                    foreach (Common_Event ee in Controller.obj.levelController.Events.
                        Where(ee => ee.linkCube.position == SelectedEvent.linkCube.position).
                        Where(ee => ee != SelectedEvent))
                    {
                        ee.LinkID = currentId;
                        ee.ChangeLinksVisibility(true);
                        ee.linkCubeLockPosition = ee.linkCube.position;
                        alone = false;
                    }

                    if (!alone) 
                    {
                        SelectedEvent.LinkID = currentId;
                        SelectedEvent.ChangeLinksVisibility(true);
                        SelectedEvent.linkCubeLockPosition = SelectedEvent.linkCube.position;
                    }
                    currentId++;
                }

                // Delete selected event
                if (Input.GetKeyDown(KeyCode.Delete) && modeEvents && SelectedEvent != null)
                {
                    if (SelectedEvent != null)
                        SelectedEvent.DisplayOffsets = false;

                    SelectedEvent.Delete();
                    SelectedEvent = null;
                    ClearInfoWindow();
                }
            }
            else 
            {
                selectedLineRend.enabled = false;
            }
        }

        public Context GameMemoryContext { get; set; }
        public R1MemoryData GameMemoryData { get; } = new R1MemoryData();

        // TODO: Move this to some main controller since we do tile stuff here too
        public bool UpdateFromMemory()
        {
            var lvl = Controller.obj.levelController.EditorManager.Level;
            bool madeEdits = false;

            // TODO: Dispose when we stop program?
            if (GameMemoryContext == null)
            {
                GameMemoryContext = new Context(Controller.CurrentSettings);

                try
                {
                    var file = new ProcessMemoryStreamFile("MemStream", Settings.ProcessName, GameMemoryContext);

                    GameMemoryContext.AddFile(file);

                    var offset = file.StartPointer;
                    var s = GameMemoryContext.Deserializer;

                    // Get the base pointer
                    var baseOffset = s.DoAt(offset + Settings.GameBasePointer, () => s.SerializePointer(default));
                    file.anchorOffset = baseOffset.AbsoluteOffset;
                    s.Goto(baseOffset);

                    GameMemoryData.Update(s);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    GameMemoryContext = null;
                }
            }

            if (GameMemoryContext != null)
            {
                Pointer currentOffset;
                SerializerObject s;

                void SerializeEvent(Editor_EventData ed)
                {
                    s = ed.HasPendingEdits ? (SerializerObject)GameMemoryContext.Serializer : GameMemoryContext.Deserializer;
                    s.Goto(currentOffset);
                    ed.Data.Init(s.CurrentPointer);
                    ed.Data.Serialize(s);
                    ed.DebugText = $"Pos: {ed.Data.XPosition}, {ed.Data.YPosition}{Environment.NewLine}" +
                                   $"RuntimePos: {ed.Data.RuntimeXPosition}, {ed.Data.RuntimeYPosition}{Environment.NewLine}" +
                                   $"Layer: {ed.Data.Layer}{Environment.NewLine}" +
                                   $"RuntimeLayer: {ed.Data.RuntimeLayer}{Environment.NewLine}" +
                                   $"{Environment.NewLine}" +
                                   $"Unk_24: {ed.Data.Unk_24}{Environment.NewLine}" +
                                   $"Unk_28: {ed.Data.Unk_28}{Environment.NewLine}" +
                                   $"Unk_32: {ed.Data.Unk_32}{Environment.NewLine}" +
                                   $"Unk_36: {ed.Data.Unk_36}{Environment.NewLine}" +
                                   $"{Environment.NewLine}" +
                                   $"Unk_48: {ed.Data.Unk_48}{Environment.NewLine}" +
                                   $"Unk_54: {ed.Data.Unk_54}{Environment.NewLine}" +
                                   $"Unk_56: {ed.Data.Unk_56}{Environment.NewLine}" +
                                   $"Unk_58: {ed.Data.Unk_58}{Environment.NewLine}" +
                                   $"{Environment.NewLine}" +
                                   $"Unk_64: {ed.Data.Unk_64}{Environment.NewLine}" +
                                   $"Unk_66: {ed.Data.Unk_66}{Environment.NewLine}" +
                                   $"{Environment.NewLine}" +
                                   $"Unk_74: {ed.Data.Unk_74}{Environment.NewLine}" +
                                   $"Unk_76: {ed.Data.Unk_76}{Environment.NewLine}" +
                                   $"Unk_78: {ed.Data.Unk_78}{Environment.NewLine}" +
                                   $"Unk_80: {ed.Data.Unk_80}{Environment.NewLine}" +
                                   $"Unk_82: {ed.Data.Unk_82}{Environment.NewLine}" +
                                   $"Unk_84: {ed.Data.Unk_84}{Environment.NewLine}" +
                                   $"Unk_86: {ed.Data.Unk_86}{Environment.NewLine}" +
                                   $"Unk_88: {ed.Data.Unk_88}{Environment.NewLine}" +
                                   $"Unk_90: {ed.Data.Unk_90}{Environment.NewLine}" +
                                   $"Unk_92: {ed.Data.Unk_92}{Environment.NewLine}" +
                                   $"Unk_94: {ed.Data.Unk_94}{Environment.NewLine}" +
                                   $"{Environment.NewLine}" +
                                   $"Flags: {Convert.ToString((byte)ed.Data.PC_Flags, 2).PadLeft(8, '0')}{Environment.NewLine}";
                    if (s is BinarySerializer)
                    {
                        Debug.Log($"Edited event");
                        madeEdits = true;
                    }

                    ed.HasPendingEdits = false;
                    currentOffset = s.CurrentPointer;
                }

                // Events
                if (GameMemoryData.EventArrayOffset != null)
                {
                    currentOffset = GameMemoryData.EventArrayOffset;
                    foreach (Editor_EventData ed in lvl.EventData)
                        SerializeEvent(ed);
                }

                // Rayman
                if (GameMemoryData.RayEventOffset != null)
                {
                    currentOffset = GameMemoryData.RayEventOffset;
                    SerializeEvent(lvl.Rayman);
                }

                // Tiles
                if (GameMemoryData.TileArrayOffset != null)
                {
                    currentOffset = GameMemoryData.TileArrayOffset;
                    var map = lvl.Maps[0];

                    for (int y = 0; y < map.Height; y++)
                    {
                        for (int x = 0; x < map.Width; x++)
                        {
                            var tileIndex = y * map.Width + x;
                            var mapTile = map.MapTiles[tileIndex];

                            s = mapTile.HasPendingEdits ? (SerializerObject)GameMemoryContext.Serializer : GameMemoryContext.Deserializer;

                            s.Goto(currentOffset);

                            var prevX = mapTile.Data.TileMapX;
                            var prevY = mapTile.Data.TileMapY;

                            mapTile.Data.Init(s.CurrentPointer);
                            mapTile.Data.Serialize(s);

                            if (s is BinarySerializer) 
                                madeEdits = true;
                            
                            mapTile.HasPendingEdits = false;

                            if (prevX != mapTile.Data.TileMapX || prevY != mapTile.Data.TileMapY)
                                Controller.obj.levelController.controllerTilemap.SetTileAtPos(x, y, mapTile);

                            currentOffset = s.CurrentPointer;

                            // On PC we need to also update the BigMap pointer table
                            if (GameMemoryData.BigMap != null && s is BinarySerializer) {
                                var pointerOffset = GameMemoryData.BigMap.MapTileTexturesPointersPointer + (4 * tileIndex);
                                var newPointer = GameMemoryData.BigMap.TileTexturesPointer + (lvl.Maps[0].PCTileOffsetTable[mapTile.Data.TileMapY]).SerializedOffset;
                                s.Goto(pointerOffset);

                                s.SerializePointer(newPointer);
                            }
                        }
                    }
                }
            }
            return madeEdits;
        }

        private void LateUpdate() 
        {
            // Update selection square lines
            if (SelectedEvent != null) 
            {
                selectedLineRend.SetPosition(0, new Vector2(SelectedEvent.midpoint.x - SelectedEvent.boxCollider.size.x / 2f, SelectedEvent.midpoint.y - SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(1, new Vector2(SelectedEvent.midpoint.x + SelectedEvent.boxCollider.size.x / 2f, SelectedEvent.midpoint.y - SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(2, new Vector2(SelectedEvent.midpoint.x + SelectedEvent.boxCollider.size.x / 2f, SelectedEvent.midpoint.y + SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(3, new Vector2(SelectedEvent.midpoint.x - SelectedEvent.boxCollider.size.x / 2f, SelectedEvent.midpoint.y + SelectedEvent.boxCollider.size.y / 2f));
                selectedLineRend.SetPosition(4, new Vector2(SelectedEvent.midpoint.x - SelectedEvent.boxCollider.size.x / 2f, SelectedEvent.midpoint.y - SelectedEvent.boxCollider.size.y / 2f));
            }
            else 
            {
                selectedLineRend.SetPosition(0, Vector2.zero);
                selectedLineRend.SetPosition(1, Vector2.zero);
                selectedLineRend.SetPosition(2, Vector2.zero);
                selectedLineRend.SetPosition(3, Vector2.zero);
                selectedLineRend.SetPosition(4, Vector2.zero);
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
            Common_Event newEvent = Instantiate(prefabEvent, new Vector3(eventData.Data.XPosition / 16f, -(eventData.Data.YPosition / 16f), eventData.Data.Layer), Quaternion.identity).GetComponent<Common_Event>();

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
