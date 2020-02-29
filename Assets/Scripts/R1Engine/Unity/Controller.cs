using System;
using UnityEngine;

namespace R1Engine.Unity {
    public class Controller : MonoBehaviour {
        public static Controller obj;

        [HideInInspector] public LevelBehaviour lvl;

        void Awake() 
        {
            obj = this;
            lvl = GameObject.Find("Level").GetComponent<LevelBehaviour>();
        }
        void Start() 
        {
            lvl.LoadLevel(Settings.GetManager(), Settings.CurrentDirectory, Settings.World, Settings.Level);
            Camera.main.transform.position = lvl.level.RaymanPos;
        }
    }
}
