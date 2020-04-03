using System;
using System.Linq;
using UnityEngine;

namespace R1Engine {
    /// <summary>
    /// Common event data
    /// </summary>
    public class Common_Event : MonoBehaviour {
        #region Event Data

        /// <summary>
        /// The event display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The event flag
        /// </summary>
        public EventFlag Flag { get; set; }

        /// <summary>
        /// The animation index to use
        /// </summary>
        public int AnimationIndex { get; set; }

        /// <summary>
        /// Animation speed
        /// </summary>
        public int AnimSpeed { get; set; } = 5;

        /// <summary>
        /// The event type
        /// </summary>
        public EventType Type { get; set; }

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

        // TODO: Use this for z sorting
        // TODO: Allow this to be edited in the editor?
        /// <summary>
        /// The event layer, used for z sorting
        /// </summary>
        public int Layer { get; set; } // <-TODO: probably unused
        public int UniqueLayer { get; set; }

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
        public Common_EventCommandCollection CommandCollection { get; set; }

        #endregion

        #region Shortcuts

        /// <summary>
        /// The editor manager
        /// </summary>
        public BaseEditorManager EditorManager => Controller.obj.levelController.EditorManager;

        #endregion

        #region Event Methods

        /// <summary>
        /// Refreshes the editor event info
        /// </summary>
        public void RefreshEditorInfo() {
            // Get the event info data
            var eventInfo = EditorManager.GetEditorEventInfo(this);
            // Set the name
            DisplayName = name = eventInfo?.DisplayName ?? $"Unknown type {Type}";

            RefreshVisuals();
            ChangeOffsetVisibility(false);
            ChangeLinksVisibility(false);
        }
        
        public void RefreshName() {
            // Get the event info data
            var eventInfo = EditorManager.GetEditorEventInfo(this);

            // Set the name
            DisplayName = name = eventInfo?.DisplayName ?? $"Unknown type {Type}";
        }

        public void RefreshFlag()
        {
            Flag = Type.GetAttribute<EventTypeInfoAttribute>()?.Flag ?? EventFlag.Normal;
        }

