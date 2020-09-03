using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Common event data
    /// </summary>
    public class Unity_ObjBehaviour : MonoBehaviour 
    {
        #region Public Properties

        public Unity_Object ObjData { get; set; }
        public float UpdateTimer { get; set; }
        public bool DisplayOffsets { get; set; }

        public int Index { get; set; }

        public bool IsVisible => ObjData.IsVisible && (ObjData.MapLayer == null || (LevelEditorData.ShowEventsForMaps?.ElementAtOrDefault(ObjData.MapLayer.Value) ?? false));
        public int Layer => (ObjData.GetLayer(Index) ?? Index) * 256;

        #endregion

        #region Event Methods

        public void RefreshEditorInfo() => ChangeLinksVisibility(false);

        #endregion

        // Default sprite
        public SpriteRenderer defautRenderer;
        // Reference to spritepart prefab
        public GameObject prefabSpritepart;
        // Reference to the created renderers
        public SpriteRenderer[] prefabRenderers;
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

        private void Start() 
        {
            Index = LevelEditorData.Level.EventData.IndexOf(ObjData);
            transform.rotation = Quaternion.identity;

            RefreshEditorInfo();

            // Snap link cube position
            linkCube.position = new Vector2(Mathf.FloorToInt(linkCube.position.x), Mathf.FloorToInt(linkCube.position.y));
        }

        public void ForceUpdate() => Update();

        void Update()
        {
            // Make sure the events have loaded
            if (!Controller.obj.levelEventController.hasLoaded)
                return;

            // Update frame and states
            if (ObjData.CurrentAnimation != null && !Settings.LoadFromMemory)
                ObjData.UpdateFrame();

            UpdateTimer += Time.deltaTime;

            // Only update 60 frames per second, as that's the framerate for the game
            if (!(UpdateTimer > 1.0f / 60.0f))
                return;

            UpdateTimer = 0.0f;

            defautRenderer.enabled = true;

            if (ObjData.ShouldUpdateAnimation())
            {
                // If animation is null, use default renderer ("E")
                if (ObjData.CurrentAnimation == null)
                {
                    ClearChildren();
                }
                else
                {
                    // Reset the current frame
                    ObjData.ResetFrame();

                    // Get the amount of layers per frame
                    var len = ObjData.CurrentAnimation.Frames.Max(f => f.Layers.Length);

                    // Clear old array
                    ClearChildren();

                    // Create array
                    prefabRenderers = new SpriteRenderer[len];

                    // Populate it with empty ones
                    for (int i = 0; i < len; i++)
                    {
                        // Instantiate prefab
                        SpriteRenderer newRenderer = Instantiate(prefabSpritepart, transform).GetComponent<SpriteRenderer>();
                        newRenderer.sortingOrder = Layer + i;

                        newRenderer.transform.localPosition = new Vector3(0, 0, len - i);
                        newRenderer.transform.localRotation = Quaternion.identity;
                        newRenderer.transform.localScale = Vector3.one * ObjData.Scale;

                        // Add to list
                        prefabRenderers[i] = newRenderer;
                    }
                }
            }

            // Get the current animation
            var anim = ObjData.CurrentAnimation;

            defautRenderer.enabled = Settings.ShowDefaultObjIcons && anim == null;

            // Update x and y, and clamp them to not have objects appear too far away from the map
            const int allowedBorder = 200;
            const int border = 10;

            var maxWidth = LevelEditorData.MaxWidth;
            var maxHeight = LevelEditorData.MaxHeight;
            var x = (float)ObjData.XPosition;
            var y = (float)ObjData.YPosition;

            if (x > (maxWidth * LevelEditorData.Level.CellSize) + allowedBorder || x < -allowedBorder)
                x = (maxWidth * LevelEditorData.Level.CellSize) + border;

            if (y > (maxHeight * LevelEditorData.Level.CellSize) + allowedBorder || y < -allowedBorder)
                y = ((maxHeight * LevelEditorData.Level.CellSize) + border);

            transform.position = new Vector3(x / LevelEditorData.Level.PixelsPerUnit, -(y / LevelEditorData.Level.PixelsPerUnit), 0);

            // Don't move link cube if it's part of a link
            if (ObjData.EditorLinkGroup != 0)
                linkCube.position = linkCubeLockPosition;
            else
                linkCubeLockPosition = linkCube.position;

            // Update sprite parts in the animation
            if (anim != null)
            {
                // Get properties
                var frame = ObjData.CurrentAnimationFrame;
                var sprites = ObjData.Sprites;
                var pivot = ObjData.Pivot;
                var mirroredX = ObjData.FlipHorizontally;
                var mirroredY = ObjData.FlipVertically;

                for (int i = 0; i < anim.Frames[frame].Layers.Length; i++)
                {
                    var layer = anim.Frames[frame].Layers[i];
                    // Get the sprite index
                    var spriteIndex = layer.ImageIndex;

                    // Change it if the event is multi-colored (Rayman 1 only)
                    if (ObjData is Unity_Object_R1 objr1 && objr1.EventData.Type.IsMultiColored())
                        spriteIndex += ((sprites.Count / 6) * objr1.EventData.HitPoints);

                    if (prefabRenderers.Length <= i)
                        continue;

                    // Set the sprite, skipping sprites which are out of bounds
                    if (spriteIndex >= sprites.Count && (LevelEditorData.CurrentSettings.EngineVersion != EngineVersion.R2_PS1 || spriteIndex < 0xFFF)) {
                        print("Sprite index too high: " + ObjData.DisplayName + ": " + spriteIndex + " >= " + sprites.Count);
                    }
                    prefabRenderers[i].sprite = spriteIndex >= sprites.Count ? null : sprites[spriteIndex];

                    var layerMirroredX = layer.IsFlippedHorizontally;
                    var layerMirroredY = layer.IsFlippedVertically;

                    // Indicate if the sprites should be flipped
                    prefabRenderers[i].flipX = (layerMirroredX ^ mirroredX);
                    prefabRenderers[i].flipY = (layerMirroredY ^ mirroredY);

                    // Get the dimensions
                    var w = prefabRenderers[i].sprite == null ? 0 : prefabRenderers[i].sprite.texture.width;
                    var h = prefabRenderers[i].sprite == null ? 0 : prefabRenderers[i].sprite.texture.height;
                    
                    var xx = layer.XPosition + (layerMirroredX ? w : 0);

                    var yy = -(layer.YPosition + (layerMirroredY ? h : 0));

                    // scale
                    Vector2 pos = new Vector2(
                        ((xx - pivot.x) * (mirroredX ? -1f : 1f) * ObjData.Scale + pivot.x) / (float)LevelEditorData.Level.PixelsPerUnit,
                        ((yy - pivot.y) * (mirroredY ? -1f : 1f) * ObjData.Scale + pivot.y) / (float)LevelEditorData.Level.PixelsPerUnit);

                    prefabRenderers[i].transform.localPosition = new Vector3(pos.x, pos.y, prefabRenderers[i].transform.localPosition.z);
                    prefabRenderers[i].transform.localScale = Vector3.one * ObjData.Scale;

                    prefabRenderers[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                    if ((layer.Rotation.HasValue && layer.Rotation.Value != 0) || (layer.Scale.HasValue && layer.Scale.Value != Vector2.one)) {

                        Vector3 transformOrigin = new Vector3(
                            (((layer.TransformOriginX - pivot.x) * (mirroredX ? -1f : 1f) * ObjData.Scale + pivot.x) / (float)LevelEditorData.Level.PixelsPerUnit),
                            ((-layer.TransformOriginY - pivot.y) * (mirroredY ? -1f : 1f) * ObjData.Scale + pivot.y) / (float)LevelEditorData.Level.PixelsPerUnit,
                            prefabRenderers[i].transform.localPosition.z);

                        // Scale first
                        if (layer.Scale.HasValue && layer.Scale.Value != Vector2.one) {
                            Vector3 scaleValue = new Vector3(layer.Scale.Value.x, layer.Scale.Value.y, 1f);
                            prefabRenderers[i].transform.localScale = Vector3.Scale(Vector3.one * ObjData.Scale, scaleValue);
                            Vector3 scaledPos = Vector3.Scale(prefabRenderers[i].transform.localPosition - transformOrigin, scaleValue);
                            prefabRenderers[i].transform.localPosition = transformOrigin + scaledPos;
                        }
                        // Then rotate
                        if (layer.Rotation.HasValue && layer.Rotation.Value != 0) {
                            /*Quaternion rotation = Quaternion.Euler(0, 0, layer.Rotation * 180f);*/
                            //Vector3 rotationOrigin = Vector3.zero;

                            prefabRenderers[i].transform.RotateAround(transform.TransformPoint(transformOrigin), new Vector3(0, 0, 1), layer.Rotation.Value * ((mirroredX ^ mirroredY) ? -1f : 1f));
                            /*    Vector2 relativePos = pos - rotationOrigin;
                            Vector2 rotatedPos = rotation * relativePos;
                            prefabRenderers[i].transform.localRotation = rotation;
                            prefabRenderers[i].transform.localPosition = new Vector3(relativePos.x + rotatedPos.x, relativePos.y + rotatedPos.y, prefabRenderers[i].transform.localPosition.z);*/
                        }
                    }

                    // Get visibility
                    prefabRenderers[i].enabled = IsVisible;
                    prefabRenderers[i].color = ObjData.IsDisabled ? new Color(1, 1, 1, 0.5f) : Color.white;
                }
                for(int i = anim.Frames[frame].Layers.Length; i < prefabRenderers.Length; i++) {
                    prefabRenderers[i].sprite = null;
                    prefabRenderers[i].enabled = false;
                }
            }

            // Update the follow sprite line (Rayman 1 only)
            if (ObjData is Unity_Object_R1 r1 && anim != null && r1.EventData.FollowSprite < anim.Frames[ObjData.CurrentAnimationFrame].Layers.Length)
            {
                followSpriteLine.localPosition = new Vector2(anim.Frames[r1.EventData.RuntimeCurrentAnimFrame].Layers[r1.EventData.FollowSprite].XPosition / (float)LevelEditorData.Level.PixelsPerUnit, -anim.Frames[r1.EventData.RuntimeCurrentAnimFrame].Layers[r1.EventData.FollowSprite].YPosition / (float)LevelEditorData.Level.PixelsPerUnit - (r1.EventData.OffsetHY / (float)LevelEditorData.Level.PixelsPerUnit));

                var w = (prefabRenderers[r1.EventData.FollowSprite].sprite == null) ? 0 : prefabRenderers[r1.EventData.FollowSprite].sprite.texture.width;
                followSpriteLine.localScale = new Vector2(w, 1f);
            }

            // Update the collider size for when selecting the events
            if (anim != null)
            {
                // Set box collider size to be the combination of all parts
                float leftX = 0, bottomY = 0, rightX = 0, topY = 0;
                bool first = true;
                foreach (SpriteRenderer part in prefabRenderers)
                {
                    if (part.sprite == null)
                        continue;

                    Bounds b = part.bounds;
                    b = new Bounds(transform.InverseTransformPoint(b.center) * LevelEditorData.Level.PixelsPerUnit, transform.InverseTransformVector(b.size) * LevelEditorData.Level.PixelsPerUnit);

                    if (b.min.x < leftX || first) leftX = b.min.x;
                    if (b.min.y < bottomY || first) bottomY = b.min.y;
                    if (b.max.x > rightX || first) rightX = b.max.x;
                    if (b.max.y > topY || first) topY = b.max.y;

                    if (first)
                        first = false;
                }

                if (!first)
                {
                    var w = (rightX - leftX) / LevelEditorData.Level.PixelsPerUnit;
                    var h = (topY - bottomY) / LevelEditorData.Level.PixelsPerUnit;
                    boxCollider.size = new Vector2(w, h);
                    boxCollider.offset = new Vector2(leftX / LevelEditorData.Level.PixelsPerUnit + w / 2f, (topY / LevelEditorData.Level.PixelsPerUnit - h / 2f));
                }
            }

            // Update offset points
            if (anim != null)
            {
                var pivot = ObjData.Pivot;

                offsetCrossBX.localPosition = new Vector2(pivot.x / LevelEditorData.Level.PixelsPerUnit, 0f);
                offsetCrossBY.localPosition = new Vector2(pivot.x / LevelEditorData.Level.PixelsPerUnit, -(pivot.y / LevelEditorData.Level.PixelsPerUnit));
                offsetCrossHY.localPosition = new Vector2(pivot.x / LevelEditorData.Level.PixelsPerUnit, -((pivot.y / LevelEditorData.Level.PixelsPerUnit) + ((ObjData.CurrentAnimation?.Frames?.ElementAtOrDefault(0)?.FrameData?.YPosition ?? 1) / (float)LevelEditorData.Level.PixelsPerUnit)));
            }

            // Update visibility
            boxCollider.enabled = IsVisible;

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
            followSpriteLine.gameObject.SetActive(DisplayOffsets && ObjData is Unity_Object_R1 r1o && r1o.EventData.GetFollowEnabled(LevelEditorData.CurrentSettings));
        }

        public void ChangeLinksVisibility(bool visible) {
            if (visible && IsVisible) {

                //Change link colors
                if (ObjData.EditorLinkGroup == 0) {
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
            if (prefabRenderers == null) 
                return;

            foreach (SpriteRenderer t in prefabRenderers) {
                GameObject g = t.gameObject;
                Destroy(t);
                Destroy(g);
            }

            Array.Clear(prefabRenderers, 0, prefabRenderers.Length);
            prefabRenderers = null;
        }

        // Delete this event properly
        public void Delete() {
            // Remove this from the event list
            Controller.obj.levelController.Events.Remove(this);
            // Remove the data
            LevelEditorData.Level.EventData.Remove(ObjData);
            // Remove all children
            ClearChildren();
            // Remove self
            Destroy(gameObject);
        }
    }
}