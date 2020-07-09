using System;
using System.Linq;
using UnityEngine;

namespace R1Engine {
    /// <summary>
    /// Common event data
    /// </summary>
    public class Common_Event : MonoBehaviour 
    {
        #region Public Properties

        public string DisplayName { get; set; }
        public Editor_EventData Data { get; set; }

        public BaseEditorManager EditorManager => Controller.obj.levelController.EditorManager;
        public Common_EventState State => EditorManager.ETA.TryGetItem(Data.ETAKey)?.ElementAtOrDefault(Data.Data.RuntimeEtat)?.ElementAtOrDefault(Data.Data.RuntimeSubEtat);
        public Common_Animation CurrentAnimation => EditorManager?.DES.TryGetItem(Data.DESKey)?.Animations?.ElementAtOrDefault(Data.Data.RuntimeCurrentAnimIndex);
        public int AnimSpeed => Data.Type is EventType et && (et == EventType.TYPE_PUNAISE4 || et == EventType.TYPE_FALLING_CRAYON) ? 0 : State?.AnimationSpeed ?? 0;

        public byte? PrevAnimIndex { get; set; }
        public float EditorAnimFrame { get; set; }
        public float UpdateTimer { get; set; }
        public int UniqueLayer { get; set; }
        public int LinkID { get; set; }

        public bool DisplayOffsets { get; set; }

        #endregion

        #region Event Methods

        // TODO: Call this when adding a new event
        /// <summary>
        /// Performs the initial setup for the event
        /// </summary>
        public void InitialSetup()
        {
            if (Data.MapLayer != null && Data.MapLayer.Value > 0)
                Scale = Controller.obj.levelController.EditorManager.Level.Maps[Data.MapLayer.Value - 1].ScaleFactor;

            Data.Data.RuntimeEtat = Data.Data.Etat;
            Data.Data.RuntimeSubEtat = Data.Data.SubEtat;
            Data.Data.RuntimeLayer = Data.Data.Layer;
            Data.Data.RuntimeXPosition = (ushort)Data.Data.XPosition;
            Data.Data.RuntimeYPosition = (ushort)Data.Data.YPosition;
            Data.Data.RuntimeCurrentAnimFrame = 0;
            Data.Data.RuntimeCurrentAnimIndex = 0;
            Data.Data.RuntimeHitPoints = Data.Data.HitPoints;

            RefreshEditorInfo();
        }

        /// <summary>
        /// Refreshes the editor event info
        /// </summary>
        public void RefreshEditorInfo() {
            RefreshName();
            ChangeLinksVisibility(false);
        }
        
        public void RefreshName() => DisplayName = name = EditorManager.GetDisplayName(Data) ?? $"Unknown type {Data.Type}";

        #endregion

        public float Scale = 1f;

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

        public void ForceUpdate() {
            Update();
        }

