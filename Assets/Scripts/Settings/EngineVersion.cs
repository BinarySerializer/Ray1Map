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

        //GBAVV_PowerRangersTimeForce,       // 2001 (early map format where objects are single array without params)
        //GBAVV_TonyHawksProSkater2,         // 2001 (TH engine)
        //GBAVV_SpiderManMysteriosMenace,    // 2001 (same as Power Rangers, has DigiBlast version)
        GBAVV_Crash1,                        // 2002
        //GBAVV_TonyHawksProSkater3,         // 2002 (TH engine)
        //GBAVV_TonyHawksProSkater4,         // 2002 (TH engine)
        GBAVV_ThePowerpuffGirlsHimAndSeek,   // 2002
        GBAVV_FroggerAdvance,                // 2002
        GBAVV_TheMuppetsOnWithTheShow,       // 2003 (maps are hard-coded, has PC version)
        GBAVV_SpyMuppetsLicenseToCroak,      // 2003 (maps are hard-coded, has PC version)
        GBAVV_Crash2,                        // 2003
        GBAVV_BruceLeeReturnOfTheLegend,     // 2003
        GBAVV_X2WolverinesRevenge,           // 2003
        GBAVV_FindingNemo,                   // 2003
        //GBAVV_JetGrindRadio,               // 2003 (TH engine)
        //GBAVV_ExtremeSkateAdventure,       // 2003 (TH engine)
        GBAVV_TheLionKing,                   // 2003
        //GBAVV_TonyHawksUnderground,        // 2003 (TH engine)
        GBAVV_BrotherBear,                   // 2003
        GBAVV_CrashNitroKart,                // 2003
        GBAVV_CrashNitroKart_NGage,          // 2004
        GBAVV_CrashFusion,                   // 2004
        GBAVV_SpyroFusion,                   // 2004
        GBAVV_SharkTale,                     // 2004
        //GBAVV_TonyHawksUnderground2,       // 2004 (TH engine)
        GBAVV_ThatsSoRaven,                  // 2004
        GBAVV_Shrek2,                        // 2004
        GBAVV_Shrek2BegForMercy,             // 2004
        GBAVV_KidsNextDoorOperationSODA,     // 2004
        GBAVV_Madagascar,                    // 2005 (has DS version)

        // Gameloft

        Gameloft_RRR,
        Gameloft_RK,

        // Jade

        Jade_RRR_PC,
        Jade_RRR_Xbox360

    }
}