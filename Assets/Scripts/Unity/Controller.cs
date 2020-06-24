using System;
using Asyncoroutine;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using R1Engine.Serialize;

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

        public Text tempDebugText;

        private Stopwatch stopwatch;
        public static Context MainContext { get; private set; }

        public static GameSettings CurrentSettings => MainContext?.Settings;

        public static string status
        {
            get => obj.loadingText.text;
            set
            {
                if (obj?.loadingText != null)
                    obj.loadingText.text = value;
            }
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

        async void Start()
        {
            var loadTimer = new Stopwatch();
            stopwatch.Start();
            loadTimer.Start();

            status = "Starting...";

            // Create the context
            MainContext = new Context(Settings.GetGameSettings);
            await levelController.LoadLevelAsync(Settings.GetGameManager, MainContext);

            status = String.Empty;

            stopwatch.Stop();
            loadTimer.Stop();

            var startEvent = levelController.Events.FindItem(x => x.Data.Type is EventType et && (et == EventType.TYPE_RAY_POS || et == EventType.TYPE_PANCARTE));

            if (startEvent != null)
                Camera.main.transform.position = new Vector3(startEvent.Data.EventData.XPosition, startEvent.Data.EventData.YPosition, -10f);

            Debug.Log($"Loaded in {loadTimer.ElapsedMilliseconds}ms");
        }
    }
}
