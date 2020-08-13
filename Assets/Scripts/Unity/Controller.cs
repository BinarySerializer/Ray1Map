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
        public Text loadingText;

        public Text tempDebugText;

        private Stopwatch stopwatch;

        public static string status
        {
            get => obj.loadingText.text;
            set
            {
                if (obj?.loadingText != null)
                    obj.loadingText.text = value;
            }
        }

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
        }

        async UniTaskVoid Start()
        {
            var loadTimer = new Stopwatch();
            stopwatch.Start();
            loadTimer.Start();

            status = "Starting...";

            // Create the context
            LevelEditorData.MainContext = new Context(Settings.GetGameSettings);
            await levelController.LoadLevelAsync(Settings.GetGameManager, LevelEditorData.MainContext);

            status = String.Empty;

            stopwatch.Stop();
            loadTimer.Stop();

            var startEvent = LevelEditorData.Level.Rayman?.Data ?? levelController.Events.FindItem(x => x.Data.Type is EventType et && (et == EventType.TYPE_RAY_POS || et == EventType.TYPE_PANCARTE))?.Data.Data;

            if (startEvent != null)
                Camera.main.transform.position = new Vector3(startEvent.XPosition, startEvent.YPosition, -10f);

            Debug.Log($"Loaded in {loadTimer.ElapsedMilliseconds}ms");
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
    }
}
