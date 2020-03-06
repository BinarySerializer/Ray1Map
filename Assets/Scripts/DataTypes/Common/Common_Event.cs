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
        /// Offset HY
        /// </summary>
        public int OffsetHY;

        /// <summary>
        /// Gets the display name based on world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The display name</returns>
        public string DisplayName(World world) => EventInfoData?.Names[world].DesignerName ?? EventInfoData?.Names[world].CustomName ?? EventInfoData?.Type.ToString() ?? "N/A";

        // Used for displaying one sprite isntead o
        public Sprite tempSprite;

        public void SetSprite(Texture2D texture) {
            tempSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 1f), 16, 20);
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            sr.sprite = tempSprite;
            //Change collider size
            BoxCollider2D bx = GetComponentInChildren<BoxCollider2D>();
            bx.size = new Vector2(texture.width/16f, texture.height/16f);
            bx.offset = new Vector2((texture.width / 16f) / 2f, -(texture.height / 16f) / 2f);
        }
        public void SetSprite(Sprite sprt) {
            tempSprite = sprt;
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            sr.sprite = tempSprite;
        }

        // TODO: Update here is just a quick and dirty way to update the X and Y
        // Most likely not the most efficent way when a level has a lot of events
        void Update() {
            if (transform.hasChanged) {
                transform.position = new Vector3(XPosition / 16f, -(YPosition / 16f), transform.position.z);
            }
        }
    }
}