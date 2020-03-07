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

        // Animation frames
        public Sprite[] animationFrames;
        public float currentFrame = 0;

        // References to certain components with this prefab
        public SpriteRenderer spriteRendered;
        public BoxCollider2D boxCollider;

        public void SetSprite(Texture2D[] textures) {
            animationFrames = new Sprite[textures.Length];
            currentFrame = 0;

            for (int i=0; i<textures.Length; i++) {
                animationFrames[i] = Sprite.Create(textures[i], new Rect(0, 0, textures[i].width, textures[i].height), new Vector2(0f, 1f), 16, 20);
            }

            // Resize box collider
            boxCollider.size = new Vector2(textures[0].width/16f, textures[0].height/16f);
            boxCollider.offset = new Vector2((textures[0].width / 16f) / 2f, -(textures[0].height / 16f) / 2f);
        }

        void Update() {
            // TODO: Update here is just a quick and dirty way to update the X and Y
            // Most likely not the most efficent way when a level has a lot of events
            if (transform.hasChanged) {
                transform.position = new Vector3(Mathf.Clamp(XPosition / 16f,0,Controller.obj.levelController.currentLevel.Width), Mathf.Clamp(-(YPosition / 16f),-Controller.obj.levelController.currentLevel.Height,0), transform.position.z);
            }

            // Scroll through the frames
            if (animationFrames.Length > 0) {
                currentFrame+=0.6f;
                if (currentFrame >= animationFrames.Length)
                    currentFrame = 0;

                spriteRendered.sprite = animationFrames[Mathf.FloorToInt(currentFrame)];
            }
        }
    }
}