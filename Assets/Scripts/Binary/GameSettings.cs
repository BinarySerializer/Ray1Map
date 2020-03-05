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
        /// <param name="gameDirectory">The game directory</param>
        /// <param name="world">The game world</param>
        /// <param name="level">The game level, starting at 1</param>
        public GameSettings(string gameDirectory, World world = World.Jungle, int level = 1)
        {
            GameDirectory = gameDirectory;
            World = world;
            Level = level;
        }

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