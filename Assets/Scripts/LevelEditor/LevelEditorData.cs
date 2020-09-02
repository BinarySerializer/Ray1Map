using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;

namespace R1Engine
{
    public static class LevelEditorData
    {
        public static GeneralEventInfoData[] EventInfoData { get; private set; }

        public static Context MainContext { get; set; }
        public static GameSettings CurrentSettings => MainContext?.Settings;

        public static Unity_Level Level { get; set; }
        public static Unity_ObjectManager ObjManager => Level.ObjManager;

        public static int MaxWidth => Level.Maps.Max(x => x.Width);
        public static int MaxHeight => Level.Maps.Max(x => x.Height);

        public static int CurrentMap { get; set; }
        public static int CurrentCollisionMap { get; set; }

        public static async UniTask Init()
        {
            const string file = "Events.csv";

            await FileSystem.PrepareFile(file);

            // Load the event info data
            using (var csvFile = FileSystem.GetFileReadStream(file))
                EventInfoData = GeneralEventInfoData.ReadCSV(csvFile);
        }
    }
}