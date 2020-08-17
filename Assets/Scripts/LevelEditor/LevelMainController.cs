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

    public class LevelMainController : MonoBehaviour {

        /// <summary>
        /// The events
        /// </summary>
        public List<Common_Event> Events { get; set; }

        public Common_Event RaymanEvent { get; set; }

        public IEnumerable<Common_Event> GetAllEvents => RaymanEvent != null ? Events.Append(RaymanEvent) : Events;

        public Editor editor => controllerEvents.editor;

        // The context, to reuse when writing
        private Context serializeContext;

        // References to specific level controller gameObjects in inspector
        public LevelTilemapController controllerTilemap;
        public LevelEventController controllerEvents;

        // Render camera things
        public Camera renderCamera;
        private Texture2D tex;

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
            await manager.LoadFilesAsync(serializeContext);

            using (serializeContext) {
                // Init editor data
                await BaseEditorManager.Init(context);

                // Load the level
                LevelEditorData.EditorManager = await manager.LoadAsync(serializeContext, true);
                LevelEditorData.CurrentMap = LevelEditorData.EditorManager.Level.DefaultMap;
                LevelEditorData.CurrentCollisionMap = LevelEditorData.EditorManager.Level.DefaultCollisionMap;

                var notSupportedEventTypes = LevelEditorData.Level.EventData.Where(x => !Enum.IsDefined(LevelEditorData.EditorManager.EventTypeEnumType, x.TypeValue)).Select(x => x.TypeValue).Distinct().OrderBy(x => x).ToArray();

                if (notSupportedEventTypes.Any())
                    Debug.LogWarning($"The following event types are not supported: {String.Join(", ", notSupportedEventTypes)}");

                await Controller.WaitIfNecessary();

                Controller.status = $"Initializing tile maps";

                // Init tilemaps
                controllerTilemap.InitializeTilemaps();

                await Controller.WaitIfNecessary();

                Controller.status = $"Initializing events";

                // Add events
                Events = LevelEditorData.Level.EventData.Select(x => controllerEvents.AddEvent(x)).ToList();

                if (LevelEditorData.Level.Rayman != null)
                    RaymanEvent = controllerEvents.AddEvent(LevelEditorData.Level.Rayman);

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
                    ConvertLevelToPNG();
            }
        }

        public void SaveLevelTEMP() {
            // Set events
            Controller.obj.levelEventController.CalculateLinkIndexes();

            using (serializeContext)
                Settings.GetGameManager.SaveLevel(serializeContext, LevelEditorData.EditorManager);

            Debug.Log("Saved.");
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
                var tileSize = (int)tileSet.Tiles.First().sprite.rect.width;

                // Create the texture
                var tileTex = new Texture2D(tileSetWidth * tileSize, tileSetHeight * tileSize, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                // Default to fully transparent
                tileTex.SetPixels(Enumerable.Repeat(new Color(0, 0, 0, 0), tileTex.width * tileTex.height).ToArray());

                // Add every tile to it
                for (int i = 0; i < tileCount; i++)
                {
                    // Get the tile texture
                    var tile = tileSet.Tiles[i].sprite;

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

        public void ConvertLevelToPNG() {

            // Get the path to save to
            var destPath = $@"Screenshots\{LevelEditorData.CurrentSettings.GameModeSelection}\{LevelEditorData.CurrentSettings.GameModeSelection} - {LevelEditorData.CurrentSettings.World} {LevelEditorData.CurrentSettings.Level:00}.png";

            // Create the directory
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));

            Enum[] exceptions = new Enum[]
            {
                EventType.TYPE_GENERATING_DOOR,
                EventType.TYPE_DESTROYING_DOOR,
                EventType.MS_scintillement,
                EventType.MS_super_gendoor,
                EventType.MS_super_kildoor,
                EventType.MS_compteur,
                EventType.TYPE_RAY_POS,
                EventType.TYPE_INDICATOR,
            };

            // TODO: Allow this to be configured | THIS whole part should be refactored, the foreach after is bad

            // Hide Rayman (except in Jaguar proto)
            if (RaymanEvent != null && LevelEditorData.CurrentSettings.EngineVersion != EngineVersion.RayJaguarProto)
                RaymanEvent.gameObject.SetActive(false);

            // Hide unused links and show gendoors
            foreach (var e in Events) 
            {
                e.ForceUpdate();

                // Hide always and editor events, except for certain ones
                if ((e.Data.GetIsAlways() || e.Data.GetIsEditor()) && !exceptions.Contains(e.Data.Type))
                    e.gameObject.SetActive(false);
                else
                    e.gameObject.SetActive(true);

                // Always hide events with no graphics
                if (e.defautRenderer.enabled)
                    e.gameObject.SetActive(false);

                // Hide events from other layers
                if (e.Data.MapLayer != null && e.Data.MapLayer - 1 != LevelEditorData.CurrentMap)
                    e.gameObject.SetActive(false);

                // TODO: Find solution to this
                // Temporarily hide the Rayman 2 waterfall as it has the wrong map layer
                if (Equals(e.Data.Type, PS1_R2Demo_EventType.WaterFall))
                    e.gameObject.SetActive(false);

                // Helper method
                bool isGendoor(Common_Event ee)
                {
                    if (LevelEditorData.CurrentSettings.MajorEngineVersion == MajorEngineVersion.Jaguar)
                        return ee.LinkID != 0;
                    else
                        return ee.Data.Type is EventType et &&
                               (et == EventType.TYPE_GENERATING_DOOR ||
                                et == EventType.TYPE_DESTROYING_DOOR ||
                                et == EventType.MS_scintillement ||
                                et == EventType.MS_super_gendoor ||
                                et == EventType.MS_super_kildoor ||
                                et == EventType.MS_compteur);
                }

                e.ChangeLinksVisibility(true);

                if (e.LinkID == 0) 
                {
                    e.lineRend.enabled = false;
                    e.linkCube.gameObject.SetActive(false);
                }
                else {
                    //Hide link if not linked to gendoor
                    bool gendoorFound = isGendoor(e);
                    var allofSame = new List<Common_Event> {
                        e
                    };
                    foreach (Common_Event f in Events.Where(f => f.LinkID == e.LinkID)) {
                        allofSame.Add(f);
                        if (isGendoor(f))
                            gendoorFound = true;
                    }
                    if (!gendoorFound) {
                        foreach(var a in allofSame) {
                            a.lineRend.enabled = false;
                            a.linkCube.gameObject.SetActive(false);
                        }
                    }
                }
            }

            bool half = false;
            RenderTexture renderTex = new RenderTexture(LevelEditorData.MaxWidth * LevelEditorData.EditorManager.CellSize / (half ? 2 : 1), LevelEditorData.MaxHeight * LevelEditorData.EditorManager.CellSize / (half ? 2 : 1), 24);
            renderCamera.targetTexture = renderTex;
            //Set camera pos
            var cellSizeInUnits = LevelEditorData.EditorManager.CellSize / (float)LevelEditorData.EditorManager.PixelsPerUnit;
            renderCamera.transform.position = new Vector3((LevelEditorData.MaxWidth) * cellSizeInUnits / 2f, -(LevelEditorData.MaxHeight) * cellSizeInUnits / 2f, renderCamera.transform.position.z);
            renderCamera.orthographicSize = (LevelEditorData.MaxHeight * cellSizeInUnits / 2f);
            renderCamera.rect = new Rect(0, 0, 1, 1);
            renderCamera.Render();

            //Save to picture
            RenderTexture.active = renderTex;

            tex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();

            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(destPath, bytes);

            Destroy(tex);
            RenderTexture.active = null;
            renderCamera.rect = new Rect(0, 0, 0, 0);

            Debug.Log("Level saved as PNG");

            //Unsub events
            Settings.OnShowAlwaysEventsChanged -= controllerEvents.ChangeEventsVisibility;
            Settings.OnShowEditorEventsChanged -= controllerEvents.ChangeEventsVisibility;

            if (Settings.ScreenshotEnumeration)
                SceneManager.LoadScene("Dummy");
        }

        public void TabClicked(int tabIndex) {
            tabs[tabIndex].open.SetActive(!tabs[tabIndex].open.activeSelf);
            tabs[tabIndex].closed.SetActive(!tabs[tabIndex].closed.activeSelf);
        }
    }
}
