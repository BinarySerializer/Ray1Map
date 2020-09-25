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
        public bool IsSelected { get; set; }
        public bool ShowOffsets => (IsSelected || Settings.ShowObjOffsets) && EnableBoxCollider;
        public bool ShowCollision => (IsSelected || Settings.ShowObjCollision) && IsVisible;
        public bool ShowDefaultRenderer => Settings.ShowDefaultObjIcons && ObjData.CurrentAnimation == null && IsVisible && !HasObjCollision;
        public bool EnableBoxCollider => IsVisible && (ObjData.CurrentAnimation != null || ShowDefaultRenderer || (HasObjCollision && ShowCollision));
        public bool HasObjCollision => ObjData.ObjCollision?.Any() == true;

        public int Index { get; set; }

        public bool IsEnabled { get; set; } = true;
        public bool IsVisible => 
            // Obj is enabled (web only)
            IsEnabled && 
            // Obj is in current sector
            (!LevelEditorData.ShowOnlyActiveSector || LevelEditorData.Level.Sectors[LevelEditorData.ActiveSector].Objects.Contains(Index)) && 
            // Obj is visible
            ObjData.IsVisible && 
            // Obj is on current map layer
            (ObjData.MapLayer == null || (LevelEditorData.ShowEventsForMaps?.ElementAtOrDefault(ObjData.MapLayer.Value) ?? false));
        public int Layer => (ObjData.GetLayer(Index) ?? Index) * 256;

        #endregion

        public LineRenderer[] gbaLinkLines;
        // Default sprite
        public SpriteRenderer defautRenderer;
        // Reference to spritepart prefab
        public GameObject prefabSpritepart;
        // Reference to the box prefab
        public GameObject prefabBox;
        // Reference to the created renderers
        public SpriteRenderer[] prefabRenderers;
        public SpriteRenderer[] prefabRenderersCollision;
        public SpriteRenderer[] prefabRendersObjCollision;
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
        public Transform offsetCrossHY;
        public Transform followSpriteLine;
        // Part parent
        //public Transform partParent;
        // Midpoint of this event when taking all the spriteparts into account
        [HideInInspector]
        public Vector2 midpoint;

        public AudioClip currentSoundEffect;

        public void Init() {
            // Update x and y, and clamp them to not have objects appear too far away from the map
            const int border = 250;

            var maxWidth = LevelEditorData.MaxWidth;
            var maxHeight = LevelEditorData.MaxHeight;
            var x = (float)ObjData.XPosition;
            var y = (float)ObjData.YPosition;
            var maxX = (maxWidth * LevelEditorData.Level.CellSize) + border + ObjData.Pivot.x;
            var minX = -(border) - ObjData.Pivot.x;
            var maxY = (maxHeight * LevelEditorData.Level.CellSize) + border + ObjData.Pivot.y;
            var minY = -(border) - ObjData.Pivot.y;

            if (x > maxX)
                x = maxX;

            if (x < minX)
                x = minX;

            if (y > maxY)
                y = maxY;

            if (y < minY)
                y = minY;

            transform.position = new Vector3(x / LevelEditorData.Level.PixelsPerUnit, -(y / LevelEditorData.Level.PixelsPerUnit), 0);

        }

        private void Start() 
        {
            transform.rotation = Quaternion.identity;

            // Snap link cube position
            linkCube.position = new Vector2(Mathf.FloorToInt(linkCube.position.x), Mathf.FloorToInt(linkCube.position.y));
        }

        public void ForceUpdate() => Update();

        private void SetCollisionBox(Unity_ObjAnimationCollisionPart collision, SpriteRenderer collisionSpriteRenderer, Vector2 pivot)
        {
            var mirroredX = ObjData.FlipHorizontally;
            var mirroredY = ObjData.FlipVertically;

            Vector2 pos = new Vector2(
                ((collision.XPosition - pivot.x) * (mirroredX ? -1f : 1f) * ObjData.Scale + pivot.x) / (float)LevelEditorData.Level.PixelsPerUnit,
                (((-collision.YPosition - pivot.y) * (mirroredY ? -1f : 1f) * ObjData.Scale + pivot.y) / (float)LevelEditorData.Level.PixelsPerUnit));

            collisionSpriteRenderer.transform.localPosition = new Vector3(pos.x, pos.y, collisionSpriteRenderer.transform.localPosition.z);
            collisionSpriteRenderer.transform.localScale = new Vector3(
                (collision.Width / (float)LevelEditorData.Level.PixelsPerUnit) * (mirroredX ? -1f : 1f),
                (collision.Height / (float)LevelEditorData.Level.PixelsPerUnit) * (mirroredY ? -1f : 1f)) * ObjData.Scale;

            // Set color depending on the collision type
            switch (collision.Type)
            {
                case Unity_ObjAnimationCollisionPart.CollisionType.AttackBox:
                    collisionSpriteRenderer.color = new Color(1f, 0f, 0f, 0.4f);
                    break;

                case Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox:
                    collisionSpriteRenderer.color = new Color(0f, 1f, 0f, 0.4f);
                    break;

                case Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox:
                    collisionSpriteRenderer.color = new Color(1f, 0.7f, 0f, 0.4f);
                    break;

                case Unity_ObjAnimationCollisionPart.CollisionType.HitTriggerBox:
                    collisionSpriteRenderer.color = new Color(1f, 0f, 1f, 0.4f);
                    break;

                case Unity_ObjAnimationCollisionPart.CollisionType.Gendoor:
                    collisionSpriteRenderer.color = new Color(0f, 1f, 0.7f, 0.4f);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
                    ClearSprites(prefabRenderers);
                    ClearSprites(prefabRenderersCollision);
                }
                else
                {
                    // Reset the current frame
                    ObjData.ResetFrame();

                    // Get the amount of sprite layers per frame
                    var spritesLength = ObjData.CurrentAnimation.Frames.Max(f => f.SpriteLayers.Length);

                    // Get the amount of collision layers per frame
                    var collisionLength = ObjData.CurrentAnimation.Frames.Max(f => f.CollisionLayers?.Length ?? 0);

                    // Clear old arrays
                    ClearSprites(prefabRenderers);
                    ClearSprites(prefabRenderersCollision);

                    // Create arrays
                    prefabRenderers = new SpriteRenderer[spritesLength];
                    prefabRenderersCollision = new SpriteRenderer[collisionLength];

                    // Populate sprites
                    for (int i = 0; i < spritesLength; i++)
                    {
                        // Instantiate prefab
                        SpriteRenderer newRenderer = Instantiate(prefabSpritepart, transform).GetComponent<SpriteRenderer>();
                        newRenderer.sortingOrder = Layer + i;

                        newRenderer.transform.localPosition = new Vector3(0, 0, spritesLength - i);
                        newRenderer.transform.localRotation = Quaternion.identity;
                        newRenderer.transform.localScale = Vector3.one * ObjData.Scale;

                        // Add to list
                        prefabRenderers[i] = newRenderer;
                    }

                    // Populate collision boxes
                    for (int i = 0; i < collisionLength; i++)
                    {
                        // Instantiate prefab
                        var newRenderer = Instantiate(prefabBox, transform).GetComponent<SpriteRenderer>();
                        newRenderer.sortingOrder = Layer + spritesLength + i;

                        newRenderer.transform.localRotation = Quaternion.identity;

                        // Add to list
                        prefabRenderersCollision[i] = newRenderer;
                    }
                }
            }

            // Get the current animation
            var anim = ObjData.CurrentAnimation;

            defautRenderer.enabled = ShowDefaultRenderer;

            // Update x and y, and clamp them to not have objects appear too far away from the map
            const int border = 250;

            var maxWidth = LevelEditorData.MaxWidth;
            var maxHeight = LevelEditorData.MaxHeight;
            var x = (float)ObjData.XPosition;
            var y = (float)ObjData.YPosition;
            var maxX = (maxWidth * LevelEditorData.Level.CellSize) + border + ObjData.Pivot.x;
            var minX = -(border) - ObjData.Pivot.x;
            var maxY = (maxHeight * LevelEditorData.Level.CellSize) + border + ObjData.Pivot.y;
            var minY = -(border) - ObjData.Pivot.y;

            if (x > maxX)
                x = maxX;

            if (x < minX)
                x = minX;

            if (y > maxY)
                y = maxY;

            if (y < minY)
                y = minY;

            transform.position = new Vector3(x / LevelEditorData.Level.PixelsPerUnit, -(y / LevelEditorData.Level.PixelsPerUnit), 0);

            // Don't move link cube if it's part of a link
            if (ObjData.R1_EditorLinkGroup != 0)
                linkCube.position = linkCubeLockPosition;
            else
                linkCubeLockPosition = linkCube.position;

            // Update sprite parts in the animation
            if (anim != null)
            {
                // Get properties
                var frame = ObjData.AnimationFrame;
                var sprites = ObjData.Sprites;
                var pivot = ObjData.Pivot;
                var mirroredX = ObjData.FlipHorizontally;
                var mirroredY = ObjData.FlipVertically;

                // Update sprites
                for (int i = 0; i < anim.Frames[frame].SpriteLayers.Length; i++)
                {
                    var layer = anim.Frames[frame].SpriteLayers[i];

                    // Get the sprite index
                    var spriteIndex = layer.ImageIndex;

                    // Change it if the event is multi-colored (Rayman 1 only)
                    if (ObjData is Unity_Object_R1 objr1 && objr1.EventData.Type.IsMultiColored())
                        spriteIndex += ((sprites.Count / 6) * objr1.EventData.HitPoints);

                    if (prefabRenderers.Length <= i)
                        continue;

                    // Set the sprite, skipping sprites which are out of bounds
                    if (spriteIndex >= sprites.Count && (LevelEditorData.CurrentSettings.EngineVersion != EngineVersion.R2_PS1 || spriteIndex < 0xFFF)) {
                        print("Sprite index too high: " + ObjData.Name + ": " + spriteIndex + " >= " + sprites.Count);
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

                // Remove unused sprites
                for(int i = anim.Frames[frame].SpriteLayers.Length; i < prefabRenderers.Length; i++) {
                    prefabRenderers[i].sprite = null;
                    prefabRenderers[i].enabled = false;
                }

                // Update collision
                for (int i = 0; i < anim.Frames[frame].CollisionLayers.Length; i++)
                {
                    SetCollisionBox(anim.Frames[frame].CollisionLayers[i], prefabRenderersCollision[i], pivot);
                    prefabRenderersCollision[i].enabled = ShowCollision;
                }

                // Remove unused collision layers
                for (int i = anim.Frames[frame].CollisionLayers.Length; i < prefabRenderersCollision.Length; i++)
                {
                    prefabRenderersCollision[i].sprite = null;
                    prefabRenderersCollision[i].enabled = false;
                }
            }

            var col = ObjData.ObjCollision;

            // Update object collision
            if (prefabRendersObjCollision == null || col.Length != prefabRendersObjCollision.Length)
            {
                // Clear old sprites
                ClearSprites(prefabRendersObjCollision);

                prefabRendersObjCollision = new SpriteRenderer[col.Length];

                // Instantiate prefabs
                for (int i = 0; i < col.Length; i++)
                {
                    prefabRendersObjCollision[i] = Instantiate(prefabBox, transform).GetComponent<SpriteRenderer>();
                    prefabRendersObjCollision[i].sortingOrder = Layer;

                    prefabRendersObjCollision[i].transform.localPosition = new Vector3(0, 0, col.Length - i);
                    prefabRendersObjCollision[i].transform.localRotation = Quaternion.identity;
                    prefabRendersObjCollision[i].transform.localScale = Vector3.one * ObjData.Scale;
                }
            }

            if (col != null)
            {
                for (var i = 0; i < prefabRendersObjCollision.Length; i++)
                {
                    SetCollisionBox(col[i], prefabRendersObjCollision[i], ObjData.Pivot);
                    prefabRendersObjCollision[i].enabled = ShowCollision;
                }
            }

            // Update the follow sprite line (Rayman 1 only)
            if (ObjData is Unity_Object_R1 r1 && anim != null)
            {
                var animLayer = anim.Frames[r1.AnimationFrame].SpriteLayers.ElementAtOrDefault(r1.EventData.FollowSprite);
                var imgDescr = r1.ObjManager.DES.ElementAtOrDefault(r1.DESIndex)?.Data?.ImageDescriptors.ElementAtOrDefault(animLayer?.ImageIndex ?? -1);

                followSpriteLine.localPosition = new Vector2(
                    ((animLayer?.XPosition ?? 0) + (imgDescr?.HitBoxOffsetX ?? 0)) / (float)LevelEditorData.Level.PixelsPerUnit,
                    -((animLayer?.YPosition ?? 0) + (imgDescr?.HitBoxOffsetY ?? 0)) / (float)LevelEditorData.Level.PixelsPerUnit - (r1.EventData.OffsetHY / (float)LevelEditorData.Level.PixelsPerUnit));

                var w = (prefabRenderers.ElementAtOrDefault(r1.EventData.FollowSprite)?.sprite == null) ? 0 : imgDescr?.HitBoxWidth ?? 0;
                followSpriteLine.localScale = new Vector2(w, 1f);
            }

            // Update the collider size for when selecting the events
            if (anim != null || col?.Any() == true)
            {
                var sprites = anim != null ? prefabRenderers : prefabRendersObjCollision;

                // Set box collider size to be the combination of all parts
                float leftX = 0, bottomY = 0, rightX = 0, topY = 0;
                bool first = true;
                foreach (SpriteRenderer part in sprites)
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
            else
            {
                boxCollider.size = new Vector2(1, 1);
                boxCollider.offset = new Vector2();
            }

            // Update offset points
            if (anim != null)
            {
                var pivot = ObjData.Pivot;

                offsetCrossBX.localPosition = new Vector2(pivot.x / LevelEditorData.Level.PixelsPerUnit, (pivot.y / LevelEditorData.Level.PixelsPerUnit));

                if (ObjData is Unity_Object_R1 r1bj)
                {
                    int hy = -(r1bj.EventData.OffsetHY);

                    if (r1bj.EventData.GetFollowEnabled(LevelEditorData.CurrentSettings))
                        hy -= anim.Frames[r1bj.EventData.RuntimeCurrentAnimFrame].SpriteLayers.ElementAtOrDefault(r1bj.EventData.FollowSprite)?.YPosition ?? 0;

                    offsetCrossHY.localPosition = new Vector2(pivot.x / LevelEditorData.Level.PixelsPerUnit, hy  / (float)LevelEditorData.Level.PixelsPerUnit);
                }
                else if (ObjData is Unity_Object_R2 r2bj)
                {
                    var hy = -(r2bj.EventData.CollisionData.OffsetHY);

                    offsetCrossHY.localPosition = new Vector2(pivot.x / LevelEditorData.Level.PixelsPerUnit, hy / (float)LevelEditorData.Level.PixelsPerUnit);
                }
            }

            // Update visibility
            boxCollider.enabled = EnableBoxCollider;

            // Set new midpoint
            midpoint = new Vector3(transform.position.x + boxCollider.offset.x, transform.position.y + boxCollider.offset.y, 0);

            // Set link line to cube
            lineRend.SetPosition(0, midpoint);
            lineRend.SetPosition(1, linkCube.position);

            // Update link colors
            if (ObjData.R1_EditorLinkGroup == 0)
            {
                lineRend.startColor = Controller.obj.levelEventController.linkColorDeactive;
                lineRend.endColor = Controller.obj.levelEventController.linkColorDeactive;
                linkCube.GetComponent<SpriteRenderer>().color = Controller.obj.levelEventController.linkColorDeactive;
            }
            else
            {
                lineRend.startColor = Controller.obj.levelEventController.linkColorActive;
                lineRend.endColor = Controller.obj.levelEventController.linkColorActive;
                linkCube.GetComponent<SpriteRenderer>().color = Controller.obj.levelEventController.linkColorActive;
            }

            // Set link visibility
            var showLinks = 
                // Make sure the obj is visible
                IsVisible && 
                // Make sure links are set to show
                Settings.ShowLinks && 
                // Only show active links on web
                !(FileSystem.mode == FileSystem.Mode.Web && ObjData.R1_EditorLinkGroup == 0) && 
                // Don't show R1 links on GBA
                LevelEditorData.CurrentSettings.MajorEngineVersion != MajorEngineVersion.GBA;
            lineRend.enabled = showLinks;
            linkCube.gameObject.SetActive(showLinks);

            // Change the offsets visibility
            offsetOrigin.gameObject.SetActive(ShowOffsets);
            offsetCrossBX.gameObject.SetActive(ShowOffsets && offsetCrossBX.transform.position != Vector3.zero);
            offsetCrossHY.gameObject.SetActive(ShowOffsets && (ObjData is Unity_Object_R1 || ObjData is Unity_Object_R2) && offsetCrossHY.transform.position != Vector3.zero);

            var engineVersion = LevelEditorData.CurrentSettings.EngineVersion;
            followSpriteLine.gameObject.SetActive(
                ShowCollision && 
                ObjData is Unity_Object_R1 r1o && 
                r1o.EventData.GetFollowEnabled(LevelEditorData.CurrentSettings) && 
                !(engineVersion == EngineVersion.R1_PS1_JP || engineVersion == EngineVersion.R1_PS1_JPDemoVol3 || engineVersion == EngineVersion.R1_PS1_JPDemoVol6 || engineVersion == EngineVersion.R1_Saturn));

            // Update GBA link lines
            if (ObjData is Unity_Object_GBA gba)
            {
                var index = 0;
                var objects = Controller.obj.levelController.Objects;

                foreach (var linkedActor in gba.GetLinkedActors())
                {
                    var lr = gbaLinkLines[index];

                    Vector3 origin = midpoint;
                    Vector3 target = objects[linkedActor].midpoint;

                    float AdaptiveSize = 0.5f / Vector3.Distance(origin, target);
                    if (AdaptiveSize < 0.25f)
                    {
                        lr.widthCurve = new AnimationCurve(
                            new Keyframe(0, 0f),
                            new Keyframe(AdaptiveSize, 0.095f),
                            new Keyframe(0.999f - AdaptiveSize, 0.095f),  // neck of arrow
                            new Keyframe(1 - AdaptiveSize, 0.5f), // max width of arrow head
                            new Keyframe(1, 0f)); // tip of arrow
                        lr.positionCount = 5;
                        lr.SetPositions(new Vector3[] {
                            origin,
                            Vector3.Lerp(origin, target, AdaptiveSize),
                            Vector3.Lerp(origin, target, 0.999f - AdaptiveSize),
                            Vector3.Lerp(origin, target, 1 - AdaptiveSize),
                            target });
                    }
                    else {
                        lr.widthCurve = new AnimationCurve(
                            new Keyframe(0, 0.095f),
                            new Keyframe(1, 0.095f)); // tip of arrow
                        lr.positionCount = 2;
                        lr.SetPositions(new Vector3[] { origin, target });
                    }

                    lr.enabled = EnableBoxCollider && Settings.ShowLinks;

                    index++;
                }
            }
        }

        private void ClearSprites(SpriteRenderer[] sprites) 
        {
            // Clear old array
            if (sprites == null) 
                return;

            foreach (SpriteRenderer t in sprites) 
            {
                GameObject g = t?.gameObject;
                Destroy(t);
                Destroy(g);
            }

            Array.Clear(sprites, 0, sprites.Length);
        }

        // Delete this event properly
        public void Delete() {
            // Remove this from the event list
            Controller.obj.levelController.Objects.Remove(this);
            // Remove the data
            LevelEditorData.Level.EventData.Remove(ObjData);
            // Remove GBA links
            if (gbaLinkLines != null) {
                foreach (var link in gbaLinkLines) {
                    if(link != null) Destroy(link.gameObject);
                }
            }
            // Remove all children
            ClearSprites(prefabRenderers);
            ClearSprites(prefabRenderersCollision);
            ClearSprites(prefabRendersObjCollision);
            // Remove from the position dictionary
            Controller.obj.levelEventController.ObjPositions.Remove(this);
            // Remove self
            Destroy(gameObject);
        }
    }
}