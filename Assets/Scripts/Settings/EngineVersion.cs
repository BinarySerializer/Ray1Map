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
        /// <summary>
        /// Rayman 1 (PS1)
        /// </summary>
        RayPS1,

        /// <summary>
        /// Rayman 2 (PS1 - Demo)
        /// </summary>
        Ray2PS1,

        /// <summary>
        /// Rayman 1 (PS1 - JP)
        /// </summary>
        RayPS1JP,

        /// <summary>
        /// Rayman 1 (PS1 - JP Demo vol. 3)
        /// </summary>
        RayPS1JPDemoVol3,

        /// <summary>
        /// Rayman 1 (PS1 - JP Demo vol. 6)
        /// </summary>
        RayPS1JPDemoVol6,

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
        RayKitPC,

        /// <summary>
        /// Educational Rayman games (PC)
        /// </summary>
        RayEduPC,

        /// <summary>
        /// Educational Rayman games (PC)
        /// </summary>
        RayEduPS1,

        /// <summary>
        /// Rayman Ultimate (Pocket PC)
        /// </summary>
        RayPocketPC,

        /// <summary>
        /// Rayman Advance (GBA)
        /// </summary>
        RayGBA,
    }
}