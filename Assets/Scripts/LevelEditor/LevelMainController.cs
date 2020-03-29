using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

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

            }
        }

        public void SaveLevelTEMP() {
            using (serializeContext) {
                Settings.GetGameManager.SaveLevel(serializeContext, currentLevel);
            }
            Debug.Log("Saved.");
        }

        public void ConvertLevelToPNG() {
            //Hide unused links and palette swappers etc
            foreach(var e in currentLevel.Events) {
                if (e.Type == EventType.TYPE_PALETTE_SWAPPER) {
                    e.gameObject.SetActive(false);
                }
                e.ChangeLinksVisibility(true);
                if (e.LinkID == 0) {
                    e.lineRend.enabled = false;
                    e.linkCube.gameObject.SetActive(false);
                }
            }

            RenderTexture renderTex = new RenderTexture(currentLevel.Width*16, currentLevel.Height*16, 24);
            renderCamera.targetTexture = renderTex;
            //Set camera pos
            renderCamera.transform.position = new Vector3((currentLevel.Width) / 2f, -(currentLevel.Height) / 2f, renderCamera.transform.position.z);
            renderCamera.orthographicSize = (currentLevel.Height / 2f);
            renderCamera.Render();

            //Save to picture
            RenderTexture.active = renderTex;

            tex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();

            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes($"E:/RayExports/{ Settings.World}{ Settings.Level:00}.png", bytes);

            Destroy(tex);
            RenderTexture.active = null;

            Debug.Log("Level saved as PNG");
        }
    }
}
