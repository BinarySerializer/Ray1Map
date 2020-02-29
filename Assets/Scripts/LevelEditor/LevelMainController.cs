using UnityEngine;

namespace R1Engine.Unity {
    public class LevelMainController : MonoBehaviour {

        // The current level we are operating with
        public Common_Lev currentLevel;

        // References to specific level controller gameObjects in inspector
        public LevelTilemapController controllerTilemap;
        // TODO: public LevelEventController controllerEvent... or something similar

        // Reference to the background ting
        public MeshFilter backgroundTint;


        public void LoadLevel(IGameManager manager, string basePath, World world, int levelIndex) 
        {
            // Load the level
            currentLevel = manager.LoadLevel(basePath, world, levelIndex);

            // Make the tilemap controller to init all the tilemaps
            controllerTilemap.InitializeTilemaps(currentLevel);

            // TODO: Make a event controller to do all the event things
            foreach (var e in currentLevel.Events)
                Instantiate(EventBehaviour.resource).GetComponent<EventBehaviour>().ev = e;

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
}
