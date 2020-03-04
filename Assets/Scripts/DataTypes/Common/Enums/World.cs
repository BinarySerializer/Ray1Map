using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace R1Engine
{
    /// <summary>
    /// The available worlds
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum World 
    {
        Jungle, 
        Music, 
        Mountain, 
        Image, 
        Cave, 
        Cake
    }
}