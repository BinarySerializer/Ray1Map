using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace R1Engine
{
    /// <summary>
    /// The available game modes
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameMode
    {
        [Description("Rayman 1 (PS1)")]
        RaymanPS1,

        // NOTE: Already works with level files, but world file is different - uses raw 1555 for tiles
        //[Description("Rayman 1 (PS1 - Japanese)")]
        //RaymanPS1JP,

        // NOTE: More or less same format as PS1, but split into multiple files. DTA is the event block.
        //[Description("Rayman 1 (Saturn)")]
        //RaymanSaturn,

        [Description("Rayman 1 (PC)")]
        RaymanPC,

        [Description("Rayman Designer (PC)")]
        RaymanDesignerPC,

        // NOTE: Same as Designer
        //[Description("Rayman by his Fans (PC)")]
        //RaymanByHisFansPC,

        // NOTE: Same as Designer
        //[Description("Rayman 60 Levels (PC)")]
        //Rayman60LevelsPC,
    }
}