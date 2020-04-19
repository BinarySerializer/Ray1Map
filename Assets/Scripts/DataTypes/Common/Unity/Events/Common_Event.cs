using System;
using System.Linq;
using UnityEngine;

namespace R1Engine {
    /// <summary>
    /// Common event data
    /// </summary>
    public class Common_Event : MonoBehaviour {
        #region Public Properties

        /// <summary>
        /// The event display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The event flag
        /// </summary>
        public EventFlag Flag { get; set; }

        public int UniqueLayer { get; set; }

        /// <summary>
        /// The event data
        /// </summary>
        public Common_EventData Data { get; set; }

        /// <summary>
        /// The current animation speed
        /// </summary>
        public int AnimSpeed { get; set; }

        /// <summary>
        /// The current animation index
        /// </summary>
        public int AnimationIndex { get; set; }

        /// <summary>
        /// The current state
        /// </summary>
        public Common_EventState CurrentState => States.ElementAtOrDefault(StateIndex);

        /// <summary>
        /// The states
        /// </summary>
        public Common_EventState[] States { get; set; }

        /// <summary>
        /// The index of the current state
        /// </summary>
        public int StateIndex { get; set; }

        #endregion

        #region Shortcuts

        /// <summary>
        /// The editor manager
        /// </summary>
        public BaseEditorManager EditorManager => Controller.obj.levelController.EditorManager;

        #endregion

        #region Event Methods

        public void UpdateCurrentState(int index)
        {
            // Set the index
            StateIndex = index;

            // Set the animation speed
            AnimSpeed = (Controller.CurrentSettings.EngineVersion == EngineVersion.RaySaturn ? CurrentState?.AnimationSpeed >> 4 : CurrentState?.AnimationSpeed) ?? 0;

            // Set the animation index
            AnimationIndex = CurrentState?.AnimationIndex ?? 0;

            // Update the graphics
            ChangeAppearance();
        }

        /// <summary>
        /// Refreshes the editor event info
        /// </summary>
        public void RefreshEditorInfo() {
            // Get the event info data
            var eventInfo = EditorManager.GetEditorEventInfo(Data);
            // Set the name
            DisplayName = name = eventInfo?.DisplayName ?? $"Unknown type {Data.Type}";

            RefreshVisuals();
            ChangeOffsetVisibility(false);
            ChangeLinksVisibility(false);
        }
        
        public void RefreshName() {
            // Get the event info data
            var eventInfo = EditorManager.GetEditorEventInfo(Data);

            // Set the name
            DisplayName = name = eventInfo?.DisplayName ?? $"Unknown type {Data.Type}";
        }

        public void RefreshFlag()
        {
            Flag = Data.Type.GetAttribute<EventTypeInfoAttribute>()?.Flag ?? EventFlag.Normal;
        }

        public void RefreshVisuals() {
            // Get the states
            States = EditorManager.GetEventStates(Data);

            // Update the state
            UpdateCurrentState(0);
        }

        #endregion

        /// <summary>
        /// The link ID used by the editor
        /// </summary>
        public int LinkID;

        /// <summary>
        /// Indicates if the entire event sprite is supposed to be mirrored
        /// </summary>
        public bool Mirrored => (Data.Type == EventType.TYPE_PUNAISE3 && Data.HitPoints == 1) || Data.CommandCollection?.Commands?.FirstOrDefault()?.Command == EventCommand.GO_RIGHT;

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
        public Transform followSpriteLine;
        // Part parent
        //public Transform partParent;
        // Midpoint of this event when taking all the spriteparts into account
        [HideInInspector]
        public Vector2 midpoint;

        public AudioClip currentSoundEffect;

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

                    // Increment frame if animating
                    if (Settings.AnimateSprites && AnimSpeed > 0)
                        currentFrame += (60f / AnimSpeed) * Time.deltaTime;

