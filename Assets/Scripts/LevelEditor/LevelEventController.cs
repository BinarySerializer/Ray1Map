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
        public Unity_ObjBehaviour ClickedEvent { get; set; }

        public Dictionary<Unity_ObjBehaviour, Vector3?> ObjPositions { get; set; } = new Dictionary<Unity_ObjBehaviour, Vector3?>();

        // Prefabs
        public GameObject eventParent;
        public GameObject prefabEvent;
        public GameObject prefabCommandLine;

        public LevelEditorBehaviour editor;

        private Vector2 selectedPosition;
        private float selectedHeight;
        private float addSelectedHeight;
        public OutlineManager outlineManager;

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

        public Color linkColorActive;
        public Color linkColorDeactive;
        public Material linkLineMaterial;

        //Keeping track of used linkIds
        public int currentId = 1;

        public int lastUsedLayer = 0;

        public bool hasLoaded;

        public bool LogModifications => false;

        private bool updateLinkPos = false;

        public Gizmo[] gizmos;

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
        public void FieldDes() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.DES = x, infoDes.value, () => SelectedEvent.ObjData.LegacyWrapper.DES, "DES");
        public void FieldEta() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.ETA = x, infoEta.value, () => SelectedEvent.ObjData.LegacyWrapper.ETA, "ETA");
        public void FieldEtat() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.Etat = x, (byte)infoEtat.value, () => SelectedEvent.ObjData.LegacyWrapper.Etat, "Etat");
        public void FieldSubEtat() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.SubEtat = x, (byte)infoSubEtat.value, () => SelectedEvent.ObjData.LegacyWrapper.SubEtat, "SubEtat");
        public void FieldOffsetBx() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetBX = x;
        }, byte.TryParse(infoOffsetBx.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetBX : default, "BX");
        public void FieldOffsetBy() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetBY = x;
        }, byte.TryParse(infoOffsetBy.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetBY : default, "BY");
        public void FieldOffsetHy() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetHY = x;
        }, byte.TryParse(infoOffsetHy.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetHY : default, "HY");
        public void FieldFollowSprite() => FieldUpdated(x =>
        {
            if (SelectedEvent.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.FollowSprite = x;
        }, byte.TryParse(infoFollowSprite.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.FollowSprite : default, "FollowSprite");
        public void FieldHitPoints() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.HitPoints = x, UInt32.TryParse(infoHitPoints.text, out var v) ? v : 0, () => SelectedEvent.ObjData.LegacyWrapper.HitPoints, "HitPoints");
        public void FieldHitSprite() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.HitSprite = x, byte.TryParse(infoHitSprite.text, out var v) ? v : (byte)0, () => SelectedEvent.ObjData.LegacyWrapper.HitSprite, "HitSprite");
        public void FieldFollowEnabled() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.FollowEnabled = x, infoFollow.isOn, () => SelectedEvent.ObjData.LegacyWrapper.FollowEnabled, "FollowEnabled");

        public void FieldType() => FieldUpdated(x => SelectedEvent.ObjData.LegacyWrapper.Type = (ushort)x, infoType.value, () => SelectedEvent.ObjData.LegacyWrapper.Type, "Type");

        public void FieldAnimIndex() => throw new NotImplementedException("The animation index can not be updated");
     
        #endregion

        public void InitializeEvents() 
        {
            LevelEditorData.ObjManager.InitObjects(LevelEditorData.Level);

            // Initialize links
            InitializeEventLinks();

            // Fill eventinfo dropdown with the event types
            if (LevelEditorData.CurrentSettings.MajorEngineVersion == MajorEngineVersion.Rayman1)
            {
                infoType.options.AddRange(EnumHelpers.GetValues<R1_EventType>().Select(x => new Dropdown.OptionData
                {
                    text = x.ToString()
                }));
            }
            else
            {
                infoType.options.AddRange(Enumerable.Range(0, 1000).Select(x => new Dropdown.OptionData
                {
                    text = x.ToString()
                }));
            }

            // Fill the dropdown menu
            var objs = LevelEditorData.ObjManager.GetAvailableObjects;
            var list = new List<Dropdown.OptionData>();
            for (int i=0; i<objs.Length; i++) {
                var entry = new Dropdown.OptionData();

                if (LevelEditorData.ObjManager.IsObjectAlways(i)) {
                    entry.text = "A-" + objs[i];
                }
                else {
                    entry.text = objs[i];
                }

                list.Add(entry);
            }
            eventDropdown.options = list;
            //eventDropdown.options = LevelEditorData.ObjManager.GetAvailableObjects.Select(x => new Dropdown.OptionData {
            //    text = x               
            //}).ToList();

            // Default to the first event
            eventDropdown.captionText.text = eventDropdown.options.FirstOrDefault()?.text;

            // Fill Des and Eta dropdowns with their max values
            infoDes.options = LevelEditorData.ObjManager.LegacyDESNames.Select(x => new Dropdown.OptionData(x)).ToList();
            infoEta.options = LevelEditorData.ObjManager.LegacyETANames.Select(x => new Dropdown.OptionData(x)).ToList();

            hasLoaded = true;
        }

        protected void InitializeEventLinks()
        {
            var objList = Controller.obj.levelController.Objects;

            Color GetColorForObject(Unity_ObjBehaviour obj) {
                Gizmo gizmo = gizmos.FirstOrDefault(g => g.name == obj.ObjData.Type.ToString());
                if (gizmo == null) gizmo = gizmos[0];
                return new Color(gizmo.color.r, gizmo.color.g, gizmo.color.b, 200/255f);
            }
            // foreach (var linkedActorIndex in obj.ObjData.Links)
            // Initialize one-way links
            foreach (var obj in objList.Where(x => x.ObjData.CanBeLinked))
            {
                var links = obj.ObjData.Links.ToArray();
                var linkCount = links.Length;

                obj.oneWayLinkLines = new LineRenderer[linkCount];

                for (int i = 0; i < linkCount; i++)
                {
                    LineRenderer lr = new GameObject("OneWayLinkLine").AddComponent<LineRenderer>();
                    //lr.transform.SetParent(transform);
                    lr.gameObject.layer = LevelEditorData.Level?.IsometricData != null ? LayerMask.NameToLayer("3D Links") : LayerMask.NameToLayer("Links");
                    lr.sortingLayerName = "Links";
                    lr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                    lr.material = linkLineMaterial;
                    var linkedObj = Controller.obj.levelController.Objects[links[i]];
                    lr.material.color = GetColorForObject(linkedObj);
                    //lr.material.color = linkColorActive;
                    lr.positionCount = 2;
                    lr.widthMultiplier = 1f;
                    obj.oneWayLinkLines[i] = lr;
                }
            }

            // Initialize link groups
            currentId = LevelEditorData.ObjManager.InitLinkGroups(objList.Select(x => x.ObjData).ToArray());

            // Set link positions
            foreach (var linkedEvents in objList.Where(x => x.ObjData.EditorLinkGroup != 0 && x.ObjData.CanBeLinkedToGroup).GroupBy(x => x.ObjData.EditorLinkGroup))
            {
                var prev = linkedEvents.Last();

                foreach (var e in linkedEvents)
                {
                    e.linkCube.position = prev.linkCube.position;
                    e.linkCubeLockPosition = new Vector2(Mathf.FloorToInt(e.linkCube.position.x), Mathf.FloorToInt(e.linkCube.position.y));
                    prev = e;
                }
            }
        }

        private void Start() 
        {
            //Create empty list for commandlines
            commandLines = new List<CommandLine>();
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
            infoName.text = SelectedEvent?.ObjData.Name ?? String.Empty;

            PrevSelectedEvent = SelectedEvent;
        }

        private void SetSelectedPosition(Unity_ObjBehaviour e) {
            if (e.ObjData is Unity_Object_3D && LevelEditorData.Level.IsometricData != null) {
                Vector3 pos = e.transform.position;
                Plane plane = new Plane(e.transform.rotation * Vector3.forward, pos); // Object is facing the camera
                Ray ray = editor.cam.camera3D.ScreenPointToRay(Input.mousePosition);
                float dist;
                if (plane.Raycast(ray, out dist)) {
                    Vector3 clickedPos = ray.GetPoint(dist);
                    Vector3 localPos = e.transform.InverseTransformPoint(clickedPos);
                    selectedPosition = new Vector2(localPos.x, localPos.y);
                    selectedHeight = e.transform.position.y;
                    addSelectedHeight = 0f;
                }

            } else {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                selectedPosition = new Vector2(mousePos.x - e.transform.position.x, mousePos.y - e.transform.position.y);
                selectedHeight = e.transform.position.y;
                addSelectedHeight = 0f;
            }
        }

        private void ClickEvent(Unity_ObjBehaviour e) {
            bool modeEvents = editor.currentMode == LevelEditorBehaviour.EditMode.Events;
            bool modeLinks = editor.currentMode == LevelEditorBehaviour.EditMode.Links;
            if (e != null) {
                if (e != SelectedEvent) {
                    outlineManager.TransferHighlightToActiveColor();
                }
                SelectEvent(e);


                // Record selected position
                SetSelectedPosition(e);

                // Change the link
                if (modeLinks && SelectedEvent != Controller.obj.levelController.RaymanObject && SelectedEvent != null && SelectedEvent.ObjData.CanBeLinkedToGroup) {
                    
                    //If someone is left alone in a group, unlink them too
                    var objList = Controller.obj.levelController.Objects;
                    var allLinkedEvents = objList.Where(x => x.ObjData.EditorLinkGroup == SelectedEvent.ObjData.EditorLinkGroup).ToList();
                    
                    if (allLinkedEvents.Count <= 2) {
                        foreach (var ev in allLinkedEvents) {
                            ev.ObjData.EditorLinkGroup = 0;
                        }
                    }

                    //Unlink self
                    SelectedEvent.ObjData.EditorLinkGroup = 0;
                }
            } else {
                if (SelectedEvent != null)
                    SelectedEvent.IsSelected = false;

                outlineManager.Active = null;
                SelectedEvent = null;

                // Clear info window
                ClearInfoWindow();
            }
        }

        public void SelectEvent(int index, bool moveCamera = false) {
            if (index == -1) {
                SelectEvent(Controller.obj.levelController.RaymanObject, moveCamera);
            } else {
                var events = Controller.obj.levelController.Objects;
                if (index < 0 || index > events.Count) return;
                var e = events[index];
                SelectEvent(e, moveCamera);
            }
        }

        public void SelectEvent(Unity_ObjBehaviour e, bool moveCamera = false)
        {
            // Return if the event is already selected
            if (SelectedEvent == e) 
                return;

            // Set previously selected event to not be selected
            if (SelectedEvent != null)
                SelectedEvent.IsSelected = false;

            // Updated selected event
            SelectedEvent = e;

            // Change event info if event is selected
            infoAnimIndex.text = SelectedEvent.ObjData.AnimationIndex.ToString();
            infoLayer.text = SelectedEvent.Layer.ToString();

            // Clear old commands
            ClearCommands();

            if (FileSystem.mode == FileSystem.Mode.Normal) {
                if (SelectedEvent.ObjData is Unity_Object_R1 r1obj) {
                    // Fill out the commands
                    foreach (var c in r1obj.EventData.Commands?.Commands ?? new R1_EventCommand[0]) {
                        CommandLine cmd = Instantiate<GameObject>(prefabCommandLine, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<CommandLine>();
                        cmd.command = c;
                        cmd.transform.SetParent(commandListParent, false);
                        commandLines.Add(cmd);
                    }
                }
            }

            if (moveCamera) {
                editor.cam.JumpTo(SelectedEvent.gameObject);
                //Controller.obj.levelEventController.editor.cam.pos = new Vector3(SelectedEvent.ObjData.XPosition / (float)LevelEditorData.Level.PixelsPerUnit, -(SelectedEvent.ObjData.YPosition / (float)LevelEditorData.Level.PixelsPerUnit));
            }
            SelectedEvent.IsSelected = true;
            //Debug.Log("GenId is: " + SelectedEvent.ObjData.R1_EditorLinkGroup.ToString());
        }

        public Context GameMemoryContext;

        private void Update() 
        {
            if (!hasLoaded)
                return;

            // Update the fields
            UpdateEventFields();

            // Check for changed obj positions for one-way links
            bool updateLinks = Controller.obj.levelController.Objects.Any(x => x.HasInitialized && x.ObjData.CanBeLinked && x.transform.position != ObjPositions[x]);
            if (updateLinks) {
                var linkableObjects = Controller.obj.levelController.Objects.Where(x => x.HasInitialized && x.ObjData.CanBeLinked);
                foreach (var obj in linkableObjects) {
                    var linkIndex = 0;

                    foreach (var linkedActorIndex in obj.ObjData.Links) {
                        var linkedObj = Controller.obj.levelController.Objects[linkedActorIndex];
                        var lr = obj.oneWayLinkLines[linkIndex];

                        if ((obj.transform.position != ObjPositions[obj] || linkedObj.transform.position != ObjPositions[linkedObj]) && obj.HasInitialized && linkedObj.HasInitialized) {
                            Vector3 origin = obj.midpoint;
                            Vector3 target = linkedObj.midpoint;

                            //Debug.Log($"Updated link arrow for actor {obj.Index} from {origin} to {target}");

                            float AdaptiveSize = 0.5f / Vector3.Distance(origin, target);
                            if (AdaptiveSize < 0.25f) {
                                lr.widthCurve = new AnimationCurve(
                                    new Keyframe(0, 0f),
                                    new Keyframe(AdaptiveSize / 2, 0.095f),
                                    new Keyframe(0.999f - AdaptiveSize, 0.095f),  // neck of arrow
                                    new Keyframe(1 - AdaptiveSize, 0.5f), // max width of arrow head
                                    new Keyframe(1, 0f)); // tip of arrow
                                lr.positionCount = 5;
                                lr.SetPositions(new Vector3[] {
                                    origin,
                                    Vector3.Lerp(origin, target, AdaptiveSize / 2),
                                    Vector3.Lerp(origin, target, 0.999f - AdaptiveSize),
                                    Vector3.Lerp(origin, target, 1 - AdaptiveSize),
                                    target });
                            } else {
                                lr.widthCurve = new AnimationCurve(
                                    new Keyframe(0, 0.095f),
                                    new Keyframe(1, 0.095f)); // tip of arrow
                                lr.positionCount = 2;
                                lr.SetPositions(new Vector3[] { origin, target });
                            }
                        }

                        linkIndex++;
                    }
                }
                // Update position
                foreach (var obj in linkableObjects) {
                    ObjPositions[obj] = obj.transform.position;
                }
            }

            bool makingChanges = false;
            if (Settings.LoadFromMemory)
            {
                memoryLoadTimer += Time.deltaTime;
                if (memoryLoadTimer > 1.0f / 60.0f)
                {
                    makingChanges = LevelEditorData.ObjManager.UpdateFromMemory(ref GameMemoryContext);
                    if(!makingChanges) 
                        memoryLoadTimer = 0.0f;
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
            bool modeR1Links = editor.currentMode == LevelEditorBehaviour.EditMode.Links && LevelEditorData.CurrentSettings.MajorEngineVersion != MajorEngineVersion.GBA && FileSystem.mode != FileSystem.Mode.Web;
            bool modeGBALinks = editor.currentMode == LevelEditorBehaviour.EditMode.Links && LevelEditorData.CurrentSettings.MajorEngineVersion == MajorEngineVersion.GBA && FileSystem.mode != FileSystem.Mode.Web;

            bool lctrl = Input.GetKey(KeyCode.LeftControl);

            outlineManager.Highlight = editor.objectHighlight.highlightedObject;
            outlineManager.Active = SelectedEvent;
            if ( modeEvents || modeR1Links ) 
            {
                outlineManager.Active = SelectedEvent;
                // Add events with ctrl+lmb
                if (lctrl && Input.GetMouseButtonDown(0) && LevelEditorData.Level.IsometricData == null && !EventSystem.current.IsPointerOverGameObject() && modeEvents && FileSystem.mode != FileSystem.Mode.Web) 
                {
                    Vector2 mousepo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var mox = mousepo.x * LevelEditorData.Level.PixelsPerUnit;
                    var moy = mousepo.y * LevelEditorData.Level.PixelsPerUnit;

                    var maxWidth = LevelEditorData.MaxWidth;
                    var maxHeight = LevelEditorData.MaxHeight;

                    // Don't add if clicked outside of the level bounds
                    if (mox > 0 && -moy > 0 && mox < maxWidth * LevelEditorData.Level.CellSize && -moy < maxHeight * LevelEditorData.Level.CellSize) 
                    {
                        // Make sure we haven't exceeded the max count or are the main event
                        if (LevelEditorData.Level.EventData.Count < LevelEditorData.ObjManager.MaxObjectCount)
                        {
                            var eventData = LevelEditorData.ObjManager.CreateObject(eventDropdown.value);

                            if (eventData != null)
                            {
                                eventData.XPosition = (short)(mox - eventData.Pivot.x);
                                eventData.YPosition = (short)(-moy + eventData.Pivot.y);

                                LevelEditorData.Level.EventData.Add(eventData);
                                var eve = AddEvent(eventData);

                                Controller.obj.levelController.Objects.Add(eve);
                            }
                        }
                    }
                }
                //Detect event under mouse when clicked
                if (!lctrl && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) 
                {
                    var e = editor.objectHighlight.highlightedObject;
                    if (e != null) ClickedEvent = e;
                    if (FileSystem.mode != FileSystem.Mode.Web) {
                        ClickEvent(e);
                    } else {
                        if (e != null) {
                            // Record selected position
                            SetSelectedPosition(e);
                        } else {
                            ClickEvent(e);
                        }
                    }
                }
                if (ClickedEvent != null) {
                    if (FileSystem.mode == FileSystem.Mode.Web) {
                        if (!lctrl && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
                            var e = editor.objectHighlight.highlightedObject;
                            ClickEvent(ClickedEvent);
                        }
                    }
                }

                // Drag and move the event
                if (!lctrl && Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    if (SelectedEvent != null && SelectedEvent == ClickedEvent) {
                        if (SelectedEvent.ObjData is Unity_Object_3D && LevelEditorData.Level.IsometricData != null) {
                            if (modeEvents) {
                                Unity_Object_3D obj = (Unity_Object_3D)SelectedEvent.ObjData;

                                Vector3 pos = obj.Position;
                                Vector3 isometricScale = LevelEditorData.Level.IsometricData.Scale;
                                if (Input.GetKey(KeyCode.U)) {
                                    addSelectedHeight += isometricScale.y / 4f;
                                } else if (Input.GetKey(KeyCode.J)) {
                                    addSelectedHeight -= isometricScale.y / 4f;
                                }
                                Vector3 scaledPos = Vector3.Scale(new Vector3(pos.x, pos.z, -pos.y) / 16f, isometricScale);
                                Vector3 transformOrigin = SelectedEvent.transform.position;
                                transformOrigin.y = selectedHeight;
                                //Debug.Log(transformOrigin);
                                Vector3 transformedSelectedPos = SelectedEvent.transform.rotation * selectedPosition + transformOrigin;

                                // Move event on the plane at the selected position height
                                Plane plane = new Plane(Vector3.up, transformedSelectedPos);
                                Ray ray = editor.cam.camera3D.ScreenPointToRay(Input.mousePosition);
                                float dist;
                                if (plane.Raycast(ray, out dist)) {
                                    Vector3 mouseWorldPos = ray.GetPoint(dist);
                                    Vector3 diff = transformedSelectedPos - transformOrigin;
                                    Vector3 scaledObjectPos = mouseWorldPos - diff;
                                    scaledObjectPos.y = selectedHeight + addSelectedHeight;
                                    Vector3 unscaledPos = Vector3.Scale(scaledObjectPos, 16 * new Vector3(1f / isometricScale.x, 1f / isometricScale.y, 1f / isometricScale.z));
                                    Vector3 newPos = new Vector3(unscaledPos.x, -unscaledPos.z, unscaledPos.y);
                                    obj.Position = newPos;
                                    //Debug.Log(mouseWorldPos + " - " + newPos);
                                }
                            }
                        } else {
                            // Move event if in event mode
                            if (modeEvents) {
                                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                                FieldUpdated(x => SelectedEvent.ObjData.XPosition = x, (short)Mathf.Clamp(Mathf.RoundToInt((mousePos.x - selectedPosition.x) * LevelEditorData.Level.PixelsPerUnit), Int16.MinValue, Int16.MaxValue), () => SelectedEvent.ObjData.XPosition, "XPos");
                                FieldUpdated(x => SelectedEvent.ObjData.YPosition = x, (short)Mathf.Clamp(Mathf.RoundToInt(-(mousePos.y - selectedPosition.y) * LevelEditorData.Level.PixelsPerUnit), Int16.MinValue, Int16.MaxValue), () => SelectedEvent.ObjData.YPosition, "YPos");

                                updateLinkPos = true;
                            }

                            // Else move links
                            if (modeR1Links && SelectedEvent != Controller.obj.levelController.RaymanObject) {
                                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                SelectedEvent.linkCube.position = new Vector2(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
                            }
                        }
                    }
                } else if(!Input.GetMouseButton(0) && (Input.GetKey(KeyCode.U) || Input.GetKey(KeyCode.J))) {
                    // For 3D objects, allow adjusting height outside selection
                    if (SelectedEvent != null) {
                        if (SelectedEvent.ObjData is Unity_Object_3D && LevelEditorData.Level.IsometricData != null) {
                            if (modeEvents) {
                                Unity_Object_3D obj = (Unity_Object_3D)SelectedEvent.ObjData;
                                Vector3 isometricScale = LevelEditorData.Level.IsometricData.Scale;
                                Vector3 scaledObjectPos = SelectedEvent.transform.position;
                                if (Input.GetKey(KeyCode.U)) {
                                    scaledObjectPos.y += isometricScale.y / 4f;
                                } else if (Input.GetKey(KeyCode.J)) {
                                    scaledObjectPos.y -= isometricScale.y / 4f;
                                }
                                Vector3 unscaledPos = Vector3.Scale(scaledObjectPos, 16 * new Vector3(1f / isometricScale.x, 1f / isometricScale.y, 1f / isometricScale.z));
                                Vector3 newPos = new Vector3(unscaledPos.x, -unscaledPos.z, unscaledPos.y);
                                obj.Position = newPos;
                            }
                        }
                    }
                }

                //Confirm links with ctrl+lmb
                if (lctrl && Input.GetMouseButtonDown(0) && modeR1Links && SelectedEvent?.ObjData.EditorLinkGroup == 0)
                {
                    bool alone = true;
                    
                    foreach (Unity_ObjBehaviour ee in Controller.obj.levelController.Objects.
                        Where(ee => ee.linkCube.position == SelectedEvent.linkCube.position).
                        Where(ee => ee != SelectedEvent))
                    {
                        ee.ObjData.EditorLinkGroup = currentId;
                        ee.linkCubeLockPosition = ee.linkCube.position;
                        alone = false;
                    }

                    if (!alone) 
                    {
                        SelectedEvent.ObjData.EditorLinkGroup = currentId;
                        SelectedEvent.linkCubeLockPosition = SelectedEvent.linkCube.position;
                    }
                    currentId++;
                }

                // Change frame with right and left arrows if not animation and not loading from memory
                if (SelectedEvent != null && !Settings.LoadFromMemory && !Settings.AnimateSprites)
                {
                    if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                    {
                        var frame = SelectedEvent.ObjData.AnimationFrame - 1;

                        if (frame < 0)
                            frame = SelectedEvent.ObjData.CurrentAnimation.Frames.Length - 1;

                        SelectedEvent.ObjData.AnimationFrame = (byte)frame;
                        SelectedEvent.ObjData.AnimationFrameFloat = frame;
                    }

                    if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                    {
                        var frame = SelectedEvent.ObjData.AnimationFrame + 1;

                        if (frame >= SelectedEvent.ObjData.CurrentAnimation.Frames.Length)
                            frame = 0;

                        SelectedEvent.ObjData.AnimationFrame = (byte)frame;
                        SelectedEvent.ObjData.AnimationFrameFloat = frame;
                    }
                }

                // Delete selected event
                if (Input.GetKeyDown(KeyCode.Delete) && 
                    modeEvents && 
                    SelectedEvent != null && 
                    Application.platform != RuntimePlatform.WebGLPlayer &&
                    SelectedEvent != Controller.obj.levelController.RaymanObject)
                {
                    if (SelectedEvent != null)
                        SelectedEvent.IsSelected = false;

                    SelectedEvent.Delete();
                    SelectedEvent = null;
                    ClearInfoWindow();
                }
            } else {
                outlineManager.Active = null;
            }
            if (ClickedEvent != null && (!Input.GetMouseButton(0) || !(modeEvents || modeR1Links))) {
                ClickedEvent = null;
            }
            outlineManager.selecting = ClickedEvent != null;
        }

        private void LateUpdate() {
            if (updateLinkPos) {
                updateLinkPos = false;
                var e = editor.objectHighlight.highlightedObject;
                if (e != null) {
                    if (SelectedEvent != null) {
                        if (SelectedEvent?.ObjData.EditorLinkGroup == 0)
                            SelectedEvent.linkCube.position = new Vector2(Mathf.FloorToInt(e.transform.position.x), Mathf.FloorToInt(e.transform.position.y));
                    }
                }
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

        // Add events to the list via the managers
        public Unity_ObjBehaviour AddEvent(Unity_Object obj)
        {
            // Instantiate prefab
            Unity_ObjBehaviour newEvent = Instantiate(prefabEvent).GetComponent<Unity_ObjBehaviour>();
            newEvent.gameObject.name = obj.Name;
            
            newEvent.ObjData = obj;
            newEvent.Index = LevelEditorData.Level.EventData.IndexOf(obj);

            // Set as child of events gameobject
            newEvent.gameObject.transform.parent = eventParent.transform;
            newEvent.Init();

            // Default position to null
            ObjPositions[newEvent] = null;
            
            // Add to list
            return newEvent;
        }

        [Serializable]
        public class Gizmo {
            public string name;
            public Sprite sprite;
            public Sprite sprite3D;
            public Color color;
        }
    }
}
