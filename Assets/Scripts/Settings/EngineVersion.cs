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

        GBA_BatmanVengeance,    // 2001
        GBA_Sabrina,            // 2002
        GBA_R3_Proto,           // 2003
        GBA_R3,                 // 2003
        GBA_R3_NGage,           // 2003
        GBA_PrinceOfPersia,     // 2003
        GBA_StarWarsTrilogy,    // 2004
        GBA_StarWarsEpisodeIII, // 2005 // TODO: Support
        GBA_KingKong,           // 2005 // TODO: Support
        GBA_OpenSeason,         // 2006
        GBA_TMNT,               // 2007
        GBA_SurfsUp,            // 2007
    }
}