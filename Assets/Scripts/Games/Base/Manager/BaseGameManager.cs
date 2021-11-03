﻿using BinarySerializer;
using Cysharp.Threading.Tasks;

namespace Ray1Map
{
    /// <summary>
    /// Defines a generic game manager
    /// </summary>
    public abstract class BaseGameManager
    {
        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public abstract GameInfo_Volume[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public virtual GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];

        /// <summary>
        /// Loads the level specified by the settings for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The level</returns>
        public abstract UniTask<Unity_Level> LoadAsync(Context context);

        /// <summary>
        /// Saves the loaded level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="level">The level</param>
        public virtual UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new System.NotImplementedException();

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public virtual UniTask LoadFilesAsync(Context context) => UniTask.CompletedTask;

        /// <summary>
        /// Allows additional settings objects to be added to the context on creation
        /// </summary>
        /// <param name="context">The newly created context</param>
        public virtual void AddContextSettings(Context context) { }

        /// <summary>
        /// Allows pre-defined pointers to be added to the context on creation
        /// </summary>
        /// <param name="context">The newly created context</param>
        public virtual void AddContextPointers(Context context) { }
    }
}