using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Common event data
    /// </summary>
    public class Common_Event : MonoBehaviour
    {
        /// <summary>
        /// The event info data
        /// </summary>
        public EventInfoData EventInfoData;

        /// <summary>
        /// The x position
        /// </summary>
        public uint XPosition;

        /// <summary>
        /// The x position
        /// </summary>
        public uint YPosition;


        public int LinkId; // 0 = no link; anything else = link group


        // TODO: Update here is just a quick and dirty way to update the X and Y
        // Most likely not the most efficent way when a level has a lot of events
        void Update() {
            if (transform.hasChanged) {
                transform.position = new Vector3(XPosition / 16f, -(YPosition / 16f), transform.position.z);
            }
        }
    }
}