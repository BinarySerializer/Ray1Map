using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// Defines a generic game manager
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        KeyValuePair<int, int[]>[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        string[] GetEduVolumes(GameSettings settings);

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        GameAction[] GetGameActions(GameSettings settings);

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The editor manager</returns>
        UniTask<BaseEditorManager> LoadAsync(Context context, bool loadTextures);

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="editorManager">The editor manager</param>savel
        void SaveLevel(Context context, BaseEditorManager editorManager);

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        UniTask LoadFilesAsync(Context context);
    }
}