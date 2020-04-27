using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// Common level data
    /// </summary>
    public class Common_Lev
    {
        // TODO: Replace this with toggle in editor
        public int DefaultMap { get; set; }

        /// <summary>
        /// The level maps
        /// </summary>
        public Common_LevelMap[] Maps { get; set; }

        /// <summary>
        /// The event data for every event
        /// </summary>
        public List<Common_EventData> EventData { get; set; }
    }
}