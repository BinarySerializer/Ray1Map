using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace R1Engine
{
    /// <summary>
    /// The available platforms
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Platform
    {
        Unspecified,
        PC,

        PS1,
        Saturn,
        Jaguar,
        PocketPC,
        PalmOS,
        GBC,
        GBA,
        DS,
        SNES,
        NGage,
        Java,


        PS2,
        GC,
        Wii,
        Xbox,
        Xbox360,
        PSP,
    }
}