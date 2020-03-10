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
        /// <summary>
        /// Rayman 1 (PS1)
        /// </summary>
        RayPS1,

        // NOTE: Already works with level files, but world file is different - uses raw 1555 for tiles
        /// <summary>
        /// Rayman 1 (PS1 - Japanese)
        /// </summary>
        RayPS1JP,

        // NOTE: More or less same format as PS1, but split into multiple files. DTA is the event block.
        /// <summary>
        /// Rayman 1 (Saturn)
        /// </summary>
        RaySaturn,

        /// <summary>
        /// Rayman 1 (PC)
        /// </summary>
        RayPC,

        /// <summary>
        /// Rayman Designer + spin-offs (PC)
        /// </summary>
        RayKit,

        /// <summary>
        /// Educational Rayman games (PC)
        /// </summary>
        RayEduPC,

        /// <summary>
        /// Rayman Ultimate (Pocket PC)
        /// </summary>
        RayPocketPC,
    }
}