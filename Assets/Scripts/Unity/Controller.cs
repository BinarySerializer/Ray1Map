using Cysharp.Threading.Tasks;

using System.Diagnostics;
using BinarySerializer;
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
        public LoadingScreen loadingScreen;

        public GameObject editorUI;

        public Text tempDebugText;

        public WebCommunicator webCommunicator;

        private static readonly Stopwatch stopwatch = new Stopwatch();

        public enum State {
            None,
            LoadingFiles,
            Loading,
            Initializing,
            Error,
            Finished
        }
        public static State LoadState { get; set; }
        public static string DetailedState { get; set; } = "Starting";

        public static async UniTask WaitFrame()
        {
            //await UniTask.NextFrame();
            await UniTask.Yield();
            //await UniTask.WaitForEndOfFrame();

            if (stopwatch.IsRunning) stopwatch.Restart();
        }

        public static void StartStopwatch() {
            stopwatch.Start();
        }
        public static void StopStopwatch() {
            if(stopwatch.IsRunning) stopwatch.Stop();
        }

        public static async UniTask WaitIfNecessary()
        {
            if (!stopwatch.IsRunning) stopwatch.Start();
            if (stopwatch.ElapsedMilliseconds > 16)
                await WaitFrame();
        }

        void Awake() 
        {
            obj = this;
            levelController = GameObject.Find("Level").GetComponent<LevelMainController>();
            levelEventController = GameObject.Find("Level").GetComponent<LevelEventController>();
            Application.logMessageReceived += Log;
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                UnityEngine.Debug.unityLogger.filterLogType = LogType.Assert;
            }

            // Make sure filesystem is set before checking here
            Settings.ConfigureFileSystem();
            if (FileSystem.mode == FileSystem.Mode.Web) {
                editorUI.SetActive(false);
            }
        }

        async UniTaskVoid Start()
        {
            var loadTimer = new Stopwatch();
            StartStopwatch();
            loadTimer.Start();

            loadingScreen.Active = true;
            LoadState = State.Loading;
            DetailedState = "Starting...";

            // Create the context
            LevelEditorData.MainContext = new R1Context(Settings.GetGameSettings);
            await levelController.LoadLevelAsync(Settings.GetGameManager, LevelEditorData.MainContext);
            if (Settings.ScreenshotEnumeration) return;

            await WaitIfNecessary();
            if (LoadState == State.Error) return;

            DetailedState = "Finished";
            LoadState = State.Finished;
            loadingScreen.Active = false;

            StopStopwatch();
            loadTimer.Stop();

            var startEvent = LevelEditorData.Level.Rayman ?? LevelEditorData.Level.ObjManager?.GetMainObject(LevelEditorData.Level.EventData);

            if (startEvent != null) {
                var startEventBehaviour = levelController.GetAllObjects.FindItem(x => x.ObjData == startEvent);
                if (startEventBehaviour != null) {
                    Controller.obj.levelEventController.editor.cam.JumpTo(startEventBehaviour.gameObject, immediate: true);
                }
                //Controller.obj.levelEventController.editor.cam.pos = new Vector3(startEvent.XPosition / (float)LevelEditorData.Level.PixelsPerUnit, -(startEvent.YPosition / (float)LevelEditorData.Level.PixelsPerUnit));
            }
            Debug.Log($"Loaded in {loadTimer.ElapsedMilliseconds}ms");
        }

        public void OnDestroy()
        {
            LevelEditorData.Level = null;
            LevelEditorData.MainContext?.Dispose();
            LevelEditorData.MainContext = null;
        }

        private void Update()
        {
            if (loadingScreen.Active)
            {
                if (LoadState == State.Error)
                {
                    loadingScreen.LoadingText = DetailedState;
                    loadingScreen.LoadingtextColor = Color.red;
                }
                else
                {
                    loadingScreen.LoadingText = DetailedState;
                    loadingScreen.LoadingtextColor = Color.white;
                }
            }
        }

        public void Log(string condition, string stacktrace, LogType type) {
            switch (type) {
                case LogType.Exception:
                case LogType.Error:
                    if (LoadState != State.Finished) {
                        // Allowed exceptions
                        if (condition.Contains("cleaning the mesh failed")) break;
                        if (condition.Contains("desc.isValid() failed!")) break;

                        // Go to error state
                        LoadState = State.Error;
                        if (loadingScreen.Active) {
                            DetailedState = condition;
                        }
                    }
                    break;
            }
        }
    }
}
