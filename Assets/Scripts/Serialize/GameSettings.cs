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
        /// <param name="gameModeSelection">The game mode selection</param>
        /// <param name="gameDirectory">The game directory</param>
        /// <param name="world">The game world</param>
        /// <param name="level">The game level, starting at 1</param>
        public GameSettings(GameModeSelection gameModeSelection, string gameDirectory, World world = World.Jungle, int level = 1)
        {
            GameModeSelection = gameModeSelection;
            GameMode = gameModeSelection.GetAttribute<GameModeAttribute>().GameMode;
            GameDirectory = Util.NormalizePath(gameDirectory, isFolder: true);
            World = world;
            Level = level;
        }

        /// <summary>
        /// The game mode selection
        /// </summary>
        public GameModeSelection GameModeSelection { get; }

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

        /// <summary>
        /// The educational game volume name
        /// </summary>
        public string EduVolume { get; set; }
    }
}