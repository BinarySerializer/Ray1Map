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
        /// The event sprite
        /// </summary>
        public Common_Animation CommonAnimation;

        /// <summary>
        /// Gets the display name based on world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The display name</returns>
        public string DisplayName(World world) => EventInfoData?.Names[world].DesignerName ?? EventInfoData?.Names[world].CustomName ?? EventInfoData?.Type.ToString() ?? "N/A";

        // Current frame in the animation
        public float currentFrame = 0;

        // Reference to spritepart prefab
        public GameObject prefabSpritepart;

        public SpriteRenderer[] prefabRendereds;

        private void Start() {
            if (CommonAnimation != null) {
                // Create array
                prefabRendereds = new SpriteRenderer[CommonAnimation.Frames.GetLength(1)];
                // Populate it with empty ones
                for (int i = 0; i < CommonAnimation.Frames.GetLength(1); i++) {
                    // Instantiate prefab
                    SpriteRenderer newRenderer = Instantiate(prefabSpritepart, new Vector3(0, 0, 5f), Quaternion.identity).GetComponent<SpriteRenderer>();
                    newRenderer.sortingOrder = i;
                    // Set as child of events gameobject
                    newRenderer.gameObject.transform.parent = gameObject.transform;
                    // Add to list
                    prefabRendereds[i] = newRenderer;
                }
            }
        }

        void Update() {

            if (Controller.obj?.levelController?.currentLevel == null)
                return;

            // Update Event's x and y here
            if (transform.hasChanged) {
                transform.position = new Vector3(Mathf.Clamp(XPosition / 16f,0,Controller.obj.levelController.currentLevel.Width), Mathf.Clamp(-(YPosition / 16f),-Controller.obj.levelController.currentLevel.Height,0), transform.position.z);
            }

            if (prefabRendereds.Length > 0 && CommonAnimation != null && Settings.AnimateSprites) {
                // Scroll through the frames           
                currentFrame += 0.1f;
                if (currentFrame >= CommonAnimation.Frames.GetLength(0))
                    currentFrame = 0;

                // Update child renderers with correct part and position
                int floored = Mathf.FloorToInt(currentFrame);
                for (int i = 0; i < CommonAnimation.Frames.GetLength(1); i++) {
                    prefabRendereds[i].sprite = CommonAnimation.Frames[floored, i].Sprite;
                    prefabRendereds[i].transform.localPosition = new Vector3(CommonAnimation.Frames[floored, i].X / 16f, -(CommonAnimation.Frames[floored, i].Y / 16f), 0);
                }
            }
        }
    }
}