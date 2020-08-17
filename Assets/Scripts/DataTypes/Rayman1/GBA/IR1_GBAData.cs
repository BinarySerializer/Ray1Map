namespace R1Engine
{
    /// <summary>
    /// Interface for the data of the GBA or DSi versions
    /// </summary>
    public interface IR1_GBAData
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        R1_GBA_LevelMapData LevelMapData { get; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        R1_GBA_LevelEventData LevelEventData { get; }


        /// <summary>
        /// The background vignette data
        /// </summary>
        R1_GBA_BackgroundVignette[] BackgroundVignettes { get; }

        /// <summary>
        /// The intro vignette data
        /// </summary>
        R1_GBA_IntroVignette[] IntroVignettes { get; }

        /// <summary>
        /// The world map vignette
        /// </summary>
        R1_GBA_WorldMapVignette WorldMapVignette { get; }


        /// <summary>
        /// The sprite palette for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        ARGB1555Color[] GetSpritePalettes(GameSettings settings);


        /// <summary>
        /// World level index offset table for global level array
        /// </summary>
        byte[] WorldLevelOffsetTable { get; }


        /// <summary>
        /// The strings
        /// </summary>
        string[][] Strings { get; }
    }
}