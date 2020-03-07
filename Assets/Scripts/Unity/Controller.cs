using System;
using Asyncoroutine;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace R1Engine
{
    public class Controller : MonoBehaviour {
        public static Controller obj;

        [HideInInspector]
        public LevelMainController levelController;
        [HideInInspector]
        public LevelEventController levelEventController;

        // The loading string
        public Text loadingText;

        private Stopwatch stopwatch;

        public static string status
        {
            set => obj.loadingText.text = value;
        }

        public async Task WaitFrame()
        {
            await new WaitForEndOfFrame();

            if (stopwatch.IsRunning)
                stopwatch.Restart();
        }

        public static async Task WaitIfNecessary()
        {
            if (obj == null)
                return;

            if (obj.stopwatch.ElapsedMilliseconds > 16)
                await obj.WaitFrame();
        }

        void Awake() 
        {
            stopwatch = new Stopwatch();
            obj = this;
            levelController = GameObject.Find("Level").GetComponent<LevelMainController>();
            levelEventController = GameObject.Find("Level").GetComponent<LevelEventController>();
        }

        async Task Start() 
        {
            stopwatch.Start();

            status = "Starting...";

            await levelController.LoadLevelAsync(Settings.GetGameManager, Settings.GetGameSettings);

            status = String.Empty;

            stopwatch.Stop();

            var startEvent = levelController.currentLevel.Events.FindItem(x => x.EventInfoData?.Type == 99 || x.EventInfoData?.Type == 124);

            if (startEvent != null)
                Camera.main.transform.position = new Vector3(startEvent.XPosition, startEvent.YPosition);

            Debug.Log("Loaded");
        }
    }
}
