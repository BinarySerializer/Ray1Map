using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R1Engine
{
    public class LevelMainController : MonoBehaviour {

        /// <summary>
        /// The editor manager
        /// </summary>
        public BaseEditorManager EditorManager;

        // The current level we are operating with
        public Common_Lev currentLevel => EditorManager?.Level;

        // The context, to reuse when writing
        private Serialize.Context serializeContext;

        // References to specific level controller gameObjects in inspector
        public LevelTilemapController controllerTilemap;
        public LevelEventController controllerEvents;

        // Reference to the background ting
        public MeshFilter backgroundTint;

        // Render camera things
        public Camera renderCamera;
        public Texture2D tex;

        public async Task LoadLevelAsync(IGameManager manager, Serialize.Context context) 
        {
            // Create the context
            serializeContext = context;

            // Make sure all the necessary files are downloaded
            await manager.LoadFilesAsync(serializeContext);

            using (serializeContext) {
                // Load the level
                EditorManager = await manager.LoadAsync(serializeContext);

                await Controller.WaitIfNecessary();

                Controller.status = $"Initializing tile maps";

                // Init tilemaps
                controllerTilemap.InitializeTilemaps();

                await Controller.WaitIfNecessary();

                Controller.status = $"Initializing events";

                // Init event things
                controllerEvents.InitializeEvents();

                await Controller.WaitIfNecessary();

                // Draw the background tint
                var mo = new Mesh {
                    vertices = new Vector3[]
                    {
                    new Vector3(0, 0), new Vector3(currentLevel.Width, 0), new Vector3(currentLevel.Width, -currentLevel.Height),
                    new Vector3(0, -currentLevel.Height)
                    }
                };

                mo.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
                backgroundTint.sharedMesh = mo;

                // FOR AUTOMATION:
                ConvertLevelToPNG();
            }
        }

        public void SaveLevelTEMP() {
            using (serializeContext) {
                Settings.GetGameManager.SaveLevel(serializeContext, currentLevel);
            }
            Debug.Log("Saved.");
        }

        public void ConvertLevelToPNG() {

            // Get the path to save to
            //var destPath = EditorUtility.SaveFilePanel("Select file destination", null, $"{Settings.GetGameSettings.GameModeSelection} - {Settings.World} {Settings.Level:00}.png", "png");

            var destPath = $@"Screenshots\{Settings.GetGameSettings.GameModeSelection}\{Settings.GetGameSettings.GameModeSelection} - {Settings.World} {Settings.Level:00}.png";

            Directory.CreateDirectory(Path.GetDirectoryName(destPath));

            // TODO: Allow this to be configured | THIS whole aprt should be refactored, the foreach after is bad
            // Set settings
            //Settings.ShowAlwaysEvents = false;
            //Settings.ShowEditorEvents = false;

            // Hide unused links and show gendoors
            foreach (var e in currentLevel.Events) 
            {
                if (e.Flag==EventFlag.Always || e.Flag == EventFlag.Editor)
                    e.gameObject.SetActive(false);
                if (e.Type == EventType.TYPE_GENERATING_DOOR || e.Type == EventType.TYPE_DESTROYING_DOOR || e.Type==EventType.MS_scintillement || e.Type==EventType.MS_super_gendoor || e.Type == EventType.MS_super_kildoor || e.Type==EventType.MS_compteur)
                    e.gameObject.SetActive(true);

                e.ChangeLinksVisibility(true);

                if (e.LinkID == 0) 
                {
                    e.lineRend.enabled = false;
                    e.linkCube.gameObject.SetActive(false);
                }
                else {
                    //Hide link if not linked to gendoor
                    bool gendoorFound = false;
                    if (e.Type == EventType.TYPE_GENERATING_DOOR || e.Type == EventType.TYPE_DESTROYING_DOOR || e.Type == EventType.MS_scintillement || e.Type == EventType.MS_super_gendoor || e.Type == EventType.MS_super_kildoor || e.Type == EventType.MS_compteur)
                        gendoorFound = true;
                    var allofSame = new List<Common_Event>();
                    allofSame.Add(e);
                    foreach (var f in currentLevel.Events) {
                        if (f.LinkID == e.LinkID) {
                            allofSame.Add(f);
                            if (f.Type == EventType.TYPE_GENERATING_DOOR || f.Type == EventType.TYPE_DESTROYING_DOOR || f.Type == EventType.MS_scintillement || f.Type == EventType.MS_super_gendoor || f.Type == EventType.MS_super_kildoor || f.Type == EventType.MS_compteur)
                                gendoorFound = true;
                        }
                    }
                    if (!gendoorFound) {
                        foreach(var a in allofSame) {
                            a.lineRend.enabled = false;
                            a.linkCube.gameObject.SetActive(false);
                        }
                    }
                }
            }

            RenderTexture renderTex = new RenderTexture(currentLevel.Width*16, currentLevel.Height*16, 24);
            renderCamera.targetTexture = renderTex;
            //Set camera pos
            renderCamera.transform.position = new Vector3((currentLevel.Width) / 2f, -(currentLevel.Height) / 2f, renderCamera.transform.position.z);
            renderCamera.orthographicSize = (currentLevel.Height / 2f);
            renderCamera.rect = new Rect(0, 0, 1, 1);
            renderCamera.Render();

            //Save to picture
            RenderTexture.active = renderTex;

            tex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();

            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(destPath, bytes);

            Destroy(tex);
            RenderTexture.active = null;
            renderCamera.rect = new Rect(0, 0, 0, 0);

            Debug.Log("Level saved as PNG");

            //For automation, go to the dummy scene to clear this place
            SceneManager.LoadScene("Dummy");
        }
    }
}
