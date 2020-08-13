using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace R1Engine
{
    /// <summary>
    /// The available worlds in Rayman 1
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum R1_World 
    {
        Jungle = 1,
        Music = 2,
        Mountain = 3,
        Image = 4,
        Cave = 5,
        Cake = 6,

        Menu = 7,
        
        // GBA only
        Multiplayer = 8
    }
}