        void Update()
        {
            // Make sure the events have loaded
            if (!Controller.obj.levelEventController.hasLoaded)
                return;

            // Update frame and states
            if (CurrentAnimation != null && !Settings.LoadFromMemory) 
            {
                // Set frame based on hit points for special events
                if (Data.Type is EventType et && (et == EventType.TYPE_PUNAISE4 || et == EventType.TYPE_FALLING_CRAYON || et == EventType.TYPE_EDU_LETTRE))
                {
                    Data.Data.RuntimeCurrentAnimFrame = Data.Data.HitPoints;
                    EditorAnimFrame = Data.Data.HitPoints;
                }
                else
                {
                    // Increment frame if animating
                    if (Settings.AnimateSprites && AnimSpeed > 0)
                        EditorAnimFrame += (60f / AnimSpeed) * Time.deltaTime;

                    // Update the frame
                    Data.Data.RuntimeCurrentAnimFrame = (byte)Mathf.FloorToInt(EditorAnimFrame);

                    // Loop back to first frame
                    if (Data.Data.RuntimeCurrentAnimFrame >= CurrentAnimation.Frames.Length)
                    {
                        Data.Data.RuntimeCurrentAnimFrame = 0;
                        EditorAnimFrame = 0;

                        if (Settings.StateSwitchingMode != StateSwitchingMode.None)
                        {
                            // Get the current state
                            var state = State;

                            // Check if we've reached the end of the linking chain and we're looping
                            if (Settings.StateSwitchingMode == StateSwitchingMode.Loop && Data.Data.RuntimeEtat == state.LinkedEtat && Data.Data.RuntimeSubEtat == state.LinkedSubEtat)
                            {
                                // Reset the state
                                Data.Data.RuntimeEtat = Data.Data.Etat;
                                Data.Data.RuntimeSubEtat = Data.Data.SubEtat;
                            }
                            else
                            {
                                // Update state values to the linked one
                                Data.Data.RuntimeEtat = state.LinkedEtat;
                                Data.Data.RuntimeSubEtat = state.LinkedSubEtat;
                            }
                        }
                    }
                }
            }

            UpdateTimer += Time.deltaTime;

            // Only update 60 frames per second, as that's the framerate for the game
            if (!(UpdateTimer > 1.0f / 60.0f))
                return;

            UpdateTimer = 0.0f;

            // Update the animation index if not loading from memory
            if (!Settings.LoadFromMemory)
            {
                Data.Data.RuntimeCurrentAnimIndex = State?.AnimationIndex ?? 0;

                // Hack for multi-colored events
                if (Data.Type is EventType et && PC_RD_Manager.MultiColoredEvents.Contains(et))
                    Data.Data.RuntimeCurrentAnimIndex = (byte)(Data.Data.RuntimeCurrentAnimIndex + ((EditorManager.DES[Data.DESKey].Animations.Count / 6) * Data.Data.HitPoints));
            }

            // Check if the animation has changed
            if (PrevAnimIndex != Data.Data.RuntimeCurrentAnimIndex)
            {
                // Update the animation index
                PrevAnimIndex = Data.Data.RuntimeCurrentAnimIndex;

                // If animation is null, use default renderer ("E")
                if (CurrentAnimation == null)
                {
                    defautRenderer.enabled = true;
                    ClearChildren();
                }
                else
                {
                    defautRenderer.enabled = false;

                    // Reset the current frame
                    if (!Settings.LoadFromMemory)
                    {
                        Data.Data.RuntimeCurrentAnimFrame = 0;
                        EditorAnimFrame = 0;
                    }

                    // Get the amount of layers per frame
                    var len = CurrentAnimation.Frames[Data.Data.RuntimeCurrentAnimFrame].Layers.Length;

                    // Clear old array
                    ClearChildren();

                    // Create array
                    prefabRendereds = new SpriteRenderer[len];

                    // Populate it with empty ones
                    for (int i = 0; i < len; i++)
                    {
                        // Instantiate prefab
                        SpriteRenderer newRenderer = Instantiate<GameObject>(prefabSpritepart, new Vector3(0, 0, len - i), Quaternion.identity, transform).GetComponent<SpriteRenderer>();
                        newRenderer.sortingOrder = Settings.LoadFromMemory ? -(Data.Data.EventIndex + (256 * Data.Data.RuntimeLayer)) : UniqueLayer;

                        // Set as child of events gameobject
                        newRenderer.gameObject.transform.parent = transform;
                        newRenderer.gameObject.transform.localScale = Vector3.one * Scale;
                        // Add to list
                        prefabRendereds[i] = newRenderer;
                    }
                }
            }

            // Get the current animation
            var anim = CurrentAnimation;

            // Update x and y
            transform.position = new Vector3(Data.Data.XPosition / 16f, -(Data.Data.YPosition / 16f), 0);

            // Don't move link cube if it's part of a link
            if (LinkID != 0)
                linkCube.position = linkCubeLockPosition;
            else
                linkCubeLockPosition = linkCube.position;

            // Update sprite parts in the animation
            if (anim != null)
            {
                var frame = Data.Data.RuntimeCurrentAnimFrame;

                // Get the sprites
                var sprites = EditorManager.DES[Data.DESKey].Sprites;

                var pivot = new Vector2(Data.Data.OffsetBX, -(Data.Data.OffsetBY));

                var mirrored = Data.GetIsFlippedHorizontally();

                for (int i = 0; i < anim.Frames[frame].Layers.Length; i++)
                {
                    // Set the sprite, skipping sprites which are out of bounds
                    prefabRendereds[i].sprite = anim.Frames[frame].Layers[i].ImageIndex >= sprites.Count ? null : sprites[anim.Frames[frame].Layers[i].ImageIndex];

                    var isFlippedHor = anim.Frames[frame].Layers[i].IsFlippedHorizontally;

                    // Indicate if the sprites should be flipped
                    prefabRendereds[i].flipX = (isFlippedHor ^ mirrored);
                    prefabRendereds[i].flipY = anim.Frames[frame].Layers[i].IsFlippedVertically;

                    // Get the dimensions
                    var w = prefabRendereds[i].sprite == null ? 0 : prefabRendereds[i].sprite.texture.width;
                    var h = prefabRendereds[i].sprite == null ? 0 : prefabRendereds[i].sprite.texture.height;
                    
                    var xx = anim.Frames[frame].Layers[i].XPosition + (isFlippedHor ? w : 0);

                    var yy = -(anim.Frames[frame].Layers[i].YPosition + (anim.Frames[frame].Layers[i].IsFlippedVertically ? h : 0));

                    // scale
                    Vector2 pos = new Vector2(
                        ((xx - pivot.x) * (mirrored ? -1f : 1f) * Scale + pivot.x) / 16f,
                        ((yy - pivot.y) * Scale + pivot.y) / 16f);

                    prefabRendereds[i].transform.localPosition = new Vector3(pos.x, pos.y, prefabRendereds[i].transform.localPosition.z);
                    prefabRendereds[i].transform.localScale = Vector3.one * Scale;

                    // Get visiblity
                    prefabRendereds[i].enabled = Data.GetIsVisible();
                    prefabRendereds[i].color = Data.GetIsFaded() ? new Color(1, 1, 1, 0.5f) : Color.white;
                }
            }

            // Update the follow sprite line
            if (anim != null && Data.Data.FollowSprite < anim.Frames[Data.Data.RuntimeCurrentAnimFrame].Layers.Length)
            {
                followSpriteLine.localPosition = new Vector2(anim.Frames[Data.Data.RuntimeCurrentAnimFrame].Layers[Data.Data.FollowSprite].XPosition / 16f, -anim.Frames[Data.Data.RuntimeCurrentAnimFrame].Layers[Data.Data.FollowSprite].YPosition / 16f - (Data.Data.OffsetHY / 16f));

                var w = (prefabRendereds[Data.Data.FollowSprite].sprite == null) ? 0 : prefabRendereds[Data.Data.FollowSprite].sprite.texture.width;
                followSpriteLine.localScale = new Vector2(w, 1f);
            }

            // Update the collider size for when selecting the events
            if (anim != null)
            {
                // Set box collider size to be the combination of all parts
                float leftX = 0, topY = 0, rightX = 0, bottomY = 0;
                bool first = true;
                foreach (SpriteRenderer part in prefabRendereds)
                {
                    var pos = new Vector2(Mathf.Abs(part.transform.localPosition.x) * 16, Mathf.Abs(part.transform.localPosition.y) * 16);

                    if (part.sprite == null)
                        continue;

                    if (pos.x - (part.flipX ? part.sprite.texture.width : 0) < leftX || first)
                        leftX = pos.x - (part.flipX ? part.sprite.texture.width : 0);
                    if (pos.x + part.sprite.texture.width - (part.flipX ? part.sprite.texture.width : 0) > rightX || first)
                        rightX = pos.x + part.sprite.texture.width - (part.flipX ? part.sprite.texture.width : 0);
                    if (pos.y < topY || first)
                        topY = pos.y;
                    if (pos.y + part.sprite.texture.height > bottomY || first)
                        bottomY = pos.y + part.sprite.texture.height;

                    if (first)
                        first = false;
                }

                if (!first)
                {
                    var w = (rightX - leftX) / 16f;
                    var h = (bottomY - topY) / 16f;
                    boxCollider.size = new Vector2(w, h);
                    boxCollider.offset = new Vector2(leftX / 16f + w / 2f, -(topY / 16f + h / 2f));
                }
            }

            // Update offset points
            if (anim != null)
            {
                offsetCrossBX.localPosition = new Vector2(Data.Data.OffsetBX / 16f, 0f);
                offsetCrossBY.localPosition = new Vector2(Data.Data.OffsetBX / 16f, -(Data.Data.OffsetBY / 16f));
                offsetCrossHY.localPosition = new Vector2(Data.Data.OffsetBX / 16f, -((Data.Data.OffsetHY / 16f) + (CurrentAnimation.Frames[0].FrameData.YPosition / 16f)));
            }

            // Update visibility
            boxCollider.enabled = Data.GetIsVisible();

            // Set new midpoint
            midpoint = new Vector3(transform.position.x + boxCollider.offset.x, transform.position.y + boxCollider.offset.y, 0);

            // Set link line to cube
            lineRend.SetPosition(0, midpoint);
            lineRend.SetPosition(1, linkCube.position);

            // Change the offsets visibility
            offsetOrigin.gameObject.SetActive(DisplayOffsets);
            offsetCrossBX.gameObject.SetActive(DisplayOffsets);
            offsetCrossBY.gameObject.SetActive(DisplayOffsets);
            offsetCrossHY.gameObject.SetActive(DisplayOffsets);
            followSpriteLine.gameObject.SetActive(DisplayOffsets);
            followSpriteLine.gameObject.SetActive(DisplayOffsets && Data.Data.GetFollowEnabled(EditorManager.Settings));
        }

        public void ChangeLinksVisibility(bool visible) {
            if (visible && Data.GetIsVisible()) {

                //Change link colors
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

        private void ClearChildren() {
            // Clear old array
            if (prefabRendereds != null) {
                foreach (SpriteRenderer t in prefabRendereds) {
                    GameObject g = t.gameObject;
                    Destroy(t);
                    Destroy(g);
                }

                Array.Clear(prefabRendereds, 0, prefabRendereds.Length);
                prefabRendereds = null;
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