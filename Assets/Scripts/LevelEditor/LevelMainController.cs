using UnityEngine;

namespace R1Engine {
    public class LevelMainController : MonoBehaviour {

        // The current level we are operating with
        public Common_Lev currentLevel;

        // References to specific level controller gameObjects in inspector
        public LevelTilemapController controllerTilemap;
        public LevelEventController controllerEvents;

        // Reference to the background ting
        public MeshFilter backgroundTint;


        public void LoadLevel(IGameManager manager, GameSettings settings) 
        {
            // Load the level
            currentLevel = manager.LoadLevel(settings, EventInfoManager.LoadEventInfo());

            // Init tilemaps
            controllerTilemap.InitializeTilemaps();
            // Init event things
            controllerEvents.InitializeEvents();

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
            Settings.GetManager().SaveLevel(Settings.GetGameSettings, currentLevel);
            Debug.Log("Saved.");
        }
    }
}
