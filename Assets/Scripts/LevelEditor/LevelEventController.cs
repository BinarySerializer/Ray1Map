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
        public LinkTypeColor[] linkTypeColors;

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

            InitializeEventsForRayWikiScreenshot();

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

        public void InitializeEventsForRayWikiScreenshot()
        {
            if (!Settings.Screenshot_RayWikiMode)
                return;
            
            // Get common properties
            var objManager = LevelEditorData.ObjManager;
            var settings = objManager.Context.Settings;
            var objects = Controller.obj.levelController.Objects;
            var level = LevelEditorData.Level;
            var tilemapController = Controller.obj.levelController.controllerTilemap;

            if (settings.MajorEngineVersion == MajorEngineVersion.GBC)
            {
                // Object types to hide
                var hideTypes = new Dictionary<Game, byte?[]>()
                {
                    [Game.GBC_R1] = new GBC_R1_ActorID[]
                    {
                        GBC_R1_ActorID.LavaWater_0,
                        GBC_R1_ActorID.LavaWater_1,
                        GBC_R1_ActorID.Pilot,
                        GBC_R1_ActorID.FireStorm,
                        GBC_R1_ActorID.DarkBeam,
                        GBC_R1_ActorID.BigFireBall,
                        GBC_R1_ActorID.DarkBeam,
                        GBC_R1_ActorID.FireMine,
                        GBC_R1_ActorID.Lightning,
                    }.Select(x => (byte?)x).ToArray(),
                    [Game.GBC_R2] = new byte?[]
                    {
                        119, // Pirate shot
                        93, // Lava
                        96, // Pilot
                    }
                };

                // Object types to remove links for
                var removeLinksTypes = new Dictionary<Game, byte?[]>()
                {
                    [Game.GBC_R1] = new byte?[0],
                    [Game.GBC_R2] = new byte?[]
                    {
                        115, // Mask
                    }
                };

                // Objects to change the action for
                var actionChanges = new Dictionary<Game, Dictionary<byte?, byte>>()
                {
                    [Game.GBC_R1] = new Dictionary<byte?, byte>()
                    {
                        [0] = 2, // Rayman
                    },
                    [Game.GBC_R2] = new Dictionary<byte?, byte>()
                    {
                        [0] = 2, // Rayman
                        [118] = 3, // Pirate
                    }
                };

                // Hide specified types
                foreach (var obj in objects.Where(x => hideTypes[settings.Game].Contains(((Unity_Object_GBC)x.ObjData).Actor.ActorID)))
                    obj.IsEnabled = false;

                // Hide any object linked to Rayman
                var mainObj = objManager.GetMainObject(level.EventData);
                if (mainObj != null && (objManager.Context.Settings.Game == Game.GBC_R1 || objManager.Context.Settings.Game == Game.GBC_R2))
                {
                    foreach (var l in mainObj.Links)
                        objects[l].IsEnabled = false;
                }

                // Remove links for specified types
                foreach (var obj in objects.Select(x => (Unity_Object_GBC)x.ObjData).Where(x => removeLinksTypes[settings.Game].Contains(x.Actor.ActorID)))
                    obj.Actor.Links = new GBC_GameObjectLink[0];

                // Modify actor actions
                foreach (var obj in objects.Select(x => (Unity_Object_GBC)x.ObjData))
                {
                    if (obj.Actor.ActorID != null && actionChanges[settings.Game].ContainsKey(obj.Actor.ActorID))
                        obj.Actor.ActionID = actionChanges[settings.Game][obj.Actor.ActorID];
                }

                // Enumerate every captor and modify links
                foreach (var captor in objects.Where(x => ((Unity_Object_GBC)x.ObjData).IsTrigger))
                {
                    var newLinks = new List<GBC_GameObjectLink>();
                    var linkIndex = 0;

                    // Enumerate every link
                    foreach (var l in captor.ObjData.Links)
                    {
                        var linkedObj = (Unity_Object_GBC)objects[l].ObjData;

                        // If the linked object is a music activator we link what that in turn is linked to in Rayman 1
                        if (settings.Game == Game.GBC_R1 && linkedObj.Actor.ActorID == 98)
                        {
                            // Add the links
                            newLinks.AddRange(linkedObj.Actor.Links);

                            // Hide the object
                            objects[l].IsEnabled = false;
                        }
                        // If it links to a checkpoint we remove the link in Rayman 1
                        else if (settings.Game == Game.GBC_R1 && linkedObj.Actor.ActorID == 89)
                        {
                            // Do nothing
                        }
                        else
                        {
                            newLinks.Add(((Unity_Object_GBC)captor.ObjData).Actor.Links[linkIndex]);
                        }

                        linkIndex++;
                    }

                    // Update the links
                    ((Unity_Object_GBC)captor.ObjData).Actor.Links = newLinks.ToArray();

                    // If the captor isn't linked to anything which is enabled then we hide it
                    if (!captor.ObjData.Links.Any(x => objects[x].IsEnabled))
                        captor.IsEnabled = false;
                }
            }
            else if (settings.Game == Game.GBA_Rayman3)
            {
                var layerOverrides = new Dictionary<int, bool[]>()
                {
                    [5] = new bool[] {  }, // Garish Gears
                    [6] = new bool[] { false }, // Hoodlum Hideout 1
                    [7] = new bool[] { false }, // Hoodlum Hideout 2
                    [9] = new bool[] {  }, // Vertigo Wastes 1
                    [10] = new bool[] { true, true, true, false }, // Vertigo Wastes 2
                    [13] = new bool[] { false, true, true }, // Jano
                    [14] = new bool[] { false, true, true }, // Prickly Passage 1
                    [15] = new bool[] { false, true, true }, // Prickly Passage 2
                    [17] = new bool[] {  }, // River of Fire 1
                    [18] = new bool[] {  }, // River of Fire 2
                    [19] = new bool[] {  }, // River of Fire 3
                    [20] = new bool[] {  }, // The Underlands 1
                    [21] = new bool[] {  }, // The Underlands 2
                    [22] = new bool[] {  }, // Boulder Brink 1
                    [23] = new bool[] { true, true, true, false }, // Boulder Brink 2
                    [24] = new bool[] {  }, // Den of Rocky
                    [25] = new bool[] { false, true, true }, // Wretched Ruin 1
                    [26] = new bool[] { false, true, true }, // Wretched Ruin 2
                    [27] = new bool[] { false, true, true }, // Wicked Flow 1
                    [28] = new bool[] {  }, // Wicked Flow 2
                    [30] = new bool[] { false, true, true }, // Creeping Chaos 1
                    [31] = new bool[] { false, true, true }, // Creeping Chaos 2
                    [32] = new bool[] { false, true, true }, // Scaleman's Keep
                    [33] = new bool[] { false, true, true }, // The Mettleworks 1
                    [34] = new bool[] { false, true, true }, // The Mettleworks 2
                    [36] = new bool[] { false, true, true }, // Razor Slide 1
                    [37] = new bool[] { false, true, true }, // Razor Slide 2
                    [38] = new bool[] {  }, // Heart of the Ancients 1
                    [39] = new bool[] {  }, // Heart of the Ancients 2
                    [40] = new bool[] { false, true, true }, // Mega Havoc 1
                    [41] = new bool[] {  }, // Mega Havoc 2
                    [42] = new bool[] {  }, // Mega Havoc 3
                    [43] = new bool[] { false, true, true }, // Mega Havoc 4
                    [44] = new bool[] { false, true, true }, // Lum Challenge
                    [45] = new bool[] { false, true }, // Ly's Punch Challenge 1
                    [46] = new bool[] { false, true }, // Ly's Punch Challenge 2
                    [47] = new bool[] { false, true }, // Ly's Punch Challenge 3
                    [49] = new bool[] { false, true, true }, // Ly Power 2
                    [50] = new bool[] { false, true }, // Ly Power 3
                    [51] = new bool[] { false, true }, // Ly Power 4
                    [52] = new bool[] {  }, // Ly Power 5
                    [53] = new bool[] { false, true, true }, // Ly Power 6
                    [55] = new bool[] { false, true }, // Haunted Dreams
                    [56] = new bool[] { false, true, true }, // Magmacosm
                    [57] = new bool[] { false, true, true }, // Pirate Stronghold
                    [58] = new bool[] {  }, // World map

                    [100] = new bool[] { false }, // GCN 1
                    [102] = new bool[] {  }, // GCN 3
                    [104] = new bool[] { true, true, true, false }, // GCN 5
                    [105] = new bool[] {  }, // GCN 6
                    [107] = new bool[] { false, true, true }, // GCN 8
                    [108] = new bool[] {  }, // GCN 9
                    [109] = new bool[] {  }, // GCN 10
                };
                var tileLayers = new Dictionary<int, bool[]>()
                {
                    [9] = new bool[] { true }, // Vertigo Wastes 1
                    [10] = new bool[] { true }, // Vertigo Wastes 2
                    [19] = new bool[] { true }, // River of Fire 3
                    [20] = new bool[] { true }, // The Underlands 1
                    [21] = new bool[] { true }, // The Underlands 2
                    [22] = new bool[] { true }, // Boulder Brink 1
                    [23] = new bool[] { true }, // Boulder Brink 2
                    [28] = new bool[] { true }, // Wicked Flow 2
                    [38] = new bool[] { true }, // Heart of the Ancients 1
                    [41] = new bool[] { true }, // Mega Havoc 2
                    [42] = new bool[] { true }, // Mega Havoc 3
                    [52] = new bool[] { true }, // Ly Power 5
                    [102] = new bool[] { true }, // GCN 3
                    [104] = new bool[] { true }, // GCN 5
                    [105] = new bool[] { true }, // GCN 6
                    [108] = new bool[] { true }, // GCN 9
                    [109] = new bool[] { true }, // GCN 10
                };
                var texFrames = new Dictionary<int, RectInt>();

                // Version specific values
                if (settings.EngineVersion == EngineVersion.GBA_R3_NGage)
                {
                    layerOverrides.Add(8, new bool[] { false, true }); // Ly's Punch Challenge 1
                    layerOverrides.Add(16, new bool[] { false, true }); // Free Falling
                    layerOverrides.Add(35, new bool[] { false, true, true }); // Falling Down

                    layerOverrides.Add(60, new bool[] { false, true }); // N-Gage multiplayer 2
                    layerOverrides.Add(61, new bool[] { false, true, true }); // N-Gage multiplayer 3
                    layerOverrides.Add(62, new bool[] { false, true }); // N-Gage multiplayer 4
                    layerOverrides.Add(63, new bool[] { false, true }); // N-Gage multiplayer 5
                    layerOverrides.Add(65, new bool[] { false, true, true }); // N-Gage multiplayer 7
                    layerOverrides.Add(66, new bool[] { false, true }); // N-Gage multiplayer 8
                    layerOverrides.Add(67, new bool[] { false, true }); // N-Gage multiplayer 9
                    layerOverrides.Add(68, new bool[] { false, true, true }); // N-Gage multiplayer 10

                    texFrames.Add(38, new RectInt(0, 0, 184, 208)); // Heart of the Ancients 1
                    texFrames.Add(58, new RectInt(0, 0, 392, 208)); // World map
                }
                else
                {
                    layerOverrides.Add(4, new bool[] { true, false, false, false, false, false }); // Swamps 1
                    layerOverrides.Add(8, new bool[] { true, false, false }); // Magma Mayhem 1
                    layerOverrides.Add(16, new bool[] { true, false, false, false, false, false }); // Swamps 2
                    layerOverrides.Add(35, new bool[] { true, false, false }); // Magma Mayhem 2

                    layerOverrides.Add(59, new bool[] { true, false, false }); // Multiplayer 1
                    layerOverrides.Add(60, new bool[] { true, false, false }); // Multiplayer 2
                    layerOverrides.Add(61, new bool[] { false, true, true }); // Multiplayer 3
                    layerOverrides.Add(62, new bool[] { false, true }); // Multiplayer 4
                    layerOverrides.Add(63, new bool[] { false, true }); // Multiplayer 5
                    layerOverrides.Add(64, new bool[] { false, true, true }); // Multiplayer 6

                    texFrames.Add(5, new RectInt(0, 8, 688, 168 - 8)); // Garish Gears
                    texFrames.Add(24, new RectInt(0, 0, 248, 192)); // Den of Rocky
                    texFrames.Add(38, new RectInt(0, 0, 288, 160)); // Heart of the Ancients 1
                    texFrames.Add(58, new RectInt(0, 0, 464, 160)); // World map
                    texFrames.Add(60, new RectInt(0, 0, 800, 800)); // Multiplayer 2

                    texFrames.Add(101, new RectInt(0, 0, 8000, 240)); // GCN 2
                }

                if (settings.EngineVersion == EngineVersion.GBA_R3_Proto)
                {
                    layerOverrides[19] = new bool[] { false, true, true }; // River of Fire 3
                    tileLayers.Remove(19); // River of Fire 3

                    texFrames.Remove(24); // Den of Rocky

                    layerOverrides[45] = new bool[] { false, false, true }; // Ly's Punch Challenge 1
                    layerOverrides[50] = new bool[] { false, false, true }; // Ly Power 3
                }

                var lvl = settings.Level;

                if (settings.World == 9) // GCN
                    lvl += 100;

                if (settings.EngineVersion == EngineVersion.GBA_R3_Proto)
                {
                    // Two levels are swapped
                    if (settings.Level == 17 || settings.Level == 18 || settings.Level == 19)
                        lvl += 10;
                    else if (settings.Level == 27 || settings.Level == 28 || settings.Level == 29)
                        lvl -= 10;

                    // Ly's Punch Challenge 3 is missing
                    if (settings.Level > 46)
                        lvl++;
                }

                /*
                if (settings.World >= 9 || settings.Level == 103) // Use this to stop the screenshot enumeration
                    throw new Exception();
                */

                // Set screenshot rect
                if (texFrames.ContainsKey(lvl))
                    Controller.obj.levelController.ScreenshotRect = texFrames[lvl];

                // Show/hide layers
                if (!layerOverrides.ContainsKey(lvl))
                {
                    // Default to hide the first layers
                    tilemapController.IsLayerVisible[0] = false;
                    tilemapController.IsLayerVisible[1] = false;
                }
                else
                {
                    for (int i = 0; i < layerOverrides[lvl].Length; i++)
                        tilemapController.IsLayerVisible[i] = layerOverrides[lvl][i];
                }

                // Tile layers
                if (tileLayers.ContainsKey(lvl))
                {
                    for (int i = 0; i < tileLayers[lvl].Length; i++)
                        tilemapController.SetGraphicsLayerTiled(i, tileLayers[lvl][i]);
                }

                // Update tile map layers
                tilemapController.UpdateMapLayersVisibility();

                var hideObj = new GBA_R3_ActorID[]
                {
                    GBA_R3_ActorID.FloatingBarrelSplash,
                    GBA_R3_ActorID.PlumSplash,
                    GBA_R3_ActorID.Effect_Sparkles,
                    GBA_R3_ActorID.CaptureFlag_Flag,
                }.Select(x => (byte)x).ToArray();
                var stateModifications = new Tuple<GBA_R3_ActorID, byte, byte>[]
                {
                    new Tuple<GBA_R3_ActorID, byte, byte>(GBA_R3_ActorID.Cage, 5, 5),
                    new Tuple<GBA_R3_ActorID, byte, byte>(GBA_R3_ActorID.FloatingBarrel, 7, 2),
                    new Tuple<GBA_R3_ActorID, byte, byte>(GBA_R3_ActorID.Plum, 3, 5),
                    new Tuple<GBA_R3_ActorID, byte, byte>(GBA_R3_ActorID.Balloon, 0, 0),
                };
                var removeLinksTypes = new GBA_R3_ActorID[]
                {
                    GBA_R3_ActorID.Keg,
                    GBA_R3_ActorID.Boss_Machine,
                    GBA_R3_ActorID.Rayman_WaterSki,
                    GBA_R3_ActorID.Platform_Falling2,
                    GBA_R3_ActorID.Bridge_Falling,
                    GBA_R3_ActorID.Climbable_Falling,
                    GBA_R3_ActorID.RedShell,
                    GBA_R3_ActorID.Rayman_Flag2p,
                    GBA_R3_ActorID.Rayman_Flag4p,
                    GBA_R3_ActorID.CaptureFlag_Base,
                    GBA_R3_ActorID.CaptureFlag_Flag,
                }.Select(x => (byte)x).ToArray();

                // Enumerate every object
                foreach (var obj in objects)
                {
                    // Force update object
                    obj.ForceUpdate();

                    var gbaObj = (Unity_Object_GBA)obj.ObjData;

                    // Hide
                    if ((obj.ObjData.IsAlways && gbaObj.XPosition == 0 && gbaObj.YPosition == 0) || hideObj.Contains(gbaObj.Actor.ActorID))
                        obj.IsEnabled = false;

                    // Modify animation
                    var mod = stateModifications.FirstOrDefault(x => (byte)x.Item1 == gbaObj.Actor.ActorID && x.Item2 == gbaObj.Actor.ActionIndex);

                    if (mod != null)
                        gbaObj.OverrideAnimIndex = mod.Item3;

                    // Move object into view
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.Murfy && gbaObj.YPosition < 20)
                        gbaObj.YPosition += 20;
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.Pirate_Red && gbaObj.YPosition <= 87)
                        gbaObj.YPosition = 87;
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.Keg && gbaObj.YPosition <= 32)
                        gbaObj.YPosition = 32;
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.ChasingBoulder && gbaObj.YPosition <= 62)
                        gbaObj.YPosition = 62;
                    
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.Boss_Grolgoth && settings.EngineVersion == EngineVersion.GBA_R3_NGage && settings.Level == 38)
                    {
                        gbaObj.XPosition = 137;
                        gbaObj.YPosition = 90;
                    }
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.Boss_Grolgoth && settings.EngineVersion == EngineVersion.GBA_R3_NGage && settings.Level == 39)
                    {
                        gbaObj.XPosition = 380;
                        gbaObj.YPosition = 105;
                    }
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.Rayman_WorldMap && settings.EngineVersion == EngineVersion.GBA_R3_NGage && settings.Level == 58)
                    {
                        gbaObj.XPosition = 170;
                        gbaObj.YPosition = 115;
                    }
                    if (gbaObj.Actor.ActorID == (byte)GBA_R3_ActorID.Keg && settings.EngineVersion == EngineVersion.GBA_R3_NGage && settings.Level == 5)
                    {
                        gbaObj.XPosition = 122;
                        gbaObj.YPosition = 184;
                    }
                    
                    // Remove links
                    if (removeLinksTypes.Contains(gbaObj.Actor.ActorID))
                    {
                        gbaObj.Actor.Link_0 = 0xFF;
                        gbaObj.Actor.Link_1 = 0xFF;
                        gbaObj.Actor.Link_2 = 0xFF;
                        gbaObj.Actor.Link_3 = 0xFF;
                    }
                }
            }
            else if (settings.EngineVersion == EngineVersion.GBARRR && settings.World < 5)
            //else if (settings.EngineVersion == EngineVersion.GBARRR)
            {
                /*
                if (settings.World >= 5) // Use this to stop the screenshot enumeration
                    throw new Exception();
                */

                // Modify stompable pencils
                var pencils = objects.Select(x => (Unity_Object_GBARRR)x.ObjData).Where(x => x.Object.ObjectType == GBARRR_ObjectType.StompablePencil).ToArray();

                foreach (var p in pencils)
                {
                    // Modify the pencil frame
                    p.ForceFrame = 2;

                    // Create 5 duplicates
                    for (int i = 1; i < 6; i++)
                    {
                        objects.Add(AddEvent(new Unity_Object_GBARRR(new GBARRR_Object
                        {
                            YPosition = (short)(p.YPosition + (32 * i)),
                            XPosition = p.XPosition,
                            ObjectType = GBARRR_ObjectType.StompablePencil,
                            Data1 = new byte[4]
                        }, p.ObjManager)
                        {
                            AnimationGroupIndex = p.AnimationGroupIndex,
                            AnimIndex = p.AnimIndex,
                        }));
                    }
                }

                // Hide Murfy and Ly
                foreach (var o in objects.Where(x => ((Unity_Object_GBARRR)x.ObjData).Object.ObjectType == GBARRR_ObjectType.Friend))
                    o.IsEnabled = false;

                // Modify the frame for some animations
                var newFrames = new Dictionary<int, int>()
                {
                    [331] = 10,
                    [651] = 11,
                    [653] = 11,
                    [711] = 7,
                    [714] = 10,
                };

                foreach (var o in objects.Select(x => (Unity_Object_GBARRR)x.ObjData).Where(x => newFrames.ContainsKey(x.GraphicsData?.BlockIndex ?? -1)))
                    o.ForceFrame = newFrames[o.GraphicsData.BlockIndex];

                // Hide the background layers
                if (settings.Level != 29 && settings.Level != 31)
                {
                    tilemapController.IsLayerVisible[0] = false;
                    tilemapController.IsLayerVisible[1] = false;
                }
                else
                {
                    tilemapController.IsLayerVisible[1] = false;
                    tilemapController.IsLayerVisible[3] = false;
                    tilemapController.SetGraphicsLayerTiled(0, true);
                }

                // Update tile map layers
                tilemapController.UpdateMapLayersVisibility();

                // Set frames
                var texFrames = new Dictionary<int, RectInt>()
                {
                    [1] = new RectInt(0, 40, 1024, 672),
                    [6] = new RectInt(0, 200, 1024, 448),
                    [11] = new RectInt(0, 320, 1024, 448),
                    [14] = new RectInt(0, 0, 2496, 2016),
                    [15] = new RectInt(0, 768, 2176, 1248),
                    [16] = new RectInt(72, 184, 952, 392),
                    [22] = new RectInt(0, 320, 1024, 384),
                    [26] = new RectInt(0, 0, 3720, 2048),
                    [27] = new RectInt(24, 224, 944, 536),
                    [29] = new RectInt(0, 0, 405, 280),
                    [31] = new RectInt(0, 0, 405, 280),
                };
                
                if (texFrames.ContainsKey(settings.Level))
                    Controller.obj.levelController.ScreenshotRect = texFrames[settings.Level];

                // Remove links from editor objects
                foreach (var o in objects.Select(x => (Unity_Object_GBARRR)x.ObjData).Where(x => x.IsEditor))
                    o.Object.LinkGroup = 0;

                // Hide all editor objects while still showing one-way links
                foreach (var o in objects.Where(x => x.ObjData.IsEditor))
                    o.IsEnabled = false;

                // Force all one-way links to show
                foreach (var o in objects)
                    o.ForceShowOneWayLinks = true;

                // Handle Boss Prison
                if (settings.Level == 1)
                {
                    // Move the boss
                    objects[4].ObjData.XPosition = 640;
                    objects[4].ObjData.YPosition = 264;
                    ((Unity_Object_GBARRR)objects[4].ObjData).Object.Data1[3] = 0xFF;

                    // Change the rock to pink Rabbid
                    objects[3].ObjData.XPosition = 838;
                    objects[3].ObjData.YPosition = 280;
                    ((Unity_Object_GBARRR)objects[3].ObjData).AnimationGroupIndex = 47;
                    ((Unity_Object_GBARRR)objects[3].ObjData).AnimIndex = 2;
                    ((Unity_Object_GBARRR)objects[3].ObjData).Object.Data1[3] = 0xFF;
                }
            }
        }

        protected void InitializeEventLinks()
        {
            var objList = Controller.obj.levelController.Objects;

            Color GetColorForObject(Unity_ObjBehaviour obj, Unity_Object.LinkType? linkType) {
                Gizmo gizmo = gizmos.FirstOrDefault(g => g.name == obj.ObjData.Type.ToString());
                if (gizmo == null) gizmo = gizmos[0];
                Color c = gizmo.color;
                if (obj.ObjData.Type == Unity_Object.ObjectType.Object) {
                    if (linkType.HasValue && linkType.Value != Unity_Object.LinkType.Unknown) {
                        LinkTypeColor linkTypeColor = linkTypeColors.FirstOrDefault(ltc => ltc.linkType == linkType.Value);
                        if (linkTypeColor != null) {
                            c = linkTypeColor.color;
                        }
                    }
                }
                return new Color(c.r, c.g, c.b, 200/255f);
            }
            // foreach (var linkedActorIndex in obj.ObjData.Links)
            // Initialize one-way links
            foreach (var obj in objList.Where(x => x.ObjData.CanBeLinked))
            {
                var links = obj.ObjData.Links.ToArray();
                var linkCount = links.Length;
                var linkTypes = obj.ObjData.LinkTypes?.ToArray();

                obj.oneWayLinkLines = new LineRenderer[linkCount];
                obj.connectedOneWayLinkLines = new bool[linkCount];

                for (int i = 0; i < linkCount; i++)
                {
                    LineRenderer lr = new GameObject("OneWayLinkLine").AddComponent<LineRenderer>();
                    //lr.transform.SetParent(transform);
                    lr.gameObject.layer = LevelEditorData.Level?.IsometricData != null ? LayerMask.NameToLayer("3D Links") : LayerMask.NameToLayer("Links");
                    lr.sortingLayerName = "Links";
                    lr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                    lr.material = linkLineMaterial;
                    var linkedObj = Controller.obj.levelController.Objects[links[i]];
                    lr.material.color = GetColorForObject(linkedObj, linkTypes?.Length > i ? (Unity_Object.LinkType?)linkTypes[i] : null);
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

            foreach (var obj in Controller.obj.levelController.Objects.Where(x => x.HasInitialized && x.ObjData.CanBeLinked && x.connectedOneWayLinkLines != null))
            {
                var linkIndex = 0;
                foreach (var linkedActorIndex in obj.ObjData.Links)
                    obj.connectedOneWayLinkLines[linkIndex++] = Controller.obj.levelController.Objects[linkedActorIndex].EnableBoxCollider;
            }

            // Check for changed obj positions for one-way links
            bool updateLinks = Controller.obj.levelController.Objects.Any(x => x.HasInitialized && x.transform.position != ObjPositions[x]);
            if (updateLinks) {
                foreach (var obj in Controller.obj.levelController.Objects.Where(x => x.HasInitialized)) {
                    if(!obj.ObjData.CanBeLinked) continue;
                    var linkIndex = 0;

                    foreach (var linkedActorIndex in obj.ObjData.Links) {
                        var linkedObj = Controller.obj.levelController.Objects[linkedActorIndex];
                        var lr = obj.oneWayLinkLines[linkIndex];

                        if ((obj.transform.position != ObjPositions[obj] || linkedObj.transform.position != ObjPositions[linkedObj]) && obj.HasInitialized && linkedObj.HasInitialized) {
                            Vector3 origin = obj.midpoint;
                            Vector3 target = linkedObj.midpoint;

                            //Debug.Log($"Updated link arrow for actor {obj.Index} from {origin} to {target}");
                            float dist = Vector3.Distance(origin, target);
                            if (dist != 0) {
                                float AdaptiveSize = 0.5f / dist;
                                float threshold = 0.9f;
                                bool hasBackLink = linkedObj?.ObjData?.Links != null && linkedObj.ObjData.Links.Contains(linkIndex);
                                if (hasBackLink) {
                                    threshold = 0.25f;
                                }
                                if (AdaptiveSize < threshold) {
                                    lr.widthCurve = new AnimationCurve(
                                        new Keyframe(0, 0f),
                                        new Keyframe(hasBackLink ? AdaptiveSize / 2 : 0.001f, 0.095f),
                                        new Keyframe(0.999f - AdaptiveSize, 0.095f),  // neck of arrow
                                        new Keyframe(1 - AdaptiveSize, 0.5f), // max width of arrow head
                                        new Keyframe(1, 0f)); // tip of arrow
                                    lr.positionCount = 5;
                                    lr.SetPositions(new Vector3[] {
                                        origin,
                                        Vector3.Lerp(origin, target, hasBackLink ? AdaptiveSize / 2 : 0.001f),
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
                        }

                        linkIndex++;
                    }
                }
                // Update position
                foreach (var obj in Controller.obj.levelController.Objects.Where(x => x.HasInitialized)) {
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
                                Vector3 objectScale = LevelEditorData.Level.IsometricData.ObjectScale;
                                Vector3 isometricObjectScale = LevelEditorData.Level.IsometricData.AbsoluteObjectScale;
                                Vector3 scaledPos = Vector3.Scale(new Vector3(pos.x, pos.z, -pos.y), isometricObjectScale);
                                Vector3 transformOrigin = SelectedEvent.transform.position;
                                transformOrigin.y = selectedHeight;
                                //Debug.Log(transformOrigin);
                                Vector3 transformedSelectedPos = SelectedEvent.transform.rotation * selectedPosition + transformOrigin;
                                bool isDifferentPlane = editor.cam.camera3D.orthographic && editor.cam.camera3D.transform.rotation == Quaternion.identity;
                                if (isDifferentPlane) {
                                    // Move event on the plane at the selected position height
                                    Plane plane = new Plane(Vector3.forward, transformedSelectedPos);
                                    Ray ray = editor.cam.camera3D.ScreenPointToRay(Input.mousePosition);
                                    float dist;
                                    if (plane.Raycast(ray, out dist)) {
                                        Vector3 mouseWorldPos = ray.GetPoint(dist);
                                        Vector3 diff = transformedSelectedPos - transformOrigin;
                                        Vector3 scaledObjectPos = mouseWorldPos - diff;
                                        //scaledObjectPos.y = selectedHeight + addSelectedHeight;
                                        Vector3 unscaledPos = Vector3.Scale(scaledObjectPos, new Vector3(1f / isometricObjectScale.x, 1f / isometricObjectScale.y, 1f / isometricObjectScale.z));
                                        Vector3 newPos = new Vector3(unscaledPos.x, -unscaledPos.z, unscaledPos.y);
                                        obj.Position = newPos;
                                        //Debug.Log(mouseWorldPos + " - " + newPos);
                                    }
                                } else {
                                    // Move event on the plane at the selected position height
                                    Plane plane = new Plane(Vector3.up, transformedSelectedPos);
                                    Ray ray = editor.cam.camera3D.ScreenPointToRay(Input.mousePosition);
                                    float dist;
                                    if (plane.Raycast(ray, out dist)) {
                                        Vector3 mouseWorldPos = ray.GetPoint(dist);
                                        Vector3 diff = transformedSelectedPos - transformOrigin;
                                        Vector3 scaledObjectPos = mouseWorldPos - diff;
                                        scaledObjectPos.y = selectedHeight + addSelectedHeight;
                                        Vector3 unscaledPos = Vector3.Scale(scaledObjectPos, new Vector3(1f / isometricObjectScale.x, 1f / isometricObjectScale.y, 1f / isometricObjectScale.z));
                                        Vector3 newPos = new Vector3(unscaledPos.x, -unscaledPos.z, unscaledPos.y);
                                        obj.Position = newPos;
                                        //Debug.Log(mouseWorldPos + " - " + newPos);
                                    }
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
                                Vector3 isometricObjectScale = LevelEditorData.Level.IsometricData.AbsoluteObjectScale;
                                Vector3 scaledObjectPos = SelectedEvent.transform.position;
                                if (Input.GetKey(KeyCode.U)) {
                                    scaledObjectPos.y += isometricScale.y / 4f;
                                } else if (Input.GetKey(KeyCode.J)) {
                                    scaledObjectPos.y -= isometricScale.y / 4f;
                                }
                                Vector3 unscaledPos = Vector3.Scale(scaledObjectPos, new Vector3(1f / isometricObjectScale.x, 1f / isometricObjectScale.y, 1f / isometricObjectScale.z));
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

        public void ForceUpdate()
        {
            Update();
            LateUpdate();
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

        [Serializable]
        public class LinkTypeColor {
            public Unity_Object.LinkType linkType;
            public Color color;
        }
    }
}
