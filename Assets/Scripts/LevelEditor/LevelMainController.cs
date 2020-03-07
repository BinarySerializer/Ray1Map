using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine {
    public class LevelMainController : MonoBehaviour {

        // The current level we are operating with
        public Common_Lev currentLevel;
        // The current designs we are using
        public List<Common_Design> currentDesigns;

        // References to specific level controller gameObjects in inspector
        public LevelTilemapController controllerTilemap;
        public LevelEventController controllerEvents;

        // Reference to the background ting
        public MeshFilter backgroundTint;


        public async Task LoadLevelAsync(IGameManager manager, GameSettings settings) 
        {
            // Load the level
            currentLevel = await manager.LoadLevelAsync(settings, EventInfoManager.LoadEventInfo());

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

        public void SaveLevelTEMP() {
            Settings.GetGameManager.SaveLevel(Settings.GetGameSettings, currentLevel);
            Debug.Log("Saved.");
        }
    }
}
