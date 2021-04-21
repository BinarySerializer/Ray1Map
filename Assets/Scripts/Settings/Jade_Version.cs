using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace R1Engine
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Jade_Version
    {
        Default,
        Xenon,

        Montreal
    }
}