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
                return EventInfoData?.Info.Names[world] ?? EventInfoData?.ID?.Type.ToString() ?? "N/A";
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error when getting event display name for type {EventInfoData?.ID?.Type}: {ex.Message}");

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
        private uint DesOld;

        /// <summary>
        /// ETA
        /// </summary>
        public uint Eta;

        /// <summary>
        /// Animation speed
        /// </summary>
        public int Speed;

        /// <summary>
        /// The animation index to use
        /// </summary>
        public int AnimationIndex;
        private int AnimationIndexOld;

        /// <summary>
        /// The current animation of this event
        /// </summary>
        public Common_Animation CurrentAnimation;

        // Current frame in the animation
        [HideInInspector]
        public float currentFrame = 0;

        // Reference to spritepart prefab
        public GameObject prefabSpritepart;
        // Reference to the created renderers
        public SpriteRenderer[] prefabRendereds;
        // Reference to box collider
        public BoxCollider2D boxCollider;

        private void Start()
        {
            // Set display name for this prefab
            name = DisplayName(Settings.World);

            ChangeAppearance(Des, AnimationIndex);
        }

        void Update()
        {

            if (Controller.obj?.levelController?.currentLevel == null)
                return;

            // Change appearance on the fly
            if (Des!=DesOld || AnimationIndexOld != AnimationIndex) {
                if (Des<1) {
                    Debug.LogWarning("Trying to set an out of range DES");
                    Des = 1;
                    DesOld = 1;
                }else if(Des > Controller.obj.levelController.eventDesigns.Count) {
                    Debug.LogWarning("Trying to set an out of range DES");
                    Des = (uint)Controller.obj.levelController.eventDesigns.Count;
                    DesOld = (uint)Controller.obj.levelController.eventDesigns.Count;
                }
                else if (AnimationIndex<0) {
                    Debug.LogWarning("Trying to set an out of range AnimationIndex");
                    AnimationIndex = 0;
                    AnimationIndexOld = 0;
                }else if(AnimationIndex > Controller.obj.levelController.eventDesigns[(int)Des - 1].Animations.Count-1) {
                    Debug.LogWarning("Trying to set an out of range AnimationIndex");
                    if (Controller.obj.levelController.eventDesigns[(int)Des - 1].Animations.Count == 0) {
                        AnimationIndex = 0;
                        AnimationIndexOld = 0;
                    }
                    else {
                        AnimationIndex = Controller.obj.levelController.eventDesigns[(int)Des - 1].Animations.Count - 1;
                        AnimationIndexOld = Controller.obj.levelController.eventDesigns[(int)Des - 1].Animations.Count - 1;
                    }
                }
                ChangeAppearance(Des, AnimationIndex);
            }

            // Update Event's x and y here
            if (transform.hasChanged)
            {
                transform.position = new Vector3(Mathf.Clamp(XPosition / 16f, 0, Controller.obj.levelController.currentLevel.Width), Mathf.Clamp(-(YPosition / 16f), -Controller.obj.levelController.currentLevel.Height, 0), transform.position.z);
            }

            // Scroll through animation frames
            if (prefabRendereds.Length > 0 && CurrentAnimation != null)
            {
                // Scroll through the frames
                if (Settings.AnimateSprites)
                    currentFrame += (60f / Speed) * Time.deltaTime;
                if (currentFrame >= CurrentAnimation.Frames.GetLength(0))
                    currentFrame = 0;

                // Update child renderers with correct part and position
                int floored = Mathf.FloorToInt(currentFrame);
                UpdateParts(floored);
            }

            //Change collider with show always/editor events
            boxCollider.enabled = !(EventInfoData.Info.IsAlways == true && !Settings.ShowAlwaysEvents) && !(EventInfoData.Info.EditorOnly == true && !Settings.ShowEditorEvents);
        }

        // Change des and everything
        private void ChangeAppearance(uint newDes, int newAnimation) {
            Des = newDes;
            AnimationIndex = newAnimation;
            DesOld = newDes;
            AnimationIndexOld = AnimationIndex;

            // Change to new animation
            ChangeAnimation(AnimationIndex);

            // Update parts for the first time
            currentFrame = 0;
            UpdateParts(0);

            // Collider
            ChangeColliderSize();
        }

        // Try to load a new animation and change to it
        private void ChangeAnimation(int newAnim) {
            if (Controller.obj.levelController.eventDesigns.Count > (int)Des - 1 && Controller.obj.levelController.eventDesigns[(int)Des - 1].Animations.Count > newAnim) {

                CurrentAnimation = Controller.obj.levelController.eventDesigns[(int)Des - 1].Animations[newAnim];

                if (CurrentAnimation != null) {
                    var len = CurrentAnimation.Frames.GetLength(1);
                    // Clear old array
                    if (prefabRendereds != null) {
                        for (int i=0; i<prefabRendereds.Length; i++) {
                            Destroy(prefabRendereds[i].gameObject);                       
                        }
                        Array.Clear(prefabRendereds, 0, prefabRendereds.Length);
                    }
                        
                    // Create array
                    prefabRendereds = new SpriteRenderer[len];
                    // Populate it with empty ones
                    for (int i = 0; i < len; i++) {
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
        }

        // Change collider size
        private void ChangeColliderSize() {
            if (CurrentAnimation != null) {
                //Set box collider size to be the combination of all parts
                int leftX = 0, topY = 0, rightX = 0, bottomY = 0;
                for (int i = 0; i < CurrentAnimation.Frames.GetLength(1); i++) {
                    var frame = CurrentAnimation.Frames[0, i];
                    var sprite = Controller.obj.levelController.eventDesigns[(int)Des - 1].Sprites[CurrentAnimation.Frames[0, i].SpriteIndex];
                    if (sprite != null) {
                        if (frame.X < leftX || i == 0)
                            leftX = frame.X;
                        if (frame.X + sprite.texture.width > rightX || i == 0)
                            rightX = frame.X + sprite.texture.width;
                        if (frame.Y < topY || i == 0)
                            topY = frame.Y;
                        if (frame.Y + sprite.texture.height > bottomY || i == 0)
                            bottomY = frame.Y + sprite.texture.height;
                    }
                }

                boxCollider.size = new Vector2((rightX - leftX) / 16f, (bottomY - topY) / 16f);
                boxCollider.offset = new Vector2(leftX / 16f + ((rightX - leftX) / 16f) / 2f, -topY / 16f - ((bottomY - topY) / 16f) / 2f);
            }
        }

        // Update all child sprite parts
        private void UpdateParts(int frame) {
            if (CurrentAnimation != null) {
                for (int i = 0; i < CurrentAnimation.Frames.GetLength(1); i++) {
                    prefabRendereds[i].sprite = Controller.obj.levelController.eventDesigns[(int)Des - 1].Sprites[CurrentAnimation.Frames[frame, i].SpriteIndex];
                    prefabRendereds[i].flipX = CurrentAnimation.Frames[frame, i].Flipped;

                    var extraX = prefabRendereds[i].sprite==null ? 0 : prefabRendereds[i].sprite.texture.width;
                    prefabRendereds[i].transform.localPosition = new Vector3((CurrentAnimation.Frames[frame, i].X + (prefabRendereds[i].flipX ? extraX : 0)) / 16f, -(CurrentAnimation.Frames[frame, i].Y / 16f), 0);

                    //Change visibility if always/editor
                    prefabRendereds[i].enabled = !(EventInfoData.Info.IsAlways == true && !Settings.ShowAlwaysEvents) && !(EventInfoData.Info.EditorOnly == true && !Settings.ShowEditorEvents);
                }
            }
        }
    }
}