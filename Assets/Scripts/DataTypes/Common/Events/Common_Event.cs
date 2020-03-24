using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Common event data
    /// </summary>
    public class Common_Event : MonoBehaviour
    {
        #region Event Data

        /// <summary>
        /// The event display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The event flag
        /// </summary>
        public EventFlag? Flag { get; set; }

        /// <summary>
        /// The animation index to use
        /// </summary>
        public int AnimationIndex { get; set; }

        /// <summary>
        /// Animation speed
        /// </summary>
        public int AnimSpeed { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// The event state
        /// </summary>
        public int Etat { get; set; }

        /// <summary>
        /// The event sub-state
        /// </summary>
        public int SubEtat { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public uint XPosition { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public uint YPosition { get; set; }

        /// <summary>
        /// The event design index
        /// </summary>
        public int DES { get; set; }

        // TODO: PC only?
        /// <summary>
        /// The event ETA index
        /// </summary>
        public int ETA { get; set; }

        /// <summary>
        /// The event offset BX
        /// </summary>
        public int OffsetBX { get; set; }

        /// <summary>
        /// The event offset BY
        /// </summary>
        public int OffsetBY { get; set; }

        /// <summary>
        /// The event offset HY
        /// </summary>
        public int OffsetHY { get; set; }

        public int FollowSprite { get; set; }

        /// <summary>
        /// The event hit-points
        /// </summary>
        public int HitPoints { get; set; }

        public int HitSprite { get; set; }

        /// <summary>
        /// Indicates if the event has collision
        /// </summary>
        public bool FollowEnabled { get; set; }

        /// <summary>
        /// The label offsets
        /// </summary>
        public ushort[] LabelOffsets { get; set; }

        /// <summary>
        /// The event commands
        /// </summary>
        public Common_EventCommandCollection Commands { get; set; }

        #endregion

        #region Event Methods

        /// <summary>
        /// Refreshes the editor event info
        /// </summary>
        public void RefreshEditorInfo()
        {
            // Get the event info data
            var eventInfo = Settings.GetGameManager.GetEditorEventInfo(Settings.GetGameSettings, this);
            // Set the name
            DisplayName = name = eventInfo?.DisplayName ?? $"Unknown type {Type}";
            // Set the flag
            Flag = eventInfo?.Flag;

            RefreshVisuals();
        }
        public void RefreshName() {
            // Get the event info data
            var eventInfo = Settings.GetGameManager.GetEditorEventInfo(Settings.GetGameSettings, this);

            // Set the name
            DisplayName = name = eventInfo?.DisplayName ?? $"Unknown type {Type}";
        }
        public void RefreshVisuals() {
            // Get the animation info
            var animInfo = Settings.GetGameManager.GetAnimationInfo(Controller.MainContext, this);

            if (animInfo.AnimationIndex != -1)
                AnimationIndex = animInfo.AnimationIndex;

            if (animInfo.AnimationSpeed != -1)
                AnimSpeed = animInfo.AnimationSpeed;

            ChangeAppearance();
        }

        #endregion

        /// <summary>
        /// The link index
        /// </summary>
        public int LinkIndex;

        /// <summary>
        /// The link ID used by the editor
        /// </summary>
        public int LinkID;

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
        // Reference to line renderer
        public LineRenderer lineRend;
        // Reference to offset crosses
        public Transform offsetCrossBX;
        public Transform offsetCrossBY;
        public Transform offsetCrossHY;
        // Midpoint of this event when taking all the spriteparts into account
        [HideInInspector]
        public Vector2 midpoint;

        void Update()
        {

            if (Controller.obj?.levelController?.currentLevel == null)
                return;

            // Scroll through animation frames
            if (prefabRendereds.Length > 0 && CurrentAnimation != null)
            {
                int prevFrame = Mathf.FloorToInt(currentFrame);
                // Scroll through the frames
                if (Settings.AnimateSprites)
                    currentFrame += (60f / AnimSpeed) * Time.deltaTime;
                if (currentFrame >= CurrentAnimation.Frames.GetLength(0))
                    currentFrame = 0;

                int floored = Mathf.FloorToInt(currentFrame);

                // Update child renderers with correct part and position, but only if current frame has updated
                if (floored != prevFrame) {
                    UpdateParts(floored);
                }
            }

            //Change collider with show always/editor events
            boxCollider.enabled = !(Flag == EventFlag.Always && !Settings.ShowAlwaysEvents) && !(Flag == EventFlag.Editor && !Settings.ShowEditorEvents);

            //Link lines
            /*
            if (LinkIndex != Controller.obj.levelController.currentLevel.Events.IndexOf(this)) {
                lineRend.SetPosition(0, midpoint);
                var linkedEvent = Controller.obj.levelController.currentLevel.Events[LinkIndex];
                lineRend.SetPosition(1, linkedEvent.midpoint);
            }*/
        }

        public void UpdateXAndY() {
            transform.position = new Vector3(Mathf.Clamp(XPosition / 16f, 0, Controller.obj.levelController.currentLevel.Width), Mathf.Clamp(-(YPosition / 16f), -Controller.obj.levelController.currentLevel.Height, 0), transform.position.z);
            midpoint = new Vector3(transform.position.x + boxCollider.offset.x, transform.position.y + boxCollider.offset.y, 0);
        }

        // Change des and everything
        private void ChangeAppearance() {
            // Change to new animation
            ChangeAnimation(AnimationIndex);

            // Update parts for the first time
            currentFrame = 0;
            UpdateParts(0);

            // Collider
            ChangeColliderSize();

            //Offset points
            ChangeOffsetPoints();
        }

        private void ChangeOffsetPoints() {
            if (CurrentAnimation != null) {
                offsetCrossBX.localPosition = new Vector2(OffsetBX / 16, 0);
                offsetCrossBY.localPosition = new Vector2(OffsetBX / 16, -(OffsetBY / 16));
                offsetCrossHY.localPosition = new Vector2(OffsetBX / 16, -((OffsetHY / 16) + (CurrentAnimation.DefaultFrameYPosition / 16f)));
            }
        }

        // Try to load a new animation and change to it
        private void ChangeAnimation(int newAnim) {

            var desIndex = DES - 1;

            if (desIndex < 0)
            {
                Debug.LogWarning($"DES index is below 0");
                return;
            }

            if (Controller.obj.levelController.eventDesigns.Count > desIndex && Controller.obj.levelController.eventDesigns[desIndex].Animations.Count > newAnim)
            {

                CurrentAnimation = Controller.obj.levelController.eventDesigns[desIndex].Animations[newAnim];

                if (CurrentAnimation != null)
                {
                    var len = CurrentAnimation.Frames.GetLength(1);
                    // Clear old array
                    if (prefabRendereds != null)
                    {
                        for (int i = 0; i < prefabRendereds.Length; i++)
                        {
                            Destroy(prefabRendereds[i].gameObject);
                        }
                        Array.Clear(prefabRendereds, 0, prefabRendereds.Length);
                    }

                    // Create array
                    prefabRendereds = new SpriteRenderer[len];
                    // Populate it with empty ones
                    for (int i = 0; i < len; i++)
                    {
                        // Instantiate prefab
                        SpriteRenderer newRenderer = Instantiate<GameObject>(prefabSpritepart, new Vector3(0, 0, 5f), Quaternion.identity).GetComponent<SpriteRenderer>();
                        newRenderer.sortingOrder = -len + i;
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
                var w = CurrentAnimation.DefaultFrameWidth / 16f;
                var h = CurrentAnimation.DefaultFrameHeight / 16f;
                boxCollider.size = new Vector2(w,h);
                boxCollider.offset = new Vector2((CurrentAnimation.DefaultFrameXPosition/16f)+w/2f,-((CurrentAnimation.DefaultFrameYPosition/16f)+h/2f));
            }
        }

        // Update all child sprite parts
        private void UpdateParts(int frame) {
            if (CurrentAnimation != null) {
                for (int i = 0; i < CurrentAnimation.Frames.GetLength(1); i++) {
                    //Skips sprites out of bounds
                    if (CurrentAnimation.Frames[frame, i].SpriteIndex >= Controller.obj.levelController.eventDesigns[DES - 1].Sprites.Count) {
                        prefabRendereds[i].sprite = null;
                    }
                    else {
                        prefabRendereds[i].sprite = Controller.obj.levelController.eventDesigns[DES - 1].Sprites[CurrentAnimation.Frames[frame, i].SpriteIndex];
                    }
                    prefabRendereds[i].flipX = CurrentAnimation.Frames[frame, i].Flipped;

                    var extraX = prefabRendereds[i].sprite==null ? 0 : prefabRendereds[i].sprite.texture.width;
                    prefabRendereds[i].transform.localPosition = new Vector3((CurrentAnimation.Frames[frame, i].X + (prefabRendereds[i].flipX ? extraX : 0)) / 16f, -(CurrentAnimation.Frames[frame, i].Y / 16f), 0);

                    //Change visibility if always/editor
                    prefabRendereds[i].enabled = !(Flag == EventFlag.Always && !Settings.ShowAlwaysEvents) && !(Flag == EventFlag.Editor && !Settings.ShowEditorEvents);
                }
            }
        }

        // Delete this event properly
        public void Delete() {
            // Remove this from the event list
            Controller.obj.levelController.currentLevel.Events.Remove(this);
            // Remove all children
            if (prefabRendereds != null) {
                for (int i = 0; i < prefabRendereds.Length; i++) {
                    Destroy(prefabRendereds[i].gameObject);
                }
                Array.Clear(prefabRendereds, 0, prefabRendereds.Length);
            }
            // Remove self
            Destroy(gameObject);
        }
    }
}