using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace R1Engine
{
    /// <summary>
    /// The available game modes
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EngineVersion
    {
        // Rayman 1

        R1_PS1,
        R2_PS1,
        R1_PS1_JP,
        R1_PS1_JPDemoVol3,
        R1_PS1_JPDemoVol6,
        R1_Saturn,
        R1_PC,
        R1_PocketPC,
        R1_PC_Kit,
        R1_PC_Edu,
        R1_PS1_Edu,
        R1_GBA,
        R1_DSi,

        // Rayman 1 Jaguar

        R1Jaguar,
        R1Jaguar_Proto,
        R1Jaguar_Demo,

        // SNES

        SNES,

        // GBA

        GBA_DonaldDuck,                      // 2001 (Shanghai)
        GBA_CrouchingTiger,                  // 2003 (Shanghai)
        GBA_R3_MadTrax,                      // 2003 (Shanghai)
                                             
        GBA_TomClancysRainbowSixRogueSpear,  // 2002 (Milan)
        GBA_TheMummy,                        // 2002 (Milan)
        GBA_TombRaiderTheProphecy,           // 2002 (Milan)

        GBA_BatmanVengeance,                 // 2001
        GBA_Sabrina,                         // 2002
        GBA_R3_Proto,                        // 2003
        GBA_R3,                              // 2003
        GBA_R3_NGage,                        // 2003
        GBA_SplinterCell,                    // 2003 - released before R3, but more developed engine
        GBA_SplinterCell_NGage,              // 2003
        GBA_PrinceOfPersia,                  // 2003
        GBA_BatmanRiseOfSinTzu,              // 2003
        GBA_SplinterCellPandoraTomorrow,     // 2004
        GBA_StarWarsTrilogy,                 // 2004
        GBA_StarWarsEpisodeIII,              // 2005
        GBA_KingKong,                        // 2005
        GBA_OpenSeason,                      // 2006
        GBA_TMNT,                            // 2007
        GBA_SurfsUp,                         // 2007

        // GBA RRR

        GBARRR,

        // GBA Isometric

        GBAIsometric_Spyro1, // Season of Ice
        GBAIsometric_Spyro2, // Season of Flame
        GBAIsometric_Spyro3, // Spyro's Adventure/Attack of the Rhynocs
        GBAIsometric_Tron2,
        GBAIsometric_RHR,

        // GBC

        GBC_R1,
        GBC_R1_Palm,
        GBC_R1_PocketPC,

        // GBA VV

        GBAVV_Crash1,
        GBAVV_Crash2,

        // Gameloft

        Gameloft_RRR,
        Gameloft_RK,
    }
}