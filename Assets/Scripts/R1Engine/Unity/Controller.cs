using UnityEngine;

namespace R1Engine.Unity {
    public class Controller : MonoBehaviour {
        public static Controller obj;

        public GameMode game;
        public World world;
        public int levelNo = 1;

        [HideInInspector] public LevelBehaviour lvl;

        void Awake() {
            obj = this;
            lvl = GameObject.Find("Level").GetComponent<LevelBehaviour>();
        }
        void Start() {
            lvl.LoadLevel(world, levelNo);
            Camera.main.transform.position = lvl.level.raymanPos;
        }
    }
}
