using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine {
    [System.Serializable]
    public class Common_Design {

        /// <summary>
        /// The sprites used by this design
        /// </summary>
        public List<Sprite> Sprites;

        /// <summary>
        /// The animations in this design
        /// </summary>
        public List<Common_Animation> Animations;
    }
}