        public void RefreshVisuals() {
            // Get the animation info
            var animInfo = EditorManager.GetEventState(this);

            if (animInfo != null) {
                AnimationIndex = animInfo.AnimationIndex;
                AnimSpeed = animInfo.AnimationSpeed;
            }
            else
            {
                Debug.LogWarning($"No matching event state found for event of type {Type}");
            }

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
        /// Is the appearance supposed to be flipped?
        /// </summary>
        public bool Mirrored;

        /// <summary>
        /// The current animation of this event
        /// </summary>
        public Common_Animation CurrentAnimation;

        // Current frame in the animation
        [HideInInspector]
        public float currentFrame = 0;

        // Default sprite
        public SpriteRenderer defautRenderer;
        // Reference to spritepart prefab
        public GameObject prefabSpritepart;
        // Reference to the created renderers
        public SpriteRenderer[] prefabRendereds;
        // Reference to box collider
        public BoxCollider2D boxCollider;
        // Reference to line renderer
        public LineRenderer lineRend;
        // Reference to link cube
        public Transform linkCube;
        [HideInInspector]
        public Vector2 linkCubeLockPosition;
        // Reference to offset crosses
        public Transform offsetOrigin;
        public Transform offsetCrossBX;
        public Transform offsetCrossBY;
        public Transform offsetCrossHY;
        // Part parent
        //public Transform partParent;
        // Midpoint of this event when taking all the spriteparts into account
        [HideInInspector]
        public Vector2 midpoint;

        private void Start() {
            //Snap link cube position
            linkCube.position = new Vector2(Mathf.FloorToInt(linkCube.position.x), Mathf.FloorToInt(linkCube.position.y));
        }

        void Update() {

            if (Controller.obj?.levelController?.currentLevel == null)
                return;

            // Scroll through animation frames
            if (CurrentAnimation != null && prefabRendereds != null) {
                if (prefabRendereds.Length > 0) {
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
            }

            //Change collider with show always/editor events
            boxCollider.enabled = !(Flag == EventFlag.Always && !Settings.ShowAlwaysEvents) && !(Flag == EventFlag.Editor && !Settings.ShowEditorEvents);

            //New midpoint
            midpoint = new Vector3(transform.position.x + boxCollider.offset.x, transform.position.y + boxCollider.offset.y, 0);
            //Link line to cube
            lineRend.SetPosition(0, midpoint);
            lineRend.SetPosition(1, linkCube.position);
        }

        public void UpdateXAndY() {
            transform.position = new Vector3(XPosition / 16f, -(YPosition / 16f), 0);
            //Don't move link cube if it's part of a link
            if (LinkID != 0) {
                linkCube.position = linkCubeLockPosition;
            }
            else {
                linkCubeLockPosition = linkCube.position;
            }
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
            UpdateOffsetPoints();
        }

        public void UpdateOffsetPoints() {
            if (CurrentAnimation != null) {
                offsetCrossBX.localPosition = new Vector2(OffsetBX / 16, 0);
                offsetCrossBY.localPosition = new Vector2(OffsetBX / 16, -(OffsetBY / 16));
                offsetCrossHY.localPosition = new Vector2(OffsetBX / 16, -((OffsetHY / 16) + (CurrentAnimation.DefaultFrameYPosition / 16f)));
            }
        }

        public void ChangeOffsetVisibility(bool visible) {
            offsetOrigin.gameObject.SetActive(visible);
            offsetCrossBX.gameObject.SetActive(visible);
            offsetCrossBY.gameObject.SetActive(visible);
            offsetCrossHY.gameObject.SetActive(visible);
        }

        public void ChangeLinksVisibility(bool visible) {
            if (visible) {
                if (Flag == EventFlag.Always && !Settings.ShowAlwaysEvents) {
                    visible = false;
                }
                else if (Flag == EventFlag.Editor && !Settings.ShowEditorEvents) {
                    visible = false;
                }
                //Change link colours
                if (LinkID == 0) {
                    lineRend.startColor = Controller.obj.levelEventController.linkColorDeactive;
                    lineRend.endColor = Controller.obj.levelEventController.linkColorDeactive;
                    linkCube.GetComponent<SpriteRenderer>().color = Controller.obj.levelEventController.linkColorDeactive;
                }
                else {
                    lineRend.startColor = Controller.obj.levelEventController.linkColorActive;
                    lineRend.endColor = Controller.obj.levelEventController.linkColorActive;
                    linkCube.GetComponent<SpriteRenderer>().color = Controller.obj.levelEventController.linkColorActive;
                }
            }
            lineRend.enabled = visible;
            linkCube.gameObject.SetActive(visible);
        }

        // Try to load a new animation and change to it
        public void ChangeAnimation(int newAnim) {
            // Make sure we have a non-negative DES index
            if (DES < 0) {
                Debug.LogWarning($"DES index is below 0");
                return;
            }

            // Get the current animation
            CurrentAnimation = EditorManager?.GetCommonDesign(this, DES)?.Animations.ElementAtOrDefault(newAnim);

            // If animation is null, use default
            if (CurrentAnimation == null) {
                defautRenderer.enabled = true;
                ClearChildren();
                return;
            }
            else {
                defautRenderer.enabled = false;
            }


            // Get the frame length
            var len = CurrentAnimation.Frames.GetLength(1);

            // Clear old array
            ClearChildren();

            // Create array
            prefabRendereds = new SpriteRenderer[len];

            // Populate it with empty ones
            for (int i = 0; i < len; i++) {
                // Instantiate prefab
                SpriteRenderer newRenderer = Instantiate<GameObject>(prefabSpritepart, new Vector3(0, 0, len-i), Quaternion.identity).GetComponent<SpriteRenderer>();
                newRenderer.sortingOrder = UniqueLayer;

                // Set as child of events gameobject
                newRenderer.gameObject.transform.parent = transform;

                // Add to list
                prefabRendereds[i] = newRenderer;
            }
        }

        private void ClearChildren() {
            // Clear old array
            if (prefabRendereds != null) {
                foreach (SpriteRenderer t in prefabRendereds)
                    Destroy(t.gameObject);

                Array.Clear(prefabRendereds, 0, prefabRendereds.Length);
                prefabRendereds = null;
            }
        }

        // Change collider size
        private void ChangeColliderSize() {
            if (CurrentAnimation != null) {
                var w = CurrentAnimation.DefaultFrameWidth / 16f;
                var h = CurrentAnimation.DefaultFrameHeight / 16f;
                boxCollider.size = new Vector2(w, h);
                boxCollider.offset = new Vector2((CurrentAnimation.DefaultFrameXPosition / 16f) + w / 2f, -((CurrentAnimation.DefaultFrameYPosition / 16f) + h / 2f));
            }
            else {
                boxCollider.size = new Vector2(3, 3);
                boxCollider.offset = new Vector2(0,0);
            }
        }

        // Update all child sprite parts
        private void UpdateParts(int frame) {
            // Make sure the current animation is not null
            if (CurrentAnimation == null)
                return;

            // Get the sprites
            var sprites = EditorManager.GetCommonDesign(this, DES).Sprites;

            for (int i = 0; i < CurrentAnimation.Frames.GetLength(1); i++) {
                // Skips sprites out of bounds
                if (CurrentAnimation.Frames[frame, i].SpriteIndex >= sprites.Count) {
                    prefabRendereds[i].sprite = null;
                }
                else {
                    prefabRendereds[i].sprite = sprites[CurrentAnimation.Frames[frame, i].SpriteIndex];
                }
                //Hardcoded flipping
                Mirrored = (Type == EventType.TYPE_PUNAISE3 && HitPoints == 1);
                prefabRendereds[i].flipX = CurrentAnimation.Frames[frame, i].Flipped || Mirrored;

                var w = prefabRendereds[i].sprite == null ? 0 : prefabRendereds[i].sprite.texture.width;
                var xx = (Mirrored 
                    ? (CurrentAnimation.DefaultFrameWidth - (CurrentAnimation.Frames[frame, i].X) - 1) + CurrentAnimation.DefaultFrameXPosition * 2 - 2
                    : CurrentAnimation.Frames[frame, i].X) + (CurrentAnimation.Frames[frame, i].Flipped ? w : 0);
                var yy = -CurrentAnimation.Frames[frame, i].Y;
                prefabRendereds[i].transform.localPosition = new Vector3(xx / 16f, yy / 16f, prefabRendereds[i].transform.localPosition.z);

                // Change visibility if always/editor
                prefabRendereds[i].enabled = !(Flag == EventFlag.Always && !Settings.ShowAlwaysEvents) && !(Flag == EventFlag.Editor && !Settings.ShowEditorEvents);
            }
        }

        // Delete this event properly
        public void Delete() {
            // Remove this from the event list
            Controller.obj.levelController.currentLevel.Events.Remove(this);
            // Remove all children
            ClearChildren();
            // Remove self
            Destroy(gameObject);
        }
    }
}