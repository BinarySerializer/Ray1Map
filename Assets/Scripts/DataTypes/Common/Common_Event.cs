using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Common event data
    /// </summary>
    public class Common_Event : MonoBehaviour
    {
        /// <summary>
        /// Gets the display name based on world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The display name</returns>
        public string DisplayName(World world)
        {
            try
            {
                return EventInfoData?.Names[world].DesignerName ?? EventInfoData?.Names[world].CustomName ?? EventInfoData?.Type.ToString() ?? "N/A";
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error when getting event display name for type {EventInfoData?.Type}: {ex.Message}");

                return "N/A";
            }
        }

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
        /// DES
        /// </summary>
        public uint Des;

        /// <summary>
        /// ETA
        /// </summary>
        public uint Eta;

        public int DefaultAnimation;

        public int Speed;

        /// <summary>
        /// The event current animation of this event
        /// </summary>
        public Common_Animation CurrentAnimation;

        // Current frame in the animation
        public float currentFrame = 0;

        // Reference to spritepart prefab
        public GameObject prefabSpritepart;
        // Reference to the created renderers
        public SpriteRenderer[] prefabRendereds;

        private void Start()
        {

            name = DisplayName(Settings.World);

            // Try to find the correct animation
            if (DefaultAnimation >= 0 && Controller.obj.levelController.currentDesigns.Count > (int)Des - 1 && Controller.obj.levelController.currentDesigns[(int)Des - 1].Animations.Count > DefaultAnimation)
            {
                CurrentAnimation = Controller.obj.levelController.currentDesigns[(int)Des - 1].Animations[DefaultAnimation];

                if (CurrentAnimation != null)
                {
                    var len = CurrentAnimation.Frames.GetLength(1);
                    // Create array
                    prefabRendereds = new SpriteRenderer[len];
                    // Populate it with empty ones
                    for (int i = 0; i < len; i++)
                    {
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
            else
            {
                Debug.LogWarning($"Error loading animation for event type {EventInfoData.Type} with name {DisplayName(Settings.World)}");
            }
        }

        void Update()
        {

            if (Controller.obj?.levelController?.currentLevel == null)
                return;

            // Update Event's x and y here
            if (transform.hasChanged)
            {
                transform.position = new Vector3(Mathf.Clamp(XPosition / 16f, 0, Controller.obj.levelController.currentLevel.Width), Mathf.Clamp(-(YPosition / 16f), -Controller.obj.levelController.currentLevel.Height, 0), transform.position.z);
            }

            if (prefabRendereds.Length > 0 && CurrentAnimation != null && Settings.AnimateSprites && DefaultAnimation >= 0)
            {
                // Scroll through the frames        
                // TODO: Fix the speed
                currentFrame += Speed / 60f;
                if (currentFrame >= CurrentAnimation.Frames.GetLength(0))
                    currentFrame = 0;

                // Update child renderers with correct part and position
                // TODO: I will refactor and make this a lot cleaner -Ryemanni
                int floored = Mathf.FloorToInt(currentFrame);
                for (int i = 0; i < CurrentAnimation.Frames.GetLength(1); i++)
                {
                    prefabRendereds[i].sprite = Controller.obj.levelController.currentDesigns[(int)Des - 1].Sprites[CurrentAnimation.Frames[floored, i].SpriteIndex];
                    prefabRendereds[i].flipX = CurrentAnimation.Frames[floored, i].Flipped;

                    var extraX = prefabRendereds[i].sprite.texture.width;
                    prefabRendereds[i].transform.localPosition = new Vector3((CurrentAnimation.Frames[floored, i].X + (prefabRendereds[i].flipX ? extraX : 0)) / 16f, -(CurrentAnimation.Frames[floored, i].Y / 16f), 0);
                }
            }
        }
    }
}