                    // Loop back to first frame
                    if (currentFrame >= CurrentAnimation.Frames.GetLength(0))
                    {
                        currentFrame = 0;

                        // Get the new state
                        var newStateIndex = StateIndex + 1;

                        // Make sure it's within the collection
                        if (newStateIndex >= States.Length)
                            newStateIndex = 0;

                        // Update the state
                        UpdateCurrentState(newStateIndex);
                    }

                    int floored = Mathf.FloorToInt(currentFrame);

                    // Update child renderers with correct part and position, but only if current frame has updated
                    if (floored != prevFrame) {
                        UpdateParts(floored);
                        UpdateFollowSpriteLine();
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
            transform.position = new Vector3(Data.XPosition / 16f, -(Data.YPosition / 16f), 0);
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
            ChangeAnimation(CurrentState?.AnimationIndex ?? 0);

            // TODO: Is there a flag for these events to determine if they should do this?
            // Hard-code frames for special events
            if (Data.Type == EventType.TYPE_PUNAISE4 ||
                Data.Type == EventType.TYPE_FALLING_CRAYON)
            {
                currentFrame = Data.HitPoints;
                AnimSpeed = 0;
            }
            else
            {
                currentFrame = 0;
            }

            UpdateParts((int)currentFrame);

            // Collider
            ChangeColliderSize();

            //Offset points
            UpdateOffsetPoints();
            //FollowSprite line
            UpdateFollowSpriteLine();
        }

        public void UpdateOffsetPoints() {
            if (CurrentAnimation != null) {
                offsetCrossBX.localPosition = new Vector2(Data.OffsetBX / 16f, 0f);
                offsetCrossBY.localPosition = new Vector2(Data.OffsetBX / 16f, -(Data.OffsetBY / 16f));
                offsetCrossHY.localPosition = new Vector2(Data.OffsetBX / 16f, -((Data.OffsetHY / 16f) + (CurrentAnimation.DefaultFrameYPosition / 16f)));
            }
        }

        public void UpdateFollowSpriteLine() {
            if (CurrentAnimation != null && Data.FollowSprite < CurrentAnimation.Frames.GetLength(1)) {
                followSpriteLine.localPosition = new Vector2(CurrentAnimation.Frames[(int)currentFrame, Data.FollowSprite].X/16f, -CurrentAnimation.Frames[(int)currentFrame, Data.FollowSprite].Y/16f - (Data.OffsetHY / 16f));

                var w = (prefabRendereds[Data.FollowSprite].sprite == null) ? 0 : prefabRendereds[Data.FollowSprite].sprite.texture.width;
                followSpriteLine.localScale = new Vector2(w, 1f);
            }
        }

        public void ChangeOffsetVisibility(bool visible) {
            offsetOrigin.gameObject.SetActive(visible);
            offsetCrossBX.gameObject.SetActive(visible);
            offsetCrossBY.gameObject.SetActive(visible);
            offsetCrossHY.gameObject.SetActive(visible);
            followSpriteLine.gameObject.SetActive(visible);
            followSpriteLine.gameObject.SetActive(visible && Data.FollowEnabled);
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
            if (Data.DES < 0) {
                Debug.LogWarning($"DES index is below 0");
                return;
            }

            // Get the current animation
            CurrentAnimation = EditorManager?.GetCommonDesign(Data, Data.DES)?.Animations.ElementAtOrDefault(newAnim);

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
            var sprites = EditorManager.GetCommonDesign(Data, Data.DES).Sprites;

            for (int i = 0; i < CurrentAnimation.Frames.GetLength(1); i++) {
                // Skips sprites out of bounds
                if (CurrentAnimation.Frames[frame, i].SpriteIndex >= sprites.Count) {
                    prefabRendereds[i].sprite = null;
                }
                else {
                    prefabRendereds[i].sprite = sprites[CurrentAnimation.Frames[frame, i].SpriteIndex];
                }

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
            Controller.obj.levelController.Events.Remove(this);
            // Remove the data
            Controller.obj.levelController.currentLevel.EventData.Remove(Data);
            // Remove all children
            ClearChildren();
            // Remove self
            Destroy(gameObject);
        }
    }
}