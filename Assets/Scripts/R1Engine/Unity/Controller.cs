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

            var startEvent = levelController.currentLevel.Events.FindItem(x => (int)x.type == 99 || (int)x.type == 124);

            if (startEvent != null)
                Camera.main.transform.position = startEvent.pos;
        }
    }
}
