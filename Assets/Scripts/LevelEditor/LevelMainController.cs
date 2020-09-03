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
        public List<Unity_ObjBehaviour> Events { get; set; }

        public Unity_ObjBehaviour RaymanEvent { get; set; }

        public IEnumerable<Unity_ObjBehaviour> GetAllEvents => RaymanEvent != null ? Events.Append(RaymanEvent) : Events;

        public LevelEditorBehaviour editor => controllerEvents.editor;

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
            Controller.LoadState = Controller.State.LoadingFiles;
            await manager.LoadFilesAsync(serializeContext);
            await Controller.WaitIfNecessary();

            using (serializeContext) {
                // Init editor data
                await LevelEditorData.Init();
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
                Settings.GetGameManager.SaveLevel(serializeContext, LevelEditorData.Level);

            Debug.Log("Saved");
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

        public void ConvertLevelToPNG() 
        {
            // Get the path to save to
            var destPath = $@"Screenshots\{LevelEditorData.CurrentSettings.GameModeSelection}\{LevelEditorData.CurrentSettings.GameModeSelection} - {LevelEditorData.CurrentSettings.World} {LevelEditorData.CurrentSettings.Level:00}.png";

            // Create the directory
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));

            var screenshot = CreateLevelScreenshot();

            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(destPath, bytes);

            Destroy(tex);

            if (Settings.ScreenshotEnumeration)
            {
                Settings.OnShowAlwaysEventsChanged -= controllerEvents.ChangeEventsVisibility;
                Settings.OnShowEditorEventsChanged -= controllerEvents.ChangeEventsVisibility;
                SceneManager.LoadScene("Dummy");
            }
        }

        public Texture2D CreateLevelScreenshot()
        {
            // TODO: Allow this to be configured | THIS whole part should be refactored, the foreach after is bad

            // Hide unused links and show gendoors
            foreach (var e in Events)
            {
                // Update the event to make sure it has rendered
                e.ForceUpdate();

                if (e.ObjData is Unity_Object_R1 r1Obj)
                {
                    Enum[] exceptions = new Enum[]
                    {
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
                        e.gameObject.SetActive(true);
                }
                else if (e.ObjData is Unity_Object_GBA gbaObj)
                {
                    if (gbaObj.Actor.ActorID == 0)
                        e.gameObject.SetActive(true);
                }

                // Always hide events with no graphics
                if (e.defautRenderer.enabled)
                    e.gameObject.SetActive(false);

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

                e.ChangeLinksVisibility(true);

                if (e.ObjData.EditorLinkGroup == 0)
                {
                    e.lineRend.enabled = false;
                    e.linkCube.gameObject.SetActive(false);
                }
                else
                {
                    // Hide link if not linked to gendoor
                    bool gendoorFound = showLinksForObj(e);
                    var allofSame = new List<Unity_ObjBehaviour> {
                        e
                    };

                    foreach (Unity_ObjBehaviour f in Events.Where(f => f.ObjData.EditorLinkGroup == e.ObjData.EditorLinkGroup))
                    {
                        allofSame.Add(f);
                        if (showLinksForObj(f))
                            gendoorFound = true;
                    }

                    if (!gendoorFound)
                    {
                        foreach (var a in allofSame)
                        {
                            a.lineRend.enabled = false;
                            a.linkCube.gameObject.SetActive(false);
                        }
                    }
                }
            }

            RenderTexture renderTex = new RenderTexture(LevelEditorData.MaxWidth * LevelEditorData.Level.CellSize / 1, LevelEditorData.MaxHeight * LevelEditorData.Level.CellSize / 1, 24);
            renderCamera.targetTexture = renderTex;
            var cellSizeInUnits = LevelEditorData.Level.CellSize / (float)LevelEditorData.Level.PixelsPerUnit;
            renderCamera.transform.position = new Vector3((LevelEditorData.MaxWidth) * cellSizeInUnits / 2f, -(LevelEditorData.MaxHeight) * cellSizeInUnits / 2f, renderCamera.transform.position.z);
            renderCamera.orthographicSize = (LevelEditorData.MaxHeight * cellSizeInUnits / 2f);
            renderCamera.rect = new Rect(0, 0, 1, 1);
            renderCamera.Render();

            // Save to picture
            RenderTexture.active = renderTex;

            tex = TextureHelpers.CreateTexture2D(renderTex.width, renderTex.height);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;
            renderCamera.rect = new Rect(0, 0, 0, 0);

            return tex;
        }

        public void TabClicked(int tabIndex) {
            tabs[tabIndex].open.SetActive(!tabs[tabIndex].open.activeSelf);
            tabs[tabIndex].closed.SetActive(!tabs[tabIndex].closed.activeSelf);
        }
    }
}
