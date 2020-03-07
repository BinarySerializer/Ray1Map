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
        /// <param name="gameMode">The game mode</param>
        /// <param name="displayName">The display name</param>
        /// <param name="managerType">The manager type</param>
        public GameModeAttribute(GameMode gameMode, string displayName, Type managerType)
        {
            DisplayName = displayName;
            ManagerType = managerType;
            GameMode = gameMode;
        }

        /// <summary>
        /// The game mode
        /// </summary>
        public GameMode GameMode { get; }

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