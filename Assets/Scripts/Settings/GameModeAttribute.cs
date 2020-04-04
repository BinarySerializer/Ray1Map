using System;

namespace R1Engine
{
    /// <summary>
    /// Attribute for the <see cref="GameModeSelection"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GameModeAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="majorEngineVersion">The major engine version</param>
        /// <param name="engineVersion">The engine version</param>
        /// <param name="game">The game</param>
        /// <param name="displayName">The display name</param>
        /// <param name="managerType">The manager type</param>
        public GameModeAttribute(MajorEngineVersion majorEngineVersion, EngineVersion engineVersion, Game game, string displayName, Type managerType)
        {
            MajorEngineVersion = majorEngineVersion;
            EngineVersion = engineVersion;
            Game = game;
            DisplayName = displayName;
            ManagerType = managerType;
        }

        /// <summary>
        /// The major engine version
        /// </summary>
        public MajorEngineVersion MajorEngineVersion { get; }

        /// <summary>
        /// The engine version
        /// </summary>
        public EngineVersion EngineVersion { get; }

        /// <summary>
        /// The game
        /// </summary>
        public Game Game { get; }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The manager type
        /// </summary>
        public Type ManagerType { get; }
    }
}