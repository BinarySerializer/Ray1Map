using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace R1Engine
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Jade_Version
    {
        Montreal
    }
}