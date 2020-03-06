namespace R1Engine
{
    /// <summary>
    /// Common game settings
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="gameMode">The game mode</param>
        /// <param name="gameDirectory">The game directory</param>
        /// <param name="world">The game world</param>
        /// <param name="level">The game level, starting at 1</param>
        public GameSettings(GameMode gameMode, string gameDirectory, World world = World.Jungle, int level = 1)
        {
            GameMode = gameMode;
            GameDirectory = gameDirectory;
            World = world;
            Level = level;
        }

        /// <summary>
        /// The game mode
        /// </summary>
        public GameMode GameMode { get; }

        /// <summary>
        /// The game directory
        /// </summary>
        public string GameDirectory { get; set; }

        /// <summary>
        /// The game world
        /// </summary>
        public World World { get; set; }

        /// <summary>
        /// The game level, starting at 1
        /// </summary>
        public int Level { get; set; }
    }
}