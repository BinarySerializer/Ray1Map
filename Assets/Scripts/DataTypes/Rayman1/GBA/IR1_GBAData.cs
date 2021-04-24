using BinarySerializer;

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
        RGBA5551Color[] GetSpritePalettes(GameSettings settings);


        /// <summary>
        /// World level index offset table for global level array
        /// </summary>
        byte[] WorldLevelOffsetTable { get; }


        /// <summary>
        /// The strings
        /// </summary>
        string[][] Strings { get; }


        R1_ZDCEntry[] TypeZDC { get; } 

        R1_ZDCData[] ZdcData { get; }

        R1_EventFlags[] EventFlags { get; }

        byte[] WorldVignetteIndices { get; }
        R1_WorldMapInfo[] WorldInfos { get; }

        R1_GBA_EventGraphicsData DES_Ray { get; }
        R1_GBA_EventGraphicsData DES_RayLittle { get; }
        R1_GBA_EventGraphicsData DES_Clock { get; }
        R1_GBA_EventGraphicsData DES_Div { get; }
        R1_GBA_EventGraphicsData DES_Map { get; }
        R1_GBA_ETA ETA_Ray { get; }
        R1_GBA_ETA ETA_Clock { get; }
        R1_GBA_ETA ETA_Div { get; }
        R1_GBA_ETA ETA_Map { get; }
    }
}