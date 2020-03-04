namespace R1Engine
{
    /// <summary>
    /// Defines a generic game manager
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Gets the level count for the specified world
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <returns>The level count</returns>
        int GetLevelCount(string basePath, World world);

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="eventInfoData">The loaded event info data</param>
        /// <returns>The level</returns>
        Common_Lev LoadLevel(string basePath, World world, int level, EventInfoData[] eventInfoData);

        /// <summary>
        /// Saves the specified level
        /// </summary>
        /// <param name="basePath">The base game path</param>
        /// <param name="world">The world</param>
        /// <param name="level">The level</param>
        /// <param name="levelData">The common level data</param>
        void SaveLevel(string basePath, World world, int level, Common_Lev levelData);
    }
}