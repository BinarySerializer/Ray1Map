using UnityEngine;

namespace R1Engine
{
    public class Controller : MonoBehaviour {
        public static Controller obj;

        [HideInInspector]
        public LevelMainController levelController;
        [HideInInspector]
        public LevelEventController levelEventController;

        void Awake() 
        {
            obj = this;
            levelController = GameObject.Find("Level").GetComponent<LevelMainController>();
            levelEventController = GameObject.Find("Level").GetComponent<LevelEventController>();
        }
        void Start() 
        {
            levelController.LoadLevel(Settings.GetGameManager, Settings.GetGameSettings);

            var startEvent = levelController.currentLevel.Events.FindItem(x => x.EventInfoData?.Type == 99 || x.EventInfoData?.Type == 124);

            if (startEvent != null)
                Camera.main.transform.position = new Vector3(startEvent.XPosition, startEvent.YPosition);
        }
    }
}
