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
        public byte[] Commands { get; set; }

        /// <summary>
        /// The parsed event commands
        /// </summary>
        public string[] ParsedCommands { get; set; }

        #endregion

        #region Event Methods

        /// <summary>
        /// Refreshes the editor event info
        /// </summary>
        public void RefreshEditorInfo()
        {
            // TODO: All these don't need to be called for every change

            // Get the event info data
            var eventInfo = Settings.GetGameManager.GetEditorEventInfo(Settings.GetGameSettings, this);

            // Set the name
            DisplayName = name = eventInfo?.DisplayName ?? $"Unknown type {Type}";

            // Set the flag
            Flag = eventInfo?.Flag;

            // Refresh parsed commands
            ParsedCommands = EventInfoManager.ParseCommands(Commands, LabelOffsets);

            // Get the animation index
            AnimationIndex = Settings.GetGameManager.GetAnimationIndex(Settings.GetGameSettings, this);

            // Update the appearance
            ChangeAppearance(DES, AnimationIndex);
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

        private int desOld;
        private int animationIndexOld;

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
        // Midpoint of this event when taking all the spriteparts into account
        public Vector2 midpoint;

        void Update()
        {

            if (Controller.obj?.levelController?.currentLevel == null)
                return;

            // Change appearance on the fly
            if (DES != desOld || animationIndexOld != AnimationIndex) {
                if (DES < 1) {
                    Debug.LogWarning("Trying to set an out of range DES");
                    DES = 1;
                    desOld = 1;
                }else if(DES > Controller.obj.levelController.eventDesigns.Count) {
                    Debug.LogWarning("Trying to set an out of range DES");
                    DES = Controller.obj.levelController.eventDesigns.Count;
                    desOld = Controller.obj.levelController.eventDesigns.Count;
                }
                else if (AnimationIndex<0) {
                    Debug.LogWarning("Trying to set an out of range AnimationIndex");
                    AnimationIndex = 0;
                    animationIndexOld = 0;
                }else if(AnimationIndex > Controller.obj.levelController.eventDesigns[DES - 1].Animations.Count-1) {
                    Debug.LogWarning("Trying to set an out of range AnimationIndex");
                    if (Controller.obj.levelController.eventDesigns[DES - 1].Animations.Count == 0) {
                        AnimationIndex = 0;
                        animationIndexOld = 0;
                    }
                    else {
                        AnimationIndex = Controller.obj.levelController.eventDesigns[DES - 1].Animations.Count - 1;
                        animationIndexOld = Controller.obj.levelController.eventDesigns[DES - 1].Animations.Count - 1;
                    }
                }
                ChangeAppearance(DES, AnimationIndex);
            }

            // Update Event's x and y here
            if (transform.hasChanged)
            {
                transform.position = new Vector3(Mathf.Clamp(XPosition / 16f, 0, Controller.obj.levelController.currentLevel.Width), Mathf.Clamp(-(YPosition / 16f), -Controller.obj.levelController.currentLevel.Height, 0), transform.position.z);
                midpoint = new Vector3(transform.position.x + boxCollider.offset.x, transform.position.y + boxCollider.offset.y, 0);
            }

            // Scroll through animation frames
            if (prefabRendereds.Length > 0 && CurrentAnimation != null)
            {
                // Scroll through the frames
                if (Settings.AnimateSprites)
                    currentFrame += (60f / AnimSpeed) * Time.deltaTime;
                if (currentFrame >= CurrentAnimation.Frames.GetLength(0))
                    currentFrame = 0;

                // Update child renderers with correct part and position
                int floored = Mathf.FloorToInt(currentFrame);
                UpdateParts(floored);
            }

            //Change collider with show always/editor events
            boxCollider.enabled = !(Flag == EventFlag.Always && !Settings.ShowAlwaysEvents) && !(Flag == EventFlag.Editor && !Settings.ShowEditorEvents);

            //Link lines
            if (LinkIndex != Controller.obj.levelController.currentLevel.Events.IndexOf(this)) {
                lineRend.SetPosition(0, midpoint);
                var linkedEvent = Controller.obj.levelController.currentLevel.Events[LinkIndex];
                lineRend.SetPosition(1, linkedEvent.midpoint);
            }
        }

        // Change des and everything
        private void ChangeAppearance(int newDes, int newAnimation) {
            DES = newDes;
            AnimationIndex = newAnimation;
            desOld = newDes;
            animationIndexOld = AnimationIndex;

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
                        SpriteRenderer newRenderer = Instantiate(prefabSpritepart, new Vector3(0, 0, 5f), Quaternion.identity).GetComponent<SpriteRenderer>();
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
                //Set box collider size to be the combination of all parts
                int leftX = 0, topY = 0, rightX = 0, bottomY = 0;
                for (int i = 0; i < CurrentAnimation.Frames.GetLength(1); i++) {
                    var frame = CurrentAnimation.Frames[0, i];
                    //Skips sprites out of bounds
                    if (CurrentAnimation.Frames[0, i].SpriteIndex < Controller.obj.levelController.eventDesigns[DES - 1].Sprites.Count) {
                        var sprite = Controller.obj.levelController.eventDesigns[DES - 1].Sprites[CurrentAnimation.Frames[0, i].SpriteIndex];
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
                }

                boxCollider.size = new Vector2((rightX - leftX) / 16f, (bottomY - topY) / 16f);
                boxCollider.offset = new Vector2(leftX / 16f + ((rightX - leftX) / 16f) / 2f, -topY / 16f - ((bottomY - topY) / 16f) / 2f);
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
    }
}