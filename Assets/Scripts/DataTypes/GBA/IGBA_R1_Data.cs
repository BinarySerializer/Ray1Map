namespace R1Engine
{
    /// <summary>
    /// Interface for the data of the GBA or DSi versions
    /// </summary>
    public interface IGBA_R1_Data
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        GBA_R1_LevelMapData LevelMapData { get; }

        /// <summary>
        /// The event data for the current level
        /// </summary>
        GBA_R1_LevelEventData LevelEventData { get; }


        /// <summary>
        /// The background vignette data
        /// </summary>
        GBA_R1_BackgroundVignette[] BackgroundVignettes { get; }

        /// <summary>
        /// The intro vignette data
        /// </summary>
        GBA_R1_IntroVignette[] IntroVignettes { get; }

        /// <summary>
        /// The world map vignette
        /// </summary>
        GBA_R1_WorldMapVignette WorldMapVignette { get; }


        /// <summary>
        /// The sprite palette for the current level
        /// </summary>
        /// <param name="settings">The game settings</param>
        ARGB1555Color[] GetSpritePalettes(GameSettings settings);


        /// <summary>
        /// The strings
        /// </summary>
        string[] Strings { get; }
    }
}