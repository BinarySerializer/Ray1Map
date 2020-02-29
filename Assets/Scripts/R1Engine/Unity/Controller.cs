using System;
using UnityEngine;

namespace R1Engine.Unity {
    public class Controller : MonoBehaviour {
        public static Controller obj;

        [HideInInspector]
        public LevelMainController levelController;

        void Awake() 
        {
            obj = this;
            levelController = GameObject.Find("Level").GetComponent<LevelMainController>();
        }
        void Start() 
        {
            levelController.LoadLevel(Settings.GetManager(), Settings.CurrentDirectory, Settings.World, Settings.Level);
            Camera.main.transform.position = levelController.currentLevel.RaymanPos;
        }
    }
}
