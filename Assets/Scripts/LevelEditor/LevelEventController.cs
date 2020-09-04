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
        public Unity_ObjBehaviour SelectedEvent { get; set; }
        public Unity_ObjBehaviour PrevSelectedEvent { get; set; }

        // Prefabs
        public GameObject eventParent;
        public GameObject prefabEvent;
        public GameObject prefabCommandLine;

        public LevelEditorBehaviour editor;

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

        public bool LogModifications => false;

        #endregion

        #region Field Changed Methods

        private void FieldUpdated<T>(Action<T> updateAction, T newValue, Func<T> currentValue, string logName)
            where T : IComparable
        {
            if (SelectedEvent != null && !newValue.Equals(currentValue()))
            {
                updateAction(newValue);
                SelectedEvent.ObjData.HasPendingEdits = true;

                if (LogModifications)
                    Debug.Log($"{logName} has been modified");
            }
        }

        public void FieldXPosition() => FieldUpdated(x => SelectedEvent.ObjData.XPosition = x, Int16.TryParse(infoX.text, out var v) ? v : (short)0, () => SelectedEvent.ObjData.XPosition, "XPos");
        public void FieldYPosition() => FieldUpdated(x => SelectedEvent.ObjData.YPosition = x, Int16.TryParse(infoY.text, out var v) ? v : (short)0, () => SelectedEvent.ObjData.YPosition, "YPos");
        [Obsolete]
        public void FieldDes() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.DES = x, infoDes.value, () => SelectedEvent.ObjData.LegacyWrapper.DES, "DES");
        [Obsolete]
        public void FieldEta() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.ETA = x, infoEta.value, () => SelectedEvent.ObjData.LegacyWrapper.ETA, "ETA");
        [Obsolete]
        public void FieldEtat() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.Etat = x, (byte)infoEtat.value, () => SelectedEvent.ObjData.LegacyWrapper.Etat, "Etat");
        [Obsolete]
        public void FieldSubEtat() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.SubEtat = x, (byte)infoSubEtat.value, () => SelectedEvent.ObjData.LegacyWrapper.SubEtat, "SubEtat");
        [Obsolete]
        public void FieldOffsetBx() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetBX = x;
        }, byte.TryParse(infoOffsetBx.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetBX : default, "BX");
        [Obsolete]
        public void FieldOffsetBy() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetBY = x;
        }, byte.TryParse(infoOffsetBy.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetBY : default, "BY");
        [Obsolete]
        public void FieldOffsetHy() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetHY = x;
        }, byte.TryParse(infoOffsetHy.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetHY : default, "HY");
        [Obsolete]
        public void FieldFollowSprite() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.FollowSprite = x;
        }, byte.TryParse(infoFollowSprite.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.FollowSprite : default, "FollowSprite");
        [Obsolete]
        public void FieldHitPoints() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.HitPoints = x, UInt32.TryParse(infoHitPoints.text, out var v) ? v : 0, () => SelectedEvent.ObjData.LegacyWrapper.HitPoints, "HitPoints");
        [Obsolete]
        public void FieldHitSprite() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.HitSprite = x, byte.TryParse(infoHitSprite.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData.LegacyWrapper.HitSprite, "HitSprite");
        [Obsolete]
        public void FieldFollowEnabled() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.FollowEnabled = x, infoFollow.isOn, () => SelectedEvent.ObjData.LegacyWrapper.FollowEnabled, "FollowEnabled");

        [Obsolete]
        public void FieldType() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.Type = (ushort)x, infoType.value, () => SelectedEvent.ObjData.LegacyWrapper.Type, "Type");

        public void FieldAnimIndex() => throw new NotImplementedException("The animation index can not be updated");
     
        #endregion

        public void InitializeEvents() 
        {
            LevelEditorData.ObjManager.InitEvents(LevelEditorData.Level);

            // Initialize links
            InitializeEventLinks();

            // Fill eventinfo dropdown with the event types
            infoType.options.AddRange(EnumHelpers.GetValues<R1_EventType>().Select(x => new Dropdown.OptionData
            {
                text = x.ToString()
            }));

            // Fill the dropdown menu
            eventDropdown.options = LevelEditorData.ObjManager.GetAvailableObjects.Select(x => new Dropdown.OptionData
            {
                text = x
            }).ToList();

            // Default to the first event
            eventDropdown.captionText.text = eventDropdown.options.FirstOrDefault()?.text;

            // Fill Des and Eta dropdowns with their max values
            infoDes.options = LevelEditorData.ObjManager.LegacyDESNames.Select(x => new Dropdown.OptionData(x)).ToList();
            infoEta.options = LevelEditorData.ObjManager.LegacyETANames.Select(x => new Dropdown.OptionData(x)).ToList();

            hasLoaded = true;
        }

        protected void InitializeEventLinks()
        {
            var eventList = Controller.obj.levelController.Events;

            // Initialize links
            LevelEditorData.ObjManager.InitLinkGroups(eventList.Select(x => x.ObjData).ToArray());

            // Set link positions
            foreach (var linkedEvents in eventList.Where(x => x.ObjData.EditorLinkGroup != 0).GroupBy(x => x.ObjData.EditorLinkGroup))
            {
                var prev = linkedEvents.Last();

                foreach (var e in linkedEvents)
                {
                    e.linkCube.position = prev.linkCube.position;
                    e.linkCubeLockPosition = e.linkCube.position;
                    prev = e;
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
            if (LevelEditorData.Level != null) {
                foreach (var eve in Controller.obj.levelController.GetAllEvents) {
                    if (editor.currentMode == LevelEditorBehaviour.EditMode.Links)
                        eve.ChangeLinksVisibility(true);
                }
            }
        }

        private float memoryLoadTimer = 0;

        private void UpdateEventFields()
        {
            var wrapper = SelectedEvent?.ObjData.LegacyWrapper;

            // Update Etat and SubEtat drop downs
            var etatLength = wrapper?.EtatLength ?? 0;
            var subEtatLength = wrapper?.SubEtatLength ?? 0;

            if (infoEtat.options.Count != etatLength)
            {
                // Clear old options
                infoEtat.options.Clear();

                // Set new options
                infoEtat.options.AddRange(Enumerable.Range(0, etatLength).Select(x => new Dropdown.OptionData
                {
                    text = x.ToString()
                }));
            }
            if (infoSubEtat.options.Count != subEtatLength)
            {
                // Clear old options
                infoSubEtat.options.Clear();

                // Set new options
                infoSubEtat.options.AddRange(Enumerable.Range(0, subEtatLength).Select(x => new Dropdown.OptionData
                {
                    text = x.ToString()
                }));
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
            void updateDropDownIndex(Dropdown field, int index)
            {
                var selectedIndex = field.value;

                if (index == -1)
                    index = 0;

                if (selectedIndex == index && PrevSelectedEvent == SelectedEvent) 
                    return;
                
                field.value = index;
            }

            // Helper method for updating a toggle
            void updateToggle(Toggle field, bool value)
            {
                if (field.isOn == value && PrevSelectedEvent == SelectedEvent) 
                    return;
                
                field.isOn = value;
            }

            // X Position
            updateInputField<short>(infoX, (short)(SelectedEvent?.ObjData.XPosition ?? -1), x => Int16.TryParse(x, out var r) ? r : (short)0);

            // Y Position
            updateInputField<short>(infoY, (short)(SelectedEvent?.ObjData.YPosition ?? -1), x => Int16.TryParse(x, out var r) ? r : (short)0);

            // DES
            updateDropDownIndex(infoDes, wrapper?.DES ?? 0);

            // ETA
            updateDropDownIndex(infoEta, wrapper?.ETA ?? 0);

            // Etat
            updateDropDownIndex(infoEtat, wrapper?.Etat ?? 0);

            // SubEtat
            updateDropDownIndex(infoSubEtat, wrapper?.SubEtat ?? 0);

            // OffsetBX
            updateInputField<byte>(infoOffsetBx, wrapper?.OffsetBX ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);
            
            // OffsetBY
            updateInputField<byte>(infoOffsetBy, wrapper?.OffsetBY ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // OffsetHY
            updateInputField<byte>(infoOffsetHy, wrapper?.OffsetHY ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // FollowSprite
            updateInputField<byte>(infoFollowSprite, wrapper?.FollowSprite ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // HitPoints
            updateInputField<uint>(infoHitPoints, wrapper?.HitPoints ?? 0, x => UInt32.TryParse(x, out var r) ? r : 0);

            // HitSprite
            updateInputField<byte>(infoHitSprite, wrapper?.HitSprite ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // FollowEnabled
            updateToggle(infoFollow, wrapper?.FollowEnabled ?? false);

            // Type
            updateDropDownIndex(infoType, wrapper?.Type ?? 0);

            // Set other fields
            infoName.text = SelectedEvent?.ObjData.DisplayName ?? String.Empty;

            PrevSelectedEvent = SelectedEvent;
        }

        public void SelectEvent(int index) {
            var events = Controller.obj.levelController.Events;
            if (index < 0 || index > events.Count) return;
            var e = events[index];
            SelectEvent(e);
        }

        public void SelectEvent(Unity_ObjBehaviour e) {
            if (SelectedEvent != e) {
                if (SelectedEvent != null) {
                    SelectedEvent.IsSelected = false;
                }
                SelectedEvent = e;

                // Change event info if event is selected
                //infoAnimIndex.text = SelectedEvent.Data.Data.RuntimeCurrentAnimIndex.ToString();
                infoLayer.text = SelectedEvent.Layer.ToString();

                // Clear old commands
                ClearCommands();

                if (SelectedEvent.ObjData is Unity_Object_R1 r1obj) {
                    // Fill out the commands
                    foreach (var c in r1obj.EventData.Commands?.Commands ?? new R1_EventCommand[0]) {
                        CommandLine cmd = Instantiate<GameObject>(prefabCommandLine, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<CommandLine>();
                        cmd.command = c;
                        cmd.transform.SetParent(commandListParent, false);
                        commandLines.Add(cmd);
                    }
                }
                SelectedEvent.IsSelected = true;
            }
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
            bool modeEvents = editor.currentMode == LevelEditorBehaviour.EditMode.Events;
            bool modeLinks = editor.currentMode == LevelEditorBehaviour.EditMode.Links;

            if ( modeEvents || modeLinks ) 
            {
                selectedLineRend.enabled = true;
                
                // Add events with mmb
                if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject() && modeEvents) 
                {
                    Vector2 mousepo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var mox = mousepo.x * LevelEditorData.Level.PixelsPerUnit;
                    var moy = mousepo.y * LevelEditorData.Level.PixelsPerUnit;

                    var maxWidth = LevelEditorData.MaxWidth;
                    var maxHeight = LevelEditorData.MaxHeight;

                    // Don't add if clicked outside of the level bounds
                    if (mox > 0 && -moy > 0 && mox < maxWidth * LevelEditorData.Level.CellSize && -moy < maxHeight * LevelEditorData.Level.CellSize) 
                    {
                        var eventData = LevelEditorData.ObjManager.CreateObject(eventDropdown.value);

                        eventData.XPosition = (short)mox;
                        eventData.YPosition = (short)-moy;

                        LevelEditorData.Level.EventData.Add(eventData);
                        var eve = AddEvent(eventData);

                        // Refresh the event
                        eve.RefreshEditorInfo();

                        Controller.obj.levelController.Events.Add(eve);
                    }
                }
                //Detect event under mouse when clicked
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) 
                {
                    int layerMask = 0;
                    layerMask |= 1 << LayerMask.NameToLayer("Object");
                    Unity_ObjBehaviour e = null;
                    RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
                    if (hits != null && hits.Length > 0) {
                        System.Array.Sort(hits, (x, y) => (x.distance.CompareTo(y.distance)));
                        for (int i = 0; i < hits.Length; i++) {
                            Unity_ObjBehaviour ob = hits[i].transform.GetComponentInParent<Unity_ObjBehaviour>();
                            if (ob != null) {
                                e = ob;
                                break;
                            }
                        }
                    }
                    
                    if (e != null) 
                    {
                        SelectEvent(e);

                        // Record selected position
                        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        selectedPosition = new Vector2(mousePos.x - e.transform.position.x, mousePos.y - e.transform.position.y);

                        // Change the link
                        if (modeLinks && SelectedEvent != Controller.obj.levelController.RaymanEvent && SelectedEvent != null) 
                        {
                            SelectedEvent.ObjData.EditorLinkGroup = 0;
                            SelectedEvent.ChangeLinksVisibility(true);
                        }
                    }
                    else 
                    {
                        if (SelectedEvent != null)
                            SelectedEvent.IsSelected = false;

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

                            FieldUpdated(x => SelectedEvent.ObjData.XPosition = x, (short)Mathf.Clamp(Mathf.RoundToInt((mousePos.x - selectedPosition.x) * LevelEditorData.Level.PixelsPerUnit), Int16.MinValue, Int16.MaxValue), () => SelectedEvent.ObjData.XPosition, "XPos");
                            FieldUpdated(x => SelectedEvent.ObjData.YPosition = x, (short)Mathf.Clamp(Mathf.RoundToInt(-(mousePos.y - selectedPosition.y) * LevelEditorData.Level.PixelsPerUnit), Int16.MinValue, Int16.MaxValue), () => SelectedEvent.ObjData.YPosition, "YPos");
                        }

                        // Else move links
                        if (modeLinks && SelectedEvent != Controller.obj.levelController.RaymanEvent) {
                            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            SelectedEvent.linkCube.position = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
                        }
                    }
                }

                //Confirm links with mmb
                if (Input.GetMouseButtonDown(2) && modeLinks && SelectedEvent?.ObjData.EditorLinkGroup == 0)
                {
                    bool alone = true;
                    
                    foreach (Unity_ObjBehaviour ee in Controller.obj.levelController.Events.
                        Where(ee => ee.linkCube.position == SelectedEvent.linkCube.position).
                        Where(ee => ee != SelectedEvent))
                    {
                        ee.ObjData.EditorLinkGroup = currentId;
                        ee.ChangeLinksVisibility(true);
                        ee.linkCubeLockPosition = ee.linkCube.position;
                        alone = false;
                    }

                    if (!alone) 
                    {
                        SelectedEvent.ObjData.EditorLinkGroup = currentId;
                        SelectedEvent.ChangeLinksVisibility(true);
                        SelectedEvent.linkCubeLockPosition = SelectedEvent.linkCube.position;
                    }
                    currentId++;
                }

                // Change frame with right and left arrows if not animation and not loading from memory
                if (SelectedEvent != null && !Settings.LoadFromMemory && !Settings.AnimateSprites)
                {
                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        var frame = SelectedEvent.ObjData.AnimationFrame - 1;

                        if (frame < 0)
                            frame = SelectedEvent.ObjData.CurrentAnimation.Frames.Length - 1;

                        SelectedEvent.ObjData.AnimationFrame = (byte)frame;
                        SelectedEvent.ObjData.AnimationFrameFloat = frame;
                    }

                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        var frame = SelectedEvent.ObjData.AnimationFrame + 1;

                        if (frame >= SelectedEvent.ObjData.CurrentAnimation.Frames.Length)
                            frame = 0;

                        SelectedEvent.ObjData.AnimationFrame = (byte)frame;
                        SelectedEvent.ObjData.AnimationFrameFloat = frame;
                    }
                }

                // Delete selected event
                if (Input.GetKeyDown(KeyCode.Delete) && modeEvents && SelectedEvent != null)
                {
                    if (SelectedEvent != null)
                        SelectedEvent.IsSelected = false;

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
            var lvl = LevelEditorData.Level;
            bool madeEdits = false;

            // TODO: Dispose when we stop program?
            if (GameMemoryContext == null)
            {
                GameMemoryContext = new Context(LevelEditorData.CurrentSettings);

                try
                {
                    var file = new ProcessMemoryStreamFile("MemStream", Settings.ProcessName, GameMemoryContext);

                    GameMemoryContext.AddFile(file);

                    var offset = file.StartPointer;
                    var basePtrPtr = offset + Settings.GameBasePointer;

                    if (Settings.FindPointerAutomatically)
                    {
                        try
                        {
                            basePtrPtr = file.GetPointerByName("MemBase"); // MemBase is the variable name in Dosbox.
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Couldn't find pointer automatically ({ex.Message}); falling back on manual specification {basePtrPtr}");
                        }
                    }

                    var s = GameMemoryContext.Deserializer;

                    // Get the base pointer
                    var baseOffset = s.DoAt(basePtrPtr, () => s.SerializePointer(default));
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

                void SerializeEvent(Unity_Object_R1 ed)
                {
                    s = ed.HasPendingEdits ? (SerializerObject)GameMemoryContext.Serializer : GameMemoryContext.Deserializer;
                    s.Goto(currentOffset);
                    ed.EventData.Init(s.CurrentPointer);
                    ed.EventData.Serialize(s);
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
                    foreach (var ed in lvl.EventData.OfType<Unity_Object_R1>())
                        SerializeEvent(ed);
                }

                // Rayman
                if (GameMemoryData.RayEventOffset != null && lvl.Rayman is Unity_Object_R1 r1Ray)
                {
                    currentOffset = GameMemoryData.RayEventOffset;
                    SerializeEvent(r1Ray);
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
            if (LevelEditorData.Level != null && areLinksVisible != t) {
                areLinksVisible = t;
                foreach (var e in Controller.obj.levelController.Events) {
                    e.ChangeLinksVisibility(t);
                }
            }
        }

        // Converts linkID to linkIndex when saving
        public void CalculateLinkIndexes() => LevelEditorData.ObjManager.SaveLinkGroups(LevelEditorData.Level.EventData);

        // Add events to the list via the managers
        public Unity_ObjBehaviour AddEvent(Unity_Object obj)
        {
            // Instantiate prefab
            Unity_ObjBehaviour newEvent = Instantiate(prefabEvent).GetComponent<Unity_ObjBehaviour>();
            newEvent.gameObject.name = obj.DisplayName;
            
            newEvent.ObjData = obj;
            newEvent.Index = LevelEditorData.Level.EventData.IndexOf(obj);

            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;

            // Add to list
            return newEvent;
        }
    }
}
