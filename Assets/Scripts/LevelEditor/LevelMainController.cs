using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        /// The editor manager
        /// </summary>
        public BaseEditorManager EditorManager;

        /// <summary>
        /// The events
        /// </summary>
        public List<Common_Event> Events { get; set; }

        public Common_Event RaymanEvent { get; set; }

        public IEnumerable<Common_Event> GetAllEvents => RaymanEvent != null ? Events.Append(RaymanEvent) : Events;

        // The current level we are operating with
        public Common_Lev currentLevel => EditorManager?.Level;
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

        public async Task LoadLevelAsync(IGameManager manager, Context context) 
        {
            // Create the context
            serializeContext = context;

            // Make sure all the necessary files are downloaded
            await manager.LoadFilesAsync(serializeContext);

            using (serializeContext) {
                // Load the level
                EditorManager = await manager.LoadAsync(serializeContext, true);

                await Controller.WaitIfNecessary();

                Controller.status = $"Initializing tile maps";

                // Resize the background tint
                controllerTilemap.ResizeBackgroundTint(currentLevel.Maps[editor.currentMap].Width, currentLevel.Maps[editor.currentMap].Height);

                // Init tilemaps
                controllerTilemap.InitializeTilemaps();

                await Controller.WaitIfNecessary();

                Controller.status = $"Initializing events";

                // Add events
                Events = currentLevel.EventData.Select(x => controllerEvents.AddEvent(x)).ToList();

                if (currentLevel.Rayman != null)
                    RaymanEvent = controllerEvents.AddEvent(currentLevel.Rayman);

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
                Settings.GetGameManager.SaveLevel(serializeContext, EditorManager);

            Debug.Log("Saved.");
        }

        public void ExportTileset() 
        {
            var tileSetIndex = 0;

            // Export every tile set
            foreach (var tileSet in currentLevel.Maps[editor.currentMap].TileSet.Where(x => x?.Tiles?.Any(y => y != null) == true))
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

                var destPath = $@"Tilemaps\{Controller.CurrentSettings.GameModeSelection}\{Controller.CurrentSettings.GameModeSelection} - {Controller.CurrentSettings.World} {Controller.CurrentSettings.Level:00} ({tileSetIndex}).png";

                Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                // Save the tile map
                File.WriteAllBytes(destPath, tileTex.EncodeToPNG());

                tileSetIndex++;
            }
        }

        public void ConvertLevelToPNG() {

            // Get the path to save to
            var destPath = $@"Screenshots\{Controller.CurrentSettings.GameModeSelection}\{Controller.CurrentSettings.GameModeSelection} - {Controller.CurrentSettings.World} {Controller.CurrentSettings.Level:00}.png";

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

            // Hide Rayman
            if (RaymanEvent != null)
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

                // Helper method
                bool isGendoor(Common_Event ee)
                {
                    if (Controller.CurrentSettings.EngineVersion == EngineVersion.RayJaguar)
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

            RenderTexture renderTex = new RenderTexture(currentLevel.Maps[editor.currentMap].Width*16, currentLevel.Maps[editor.currentMap].Height*16, 24);
            renderCamera.targetTexture = renderTex;
            //Set camera pos
            renderCamera.transform.position = new Vector3((currentLevel.Maps[editor.currentMap].Width) / 2f, -(currentLevel.Maps[editor.currentMap].Height) / 2f, renderCamera.transform.position.z);
            renderCamera.orthographicSize = (currentLevel.Maps[editor.currentMap].Height / 2f);
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
