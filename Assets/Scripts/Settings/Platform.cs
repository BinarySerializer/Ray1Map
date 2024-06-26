using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ray1Map
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
        PS3,
        PSP,
        iOS,
        Switch,
    }
}