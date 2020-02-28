using System.Collections.Generic;
using static R1Engine.GameMode;

namespace R1Engine {
    public class Settings {

        public static Dictionary<GameMode, string> gameDirs = new Dictionary<GameMode, string> {
            { RaymanPS1, "GameDirs/RaymanPS1" }
        };
    }
}