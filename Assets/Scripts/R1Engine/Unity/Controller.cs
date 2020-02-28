using System;
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
            lvl.LoadLevel(GetManager(game), Settings.gameDirs[game], world, levelNo);
            Camera.main.transform.position = lvl.level.RaymanPos;
        }

        protected static IGameManager GetManager(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.RaymanPS1:
                    return new PS1_R1_Manager();

                case GameMode.RaymanPC:
                    return new PC_R1_Manager();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
