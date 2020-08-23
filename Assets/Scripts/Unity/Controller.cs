using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private Stopwatch stopwatch;

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

        public async UniTask WaitFrame()
        {
            await UniTask.WaitForEndOfFrame();

            if (stopwatch.IsRunning)
                stopwatch.Restart();
        }

        public static async UniTask WaitIfNecessary()
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
            stopwatch.Start();
            loadTimer.Start();

            loadingScreen.Active = true;
            LoadState = State.Loading;
            DetailedState = "Starting...";

            // Create the context
            LevelEditorData.MainContext = new Context(Settings.GetGameSettings);
            await levelController.LoadLevelAsync(Settings.GetGameManager, LevelEditorData.MainContext);

            await WaitIfNecessary();
            if (LoadState == State.Error) return;

            DetailedState = "Finsihed";
            LoadState = State.Finished;
            loadingScreen.Active = false;

            stopwatch.Stop();
            loadTimer.Stop();

            var startEvent = LevelEditorData.Level.Rayman?.Data ?? levelController.Events.FindItem(x => (x.Data.Type is R1_EventType et && (et == R1_EventType.TYPE_RAY_POS || et == R1_EventType.TYPE_PANCARTE)) || (x.Data.Type is GBA_R3_ActorID ai && ai == GBA_R3_ActorID.Rayman))?.Data.Data;

            if (startEvent != null)
                Controller.obj.levelEventController.editor.cam.pos = new Vector3(startEvent.XPosition / (float)LevelEditorData.EditorManager.PixelsPerUnit, -(startEvent.YPosition / (float)LevelEditorData.EditorManager.PixelsPerUnit));

            Debug.Log($"Loaded in {loadTimer.ElapsedMilliseconds}ms");
        }

        public void OnDestroy()
        {
            LevelEditorData.EditorManager = null;
        }

        public void FindMatchingEncoding(params KeyValuePair<string, byte[]>[] input)
        {
            if (input.Length < 2)
                throw new Exception("Too few strings to check!");

            // Get all possible encodings
            var encodings = Encoding.GetEncodings().Select(x => Encoding.GetEncoding(x.CodePage)).ToArray();

            // Keep a list of all matching ones
            var matches = new List<Encoding>();

            // Helper method for getting all matching encodings
            IEnumerable<Encoding> GetMatches(KeyValuePair<string, byte[]> str)
            {
                var m = encodings.Where(enc => enc.GetString(str.Value).Equals(str.Key, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                Debug.Log($"Matching encodings for {str.Key}: {String.Join(", ", m.Select(x => $"{x.EncodingName} ({x.CodePage})"))}");
                return m;
            }

            // Add matches for the first one
            matches.AddRange(GetMatches(input.First()));

            // Check remaining ones, removing any which don't match
            foreach (var str in input.Skip(1))
            {
                var ma = GetMatches(str);
                matches.RemoveAll(x => !ma.Contains(x));
            }

            // Log the result
            Debug.Log($"Matching encodings for all: {String.Join(", ", matches.Select(x => $"{x.EncodingName} ({x.CodePage})"))}");
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

		private void Update() {
            if (loadingScreen.Active) {
                if (LoadState == State.Error) {
                    loadingScreen.LoadingText = DetailedState;
                    loadingScreen.LoadingtextColor = Color.red;
                } else {
                    loadingScreen.LoadingText = DetailedState;
                    loadingScreen.LoadingtextColor = Color.white;
                }
            }
        }
	}
}
