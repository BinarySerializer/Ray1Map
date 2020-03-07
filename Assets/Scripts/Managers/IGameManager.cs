using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// Defines a generic game manager
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        bool Has3Palettes { get; }

        /// <summary>
        /// Gets the levels for the specified world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        int[] GetLevels(GameSettings settings);

        /// <summary>
        /// Gets the available educational volumes
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The available educational volumes</returns>
        string[] GetEduVolumes(GameSettings settings);

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="eventInfoData">The loaded event info data</param>
        /// <returns>The level</returns>
        Task<Common_Lev> LoadLevelAsync(GameSettings settings, EventInfoData[] eventInfoData);

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="commonLevelData">The common level data</param>
        void SaveLevel(GameSettings settings, Common_Lev commonLevelData);
    }
}