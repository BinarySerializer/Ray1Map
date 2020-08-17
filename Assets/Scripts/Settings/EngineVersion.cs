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

        GBA_R3,
        GBA_R3_Proto,
        GBA_R3_NGage,
        GBA_PrinceOfPersia,
        GBA_Sabrina,
        GBA_StarWars,
        GBA_BatmanVengeance,
    }
}