using System;
using System.Collections.Generic;
using Asyncoroutine;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            //FindMatchingEncoding(
            //    new KeyValuePair<string, byte[]>("TARAYZAN DÀ A RAYMAN UN SEME MAGICO", new byte[] { 0x54, 0x41, 0x52, 0x41, 0x59, 0x5A, 0x41, 0x4E, 0x20, 0x44, 0xC3, 0x80, 0x20, 0x41, 0x20, 0x52, 0x41, 0x59, 0x4D, 0x41, 0x4E, 0x20, 0x55, 0x4E, 0x20, 0x53, 0x45, 0x4D, 0x45, 0x20, 0x4D, 0x41, 0x47, 0x49, 0x43, 0x4F }),
            //    new KeyValuePair<string, byte[]>("bongo-hügel", new byte[] { 0x62, 0x6F, 0x6E, 0x67, 0x6F, 0x2D, 0x68, 0x81, 0x67, 0x65, 0x6C }),
            //    new KeyValuePair<string, byte[]>("löschen", new byte[] { 0x6C, 0x94, 0x73, 0x63, 0x68, 0x65, 0x6E })
            //    );


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

            var startEvent = levelController.EditorManager.Level.Rayman?.Data ?? levelController.Events.FindItem(x => x.Data.Type is EventType et && (et == EventType.TYPE_RAY_POS || et == EventType.TYPE_PANCARTE))?.Data.Data;

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
