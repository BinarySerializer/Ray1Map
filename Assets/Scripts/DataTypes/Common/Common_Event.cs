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

        /// <summary>
        /// The link index
        /// </summary>
        public int LinkIndex;

        /// <summary>
        /// Offset BX
        /// </summary>
        public int OffsetBX;

        /// <summary>
        /// Offset BY
        /// </summary>
        public int OffsetBY;

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName => EventInfoData.DesignerName ?? EventInfoData.CustomName;

        // TODO: Update here is just a quick and dirty way to update the X and Y
        // Most likely not the most efficent way when a level has a lot of events
        void Update() {
            if (transform.hasChanged) {
                transform.position = new Vector3(XPosition / 16f, -(YPosition / 16f), transform.position.z);
            }
        }
    }
}