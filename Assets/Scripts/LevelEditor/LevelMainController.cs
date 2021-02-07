using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R1Engine
{
    [Serializable]
    public class UiTab {
        public GameObject open;
        public GameObject closed;
    }

    public class LevelMainController : MonoBehaviour 
    {
        // Object behaviors
        public List<Unity_ObjBehaviour> Objects { get; set; }
        public Unity_ObjBehaviour RaymanObject { get; set; }
        public IEnumerable<Unity_ObjBehaviour> GetAllObjects => RaymanObject != null ? Objects.Append(RaymanObject) : Objects;

        public LevelEditorBehaviour editor => controllerEvents.editor;

        // The context, to reuse when writing
        private Context serializeContext;

        // References to specific level controller gameObjects in inspector
        public LevelTilemapController controllerTilemap;
        public LevelEventController controllerEvents;

        //Ui tabs for showing/hiding them
        public UiTab[] tabs;

        /// <summary>
        /// The editor history
        /// </summary>
        public EditorHistory<Ray1MapEditorHistoryItem> History { get; set; }

        public async UniTask LoadLevelAsync(IGameManager manager, Context context) 
        {
            // Create the context
            serializeContext = context;

            // Make sure all the necessary files are downloaded
            Controller.LoadState = Controller.State.LoadingFiles;
            await manager.LoadFilesAsync(serializeContext);
            await Controller.WaitIfNecessary();

            using (serializeContext) {
                // Init editor data
                await LevelEditorData.InitAsync(context.Settings);
                await Controller.WaitIfNecessary();

                // Load the level
                Controller.LoadState = Controller.State.Loading;
                LevelEditorData.Level = await manager.LoadAsync(serializeContext, true);
                LevelEditorData.ShowEventsForMaps = LevelEditorData.Level.Maps.Select(x => true).ToArray();

                await Controller.WaitIfNecessary();
                if (Controller.LoadState == Controller.State.Error) return;

                Controller.LoadState = Controller.State.Initializing;
                await Controller.WaitIfNecessary();

                LevelEditorData.CurrentMap = LevelEditorData.Level.DefaultMap;
                LevelEditorData.CurrentCollisionMap = LevelEditorData.Level.DefaultCollisionMap;

                Controller.DetailedState = $"Initializing tile maps";
                await Controller.WaitIfNecessary();

                // Init tilemaps
                controllerTilemap.InitializeTilemaps();

                Controller.DetailedState = $"Initializing events";
                await Controller.WaitIfNecessary();

                // Add events
                Objects = LevelEditorData.Level.EventData.Select(x => controllerEvents.AddEvent(x)).ToList();

                if (LevelEditorData.Level.Rayman != null)
                    RaymanObject = controllerEvents.AddEvent(LevelEditorData.Level.Rayman);

                // Init event things
                controllerEvents.InitializeEvents();

                await Controller.WaitIfNecessary();

                // Set up history
                History = new EditorHistory<Ray1MapEditorHistoryItem>(x =>
                {
                    // Set tiles
                    foreach (var tileItem in x.ModifiedTiles)
                        controllerTilemap.SetTileAtPos(tileItem.XPos, tileItem.YPos, tileItem.Item);
                });

                if (Settings.ScreenshotEnumeration)
                    await ConvertLevelToPNGAsync();
            }
        }

        public async void SaveLevelTEMP() 
        {
            try
            {
                // Save link groups
                LevelEditorData.ObjManager.SaveLinkGroups(LevelEditorData.Level.EventData);

                // Save objects
                LevelEditorData.ObjManager.SaveObjects(LevelEditorData.Level.EventData);

                // Save level
                using (serializeContext)
                    await Settings.GetGameManager.SaveLevelAsync(serializeContext, LevelEditorData.Level);

                Debug.Log("Saved");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving level: {ex.Message}{Environment.NewLine}{ex}");
            }
        }

        public void ExportTileset() 
        {
            var tileSetIndex = 0;

            // Export every tile set
            foreach (var tileSet in LevelEditorData.Level.Maps[LevelEditorData.CurrentMap].TileSet.Where(x => x?.Tiles?.Any(y => y != null) == true))
            {
                // Get values
                var tileCount = tileSet.Tiles.Length;
                const int tileSetWidth = 16;
                var tileSetHeight = (int)Math.Ceiling(tileCount / (double)tileSetWidth);
                var tileSize = (int)tileSet.Tiles.First().rect.width;

                // Create the texture
                var tileTex = TextureHelpers.CreateTexture2D(tileSetWidth * tileSize, tileSetHeight * tileSize);

                // Default to fully transparent
                tileTex.SetPixels(Enumerable.Repeat(new Color(0, 0, 0, 0), tileTex.width * tileTex.height).ToArray());

                // Add every tile to it
                for (int i = 0; i < tileCount; i++)
                {
                    // Get the tile texture
                    var tile = tileSet.Tiles[i];

                    // Get the texture offsets
                    var offsetY = (i / tileSetWidth) * tileSize;
                    var offsetX = (i % tileSetWidth) * tileSize;

                    // Set the pixels
                    for (int y = 0; y < tile.rect.height; y++)
                    {
                        for (int x = 0; x < tile.rect.width; x++)
                        {
                            tileTex.SetPixel(x + offsetX, tileTex.height - (y + offsetY) - 1, tile.texture.GetPixel((int)tile.rect.x + x, (int)tile.rect.y + y));
                        }
                    }
                }

                tileTex.Apply();

                var destPath = $@"Tilemaps\{LevelEditorData.CurrentSettings.GameModeSelection}\{LevelEditorData.CurrentSettings.GameModeSelection} - {LevelEditorData.CurrentSettings.World} {LevelEditorData.CurrentSettings.Level:00} ({tileSetIndex}).png";

                Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                // Save the tile map
                File.WriteAllBytes(destPath, tileTex.EncodeToPNG());

                tileSetIndex++;
            }
        }

        public RectInt? ScreenshotRect { get; set; }
        public async void ConvertLevelToPNG() => await ConvertLevelToPNGAsync();
        public async UniTask ConvertLevelToPNGAsync() 
        {
            // Get the path to save to
            var destPath = Path.Combine("Screenshots", LevelEditorData.CurrentSettings.GameModeSelection.ToString());

            switch (Settings.Screenshot_FileName)
            {
                default:
                case ScreenshotName.Mode_World_Level:
                    destPath = Path.Combine(destPath, $"{LevelEditorData.CurrentSettings.GameModeSelection} - {LevelEditorData.CurrentSettings.World} {LevelEditorData.CurrentSettings.Level:00}.png");
                    break;

                case ScreenshotName.Mode_Level:
                    destPath = Path.Combine(destPath, $"{LevelEditorData.CurrentSettings.GameModeSelection} - {LevelEditorData.CurrentSettings.Level:00}.png");
                    break;

                case ScreenshotName.Engine_World_Level:
                    destPath = Path.Combine(destPath, $"{LevelEditorData.CurrentSettings.EngineVersion} - {LevelEditorData.CurrentSettings.World} {LevelEditorData.CurrentSettings.Level:00}.png");
                    break;

                case ScreenshotName.Engine_Level:
                    destPath = Path.Combine(destPath, $"{LevelEditorData.CurrentSettings.EngineVersion} - {LevelEditorData.CurrentSettings.Level:00}.png");
                    break;
            }

            // Create and write screenshot
            Util.ByteArrayToFile(destPath, await CreateLevelScreenshot());

            if (Settings.ScreenshotEnumeration)
            {
                foreach (var o in Objects)
                    Destroy(o);
                Objects.Clear();
                Destroy(controllerEvents);
                Destroy(controllerTilemap.tilemapFull);
                Destroy(controllerTilemap.tilemapPreview);
                Destroy(controllerTilemap);
                Destroy(Controller.obj);
                Destroy(this);
                LevelEditorData.Level = null;
                SceneManager.LoadScene("Dummy");
            }
        }

        public async UniTask<byte[]> CreateLevelScreenshot()
        {
            var onFinished = new List<Action>();

            // Force update event controller
            controllerEvents.ForceUpdate();

            // Hide unused links and show gendoors
            foreach (var e in Objects)
            {
                // Update the event to make sure it has rendered
                e.ForceUpdate();

                if (e.ObjData is Unity_Object_R1 r1Obj)
                {
                    Enum[] exceptions = {
                        R1_EventType.TYPE_GENERATING_DOOR,
                        R1_EventType.TYPE_DESTROYING_DOOR,
                        R1_EventType.MS_scintillement,
                        R1_EventType.MS_super_gendoor,
                        R1_EventType.MS_super_kildoor,
                        R1_EventType.MS_compteur,
                        R1_EventType.TYPE_RAY_POS,
                        R1_EventType.TYPE_INDICATOR,
                    };

                    if (exceptions.Contains(r1Obj.EventData.Type))
                    {
                        var wasActive = e.gameObject.activeSelf;
                        e.gameObject.SetActive(true);
                        onFinished.Add(() => e.gameObject.SetActive(wasActive));
                    }
                }
                else if (e.ObjData is Unity_Object_GBA gbaObj)
                {
                    if (gbaObj.Actor.ActorID == 0)
                    {
                        var wasActive = e.gameObject.activeSelf;
                        e.gameObject.SetActive(true);
                        onFinished.Add(() => e.gameObject.SetActive(wasActive));
                    }
                }

                // Hide events with no graphics
                if (e.defaultRenderer.gameObject.activeSelf && !Settings.Screenshot_ShowDefaultObj)
                {
                    var wasActive = e.gameObject.activeSelf;
                    e.gameObject.SetActive(false);
                    onFinished.Add(() => e.gameObject.SetActive(wasActive));
                }

                // TODO: Change this option
                // Helper method
                bool showLinksForObj(Unity_ObjBehaviour ee)
                {
                    if (ee.ObjData is Unity_Object_R1 r1Object)
                        return (r1Object.EventData.Type == R1_EventType.TYPE_GENERATING_DOOR ||
                                r1Object.EventData.Type == R1_EventType.TYPE_DESTROYING_DOOR ||
                                r1Object.EventData.Type == R1_EventType.MS_scintillement ||
                                r1Object.EventData.Type == R1_EventType.MS_super_gendoor ||
                                r1Object.EventData.Type == R1_EventType.MS_super_kildoor ||
                                r1Object.EventData.Type == R1_EventType.MS_compteur);

                    return ee.ObjData.EditorLinkGroup != 0;
                }

                if (e.ObjData.EditorLinkGroup == 0)
                {
                    var wasActive = e.lineRend.enabled;
                    e.lineRend.enabled = false;
                    e.linkCube.gameObject.SetActive(false);
                    onFinished.Add(() =>
                    {
                        e.lineRend.enabled = wasActive;
                        e.linkCube.gameObject.SetActive(wasActive);
                    });
                }
                else
                {
                    // Hide link if not linked to gendoor
                    bool gendoorFound = showLinksForObj(e);
                    var allofSame = new List<Unity_ObjBehaviour> {
                        e
                    };

                    foreach (Unity_ObjBehaviour f in Objects.Where(f => f.ObjData.EditorLinkGroup == e.ObjData.EditorLinkGroup))
                    {
                        allofSame.Add(f);
                        if (showLinksForObj(f))
                            gendoorFound = true;
                    }

                    if (!gendoorFound)
                    {
                        foreach (var a in allofSame)
                        {
                            var wasActive = a.lineRend.enabled;
                            a.lineRend.enabled = false;
                            a.linkCube.gameObject.SetActive(false);
                            onFinished.Add(() =>
                            {
                                a.lineRend.enabled = wasActive;
                                a.linkCube.gameObject.SetActive(wasActive);
                            });
                        }
                    }
                }
            }

            TransparencyCaptureBehaviour tcb = Camera.main.GetComponent<TransparencyCaptureBehaviour>();
            byte[] result = await tcb.CaptureFulllevel(false, ScreenshotRect);

            foreach (var a in onFinished)
                a?.Invoke();

            return result;
        }

        public void TabClicked(int tabIndex) {
            tabs[tabIndex].open.SetActive(!tabs[tabIndex].open.activeSelf);
            tabs[tabIndex].closed.SetActive(!tabs[tabIndex].closed.activeSelf);
        }

        public void Update()
        {
            var updatedSettings = false;

            if (Input.GetKeyDown(KeyCode.O))
            {
                Settings.ShowObjects = !Settings.ShowObjects;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                Settings.ShowTiles = !Settings.ShowTiles;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Settings.ShowCollision = !Settings.ShowCollision;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                Settings.ShowObjCollision = !Settings.ShowObjCollision;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Settings.AnimateSprites = !Settings.AnimateSprites;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Settings.AnimateTiles = !Settings.AnimateTiles;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                Settings.ShowAlwaysObjects = !Settings.ShowAlwaysObjects;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Settings.ShowEditorObjects = !Settings.ShowEditorObjects;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                Settings.ShowLinks = !Settings.ShowLinks;
                updatedSettings = true;
            }
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    Settings.ShowDebugInfo = !Settings.ShowDebugInfo;
            //    updatedSettings = true;
            //}
            if (Input.GetKeyDown(KeyCode.X))
            {
                Settings.ShowObjOffsets = !Settings.ShowObjOffsets;
                updatedSettings = true;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Settings.ShowRayman = !Settings.ShowRayman;
                updatedSettings = true;
            }

            if (Controller.obj.levelEventController.hasLoaded && LevelEditorData.Level?.IsometricData != null) {
                if (Input.GetKeyDown(KeyCode.F)) {
                    editor.cam.ToggleFreeCameraMode(!editor.cam.FreeCameraMode);
                    updatedSettings = true;
                }
            }

            if (updatedSettings)
            {
                Controller.obj.webCommunicator.SendSettings();
            }
        }
    }
}
