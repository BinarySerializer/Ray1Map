using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    public static class LevelEditorData
    {
        public static Context MainContext { get; set; }
        public static GameSettings CurrentSettings => MainContext?.Settings;

        public static BaseEditorManager EditorManager { get; set; }
        public static Common_Lev Level => EditorManager?.Level;

        public static int MaxWidth => Level.Maps.Max(x => x.Width);
        public static int MaxHeight => Level.Maps.Max(x => x.Height);

        public static int CurrentMap { get; set; }
        public static int CurrentCollisionMap { get; set; }
    }
}