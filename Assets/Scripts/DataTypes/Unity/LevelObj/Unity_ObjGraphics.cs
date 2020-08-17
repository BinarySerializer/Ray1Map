using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Common event design data
    /// </summary>
    [System.Serializable]
    public class Unity_ObjGraphics {

        /// <summary>
        /// The sprites used by this design
        /// </summary>
        public List<Sprite> Sprites;

        /// <summary>
        /// The animations in this design
        /// </summary>
        public List<Unity_ObjAnimation> Animations;

        /// <summary>
        /// The original file path for the sprite data
        /// </summary>
        public string FilePath { get; set; }
    }
}