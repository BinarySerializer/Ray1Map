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
        public bool ShowGizmo => Settings.ShowDefaultObjIcons && ObjData.CurrentAnimation == null && IsVisible && (!HasObjCollision || !Settings.ShowObjCollision);
        public bool EnableBoxCollider => IsVisible && (ObjData.CurrentAnimation != null || ShowGizmo || (HasObjCollision && ShowCollision));
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
        public bool ForceShowOneWayLinks { get; set; } // Used for some screenshots
        public int Layer => (ObjData.GetLayer(Index) ?? Index) * 128;

        public bool HasInitialized { get; set; }
        private int CurrentFrame { get; set; } = -1;
        private bool CurrentMirrorX { get; set; }
        private bool CurrentMirrorY { get; set; }
        private bool CurrentShowCollision { get; set; } = false;

        #endregion

        public LineRenderer[] oneWayLinkLines;
        public bool[] connectedOneWayLinkLines;
        // Default sprite
        public SpriteRenderer defaultRenderer;
        // Reference to spritepart prefab
        public GameObject prefabSpritepart;
        // Reference to the box prefab
        public GameObject prefabBox;
        // Reference to the created renderers
        public SpriteRenderer[] animSpriteRenderers;
        public SpriteRenderer[] animCollisionRenderers;
        public SpriteRenderer[] objCollisionRenderers;
        // Reference to box collider
        public BoxCollider2D boxCollider;
        public BoxCollider boxCollider3D;
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
        public Vector3 midpoint;

        public AudioClip currentSoundEffect;
        public Unity_Object.ObjectType PrevObjType;

        public void Init() {
            UpdatePosition();
            InitGizmo();
        }

        private void Start() 
        {
            transform.rotation = Quaternion.identity;

            // Snap link cube position
            linkCube.position = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
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

                case Unity_ObjAnimationCollisionPart.CollisionType.SizeChange:
                case Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox:
                    collisionSpriteRenderer.color = new Color(0f, 1f, 0f, 0.4f);
                    break;

                case Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox:
                    collisionSpriteRenderer.color = new Color(1f, 0.7f, 0f, 0.4f);
                    break;

                case Unity_ObjAnimationCollisionPart.CollisionType.HitTriggerBox:
                case Unity_ObjAnimationCollisionPart.CollisionType.ExitLevel:
                    collisionSpriteRenderer.color = new Color(1f, 0f, 1f, 0.4f);
                    break;

                case Unity_ObjAnimationCollisionPart.CollisionType.Gendoor:
                    collisionSpriteRenderer.color = new Color(0f, 1f, 0.7f, 0.4f);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdatePosition()
        {
            
            if (ObjData is Unity_Object_3D && LevelEditorData.Level?.IsometricData != null) {
                UpdatePosition3D();
                return;
            }
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

        public void InitGizmo() {
            var gizmos = Controller.obj.levelEventController.gizmos;
            LevelEventController.Gizmo gizmo = gizmos.FirstOrDefault(g => g.name == ObjData.Type.ToString());
            if(gizmo == null) gizmo = gizmos[0];
            if (ObjData is Unity_Object_3D && LevelEditorData.Level?.IsometricData != null) {
                Sprite spr = gizmo.sprite3D;
                defaultRenderer.sprite = spr;
            } else {
                Sprite spr = gizmo.sprite;
                defaultRenderer.sprite = spr;
            }
            if (ShowGizmo) {
                UpdateGizmoPosition(ObjData.ObjCollision, ObjData.Pivot);
            }
        }
        public void UpdateGizmoPosition(Unity_ObjAnimationCollisionPart[] collision, Vector2 pivot) {
            if (collision == null || collision.Length == 0) {
                defaultRenderer.transform.localPosition = Vector3.zero;
            } else {
                var mirroredX = ObjData.FlipHorizontally;
                var mirroredY = ObjData.FlipVertically;
                Vector2 center = new Vector2(
                    collision.Average(c => (c.XPosition - pivot.x + c.Width / 2f) * (mirroredX ? -1f : 1f) * ObjData.Scale + pivot.x),
                    collision.Average(c => (-c.YPosition - pivot.y - c.Height / 2f) * (mirroredY ? -1f : 1f) * ObjData.Scale + pivot.y)) / LevelEditorData.Level.PixelsPerUnit;

                defaultRenderer.transform.localPosition = center;
            }
        }
        public void SetGizmoBoxCollider() {
            if (boxCollider != null) {
                var center = new Vector2(defaultRenderer.transform.localPosition.x, defaultRenderer.transform.localPosition.y);
                boxCollider.size = Vector2.one * 1.45f;
                boxCollider.offset = Vector2.zero + center;
            } else if (boxCollider3D != null) {
                var center = new Vector3(defaultRenderer.transform.localPosition.x, defaultRenderer.transform.localPosition.y);
                boxCollider3D.size = new Vector3(1.45f, 1.925f, 0.1f);
                boxCollider3D.center = new Vector3(0, 1.925f / 2, 0f) + center;
            }
        }

        public void UpdatePosition3D() {
            gameObject.layer = LayerMask.NameToLayer("3D Object");
            defaultRenderer.gameObject.layer = gameObject.layer;
            Unity_Object_3D obj = (Unity_Object_3D)ObjData; 
            Vector3 pos = obj.Position;

            // Convert position to unity space
            //transform.position = new Vector3(pos.x / 16f - 0.5f, pos.z / 32f, -pos.y / 16f + 0.5f);
            Vector3 isometricScale = LevelEditorData.Level.IsometricData.AbsoluteObjectScale;
            transform.position = Vector3.Scale(new Vector3(pos.x, pos.z, -pos.y), isometricScale);

            // Billboard
            Camera cam = Controller.obj.levelEventController.editor.cam.camera3D;
            Quaternion inverseParent = transform.parent != null ? Quaternion.Inverse(transform.parent.rotation) : Quaternion.identity;
            Quaternion parentRot = transform.parent != null ? transform.parent.rotation * Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, -90, 0);
            Quaternion lookRot = (inverseParent * Quaternion.LookRotation(
                cam.transform.rotation * Vector3.forward,
                cam.transform.rotation * Vector3.up));
            transform.localRotation = lookRot;

            // Create boxCollider

            if (boxCollider3D == null) {
                if (boxCollider != null) Destroy(boxCollider);
                if (boxCollider == null) { // Check if object is destroyed
                    boxCollider = null; // Remove the reference. Despite the null check earlier the reference still exists.
                    boxCollider3D = gameObject.AddComponent<BoxCollider>();
                    boxCollider3D.center = Vector3.zero;
                    boxCollider3D.size = new Vector3(1,1,0.1f);
                }
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

            // Update object
            ObjData.OnUpdate();

            defaultRenderer.enabled = true;

            bool frameUpdated = false;
            bool collisionUpdated = false;
            if (ShowCollision != CurrentShowCollision) {
                CurrentShowCollision = ShowCollision;
                collisionUpdated = true;
            }
            if (ObjData.ShouldUpdateAnimation())
            {
                CurrentFrame = -1;
                CurrentShowCollision = ShowCollision;
                frameUpdated = true;

                // Clear old arrays
                ClearSprites(animSpriteRenderers);
                ClearSprites(animCollisionRenderers);

                // If animation is null, show gizmo instead
                if (ObjData.CurrentAnimation != null) {
                    // Reset the current frame
                    ObjData.ResetFrame();

                    // Get the amount of sprite layers per frame
                    var spritesLength = ObjData.CurrentAnimation.Frames.Max(f => f.SpriteLayers.Length);

                    // Get the amount of collision layers per frame
                    var collisionLength = ObjData.CurrentAnimation.Frames.Max(f => f.CollisionLayers?.Length ?? 0);

                    // Create arrays
                    animSpriteRenderers = new SpriteRenderer[spritesLength];
                    animCollisionRenderers = new SpriteRenderer[collisionLength];

                    bool is3D = ObjData is Unity_Object_3D && LevelEditorData.Level.IsometricData != null;

                    // Populate sprites
                    for (int i = 0; i < spritesLength; i++)
                    {
                        // Instantiate prefab
                        SpriteRenderer newRenderer = Instantiate(prefabSpritepart, transform).GetComponent<SpriteRenderer>();
                        newRenderer.sortingOrder = is3D ? 0 : (Layer + i);
                        newRenderer.sortingLayerName = ObjData.MapLayer == 2 ? "Object Sprites Back" : "Object Sprites";
                        if (is3D) newRenderer.gameObject.layer = LayerMask.NameToLayer("3D Object");

                        newRenderer.transform.localPosition = new Vector3(0, 0, is3D ? 0 : (spritesLength - i));
                        newRenderer.transform.localRotation = Quaternion.identity;
                        newRenderer.transform.localScale = Vector3.one * ObjData.Scale;

                        // Add to list
                        animSpriteRenderers[i] = newRenderer;
                    }

                    // Populate collision boxes
                    for (int i = 0; i < collisionLength; i++)
                    {
                        // Instantiate prefab
                        var newRenderer = Instantiate(prefabBox, transform).GetComponent<SpriteRenderer>();
                        newRenderer.sortingOrder = is3D ? 0 : (Layer + spritesLength + i);
                        if (is3D) newRenderer.gameObject.layer = LayerMask.NameToLayer("3D Object");

                        newRenderer.transform.localRotation = Quaternion.identity;

                        // Add to list
                        animCollisionRenderers[i] = newRenderer;
                    }
                }
            }

            // Get the current animation
            var anim = ObjData.CurrentAnimation;

            // 
            bool defaultRendererEnabled = ShowGizmo;
            if (defaultRenderer.gameObject.activeSelf != defaultRendererEnabled) {
                defaultRenderer.gameObject.SetActive(defaultRendererEnabled);
            }

            UpdatePosition();

            // Don't move link cube if it's part of a link

            if (ObjData.EditorLinkGroup != 0)
                linkCube.position = linkCubeLockPosition;
            else
                linkCubeLockPosition = linkCube.position;

            var isVisible = IsVisible;
            // Update sprite parts in the animation
            if (anim != null)
            {
                // Get properties
                var frame = ObjData.AnimationFrame;
                if (CurrentFrame != frame) {
                    frameUpdated = true;
                    CurrentFrame = frame;
                }
                var sprites = ObjData.Sprites;
                var pivot = ObjData.Pivot;
                var mirroredX = ObjData.FlipHorizontally;
                var mirroredY = ObjData.FlipVertically;
                if (CurrentMirrorX != mirroredX || CurrentMirrorY != mirroredY) {
                    frameUpdated = true;
                    CurrentMirrorX = mirroredX;
                    CurrentMirrorY = mirroredY;
                }

                if (frame >= anim.Frames.Length)
                    throw new Exception($"Invalid frame index {frame} with length {anim.Frames.Length} for obj {Index}. {ObjData.PrimaryName}-{ObjData.SecondaryName}");

                // Update sprites
                for (int i = 0; i < anim.Frames[frame].SpriteLayers.Length; i++)
                {
                    var layer = anim.Frames[frame].SpriteLayers[i];

                    // Get the sprite index
                    var spriteIndex = layer.ImageIndex;

                    // Change it if the event is multi-colored (Rayman 1 only)
                    if (ObjData is Unity_Object_R1 objr1 && objr1.EventData.Type.IsMultiColored())
                        spriteIndex += ((sprites.Count / 6) * objr1.EventData.HitPoints);

                    if (animSpriteRenderers.Length <= i)
                        continue;

                    if (frameUpdated) {
                        // Set the sprite, skipping sprites which are out of bounds
                        if (spriteIndex >= sprites.Count && (LevelEditorData.CurrentSettings.EngineVersion != EngineVersion.R2_PS1 || spriteIndex < 0xFFF)) {
                            print($"Sprite index too high: {ObjData.PrimaryName}|{ObjData.SecondaryName}: {spriteIndex} >= {sprites.Count}");
                        }
                        animSpriteRenderers[i].sprite = spriteIndex >= sprites.Count ? null : sprites[spriteIndex];

                        var layerMirroredX = layer.IsFlippedHorizontally;
                        var layerMirroredY = layer.IsFlippedVertically;

                        // Indicate if the sprites should be flipped
                        animSpriteRenderers[i].flipX = (layerMirroredX ^ mirroredX);
                        animSpriteRenderers[i].flipY = (layerMirroredY ^ mirroredY);

                        // Get the dimensions
                        var w = animSpriteRenderers[i].sprite == null ? 0 : animSpriteRenderers[i].sprite.textureRect.width;
                        var h = animSpriteRenderers[i].sprite == null ? 0 : animSpriteRenderers[i].sprite.textureRect.height;

                        var xx = layer.XPosition + (layerMirroredX ? w : 0);

                        var yy = -(layer.YPosition + (layerMirroredY ? h : 0));

                        // scale
                        Vector2 pos = new Vector2(
                            ((xx - pivot.x) * (mirroredX ? -1f : 1f) * ObjData.Scale + pivot.x) / (float)LevelEditorData.Level.PixelsPerUnit,
                            ((yy - pivot.y) * (mirroredY ? -1f : 1f) * ObjData.Scale + pivot.y) / (float)LevelEditorData.Level.PixelsPerUnit);

                        animSpriteRenderers[i].transform.localPosition = new Vector3(pos.x, pos.y, animSpriteRenderers[i].transform.localPosition.z);
                        animSpriteRenderers[i].transform.localScale = Vector3.one * ObjData.Scale;

                        animSpriteRenderers[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                        // First transform based on layer properties
                        if ((layer.Rotation.HasValue && layer.Rotation.Value != 0) || (layer.Scale.HasValue && layer.Scale.Value != Vector2.one)) {

                            Vector3 transformOrigin = new Vector3(
                                (((layer.TransformOriginX - pivot.x) * (mirroredX ? -1f : 1f) * ObjData.Scale + pivot.x) / (float)LevelEditorData.Level.PixelsPerUnit),
                                ((-layer.TransformOriginY - pivot.y) * (mirroredY ? -1f : 1f) * ObjData.Scale + pivot.y) / (float)LevelEditorData.Level.PixelsPerUnit,
                                animSpriteRenderers[i].transform.localPosition.z);

                            // Scale first
                            if (layer.Scale.HasValue && layer.Scale.Value != Vector2.one) {
                                Vector3 scaleValue = new Vector3(layer.Scale.Value.x, layer.Scale.Value.y, 1f);
                                animSpriteRenderers[i].transform.localScale = Vector3.Scale(Vector3.one * ObjData.Scale, scaleValue);
                                Vector3 scaledPos = Vector3.Scale(animSpriteRenderers[i].transform.localPosition - transformOrigin, scaleValue);
                                animSpriteRenderers[i].transform.localPosition = transformOrigin + scaledPos;
                            }
                            // Then rotate
                            if (layer.Rotation.HasValue && layer.Rotation.Value != 0) {
                                /*Quaternion rotation = Quaternion.Euler(0, 0, layer.Rotation * 180f);*/
                                //Vector3 rotationOrigin = Vector3.zero;

                                animSpriteRenderers[i].transform.RotateAround(transform.TransformPoint(transformOrigin), new Vector3(0, 0, 1), layer.Rotation.Value * ((mirroredX ^ mirroredY) ? -1f : 1f));
                                /*    Vector2 relativePos = pos - rotationOrigin;
                                Vector2 rotatedPos = rotation * relativePos;
                                prefabRenderers[i].transform.localRotation = rotation;
                                prefabRenderers[i].transform.localPosition = new Vector3(relativePos.x + rotatedPos.x, relativePos.y + rotatedPos.y, prefabRenderers[i].transform.localPosition.z);*/
                            }
                        }

                        // Then transform based on object properties
                        if ((ObjData.Rotation.HasValue && ObjData.Rotation.Value != 0)) {

                            Vector3 transformOrigin = new Vector3(
                                pivot.x / (float)LevelEditorData.Level.PixelsPerUnit,
                                pivot.y / (float)LevelEditorData.Level.PixelsPerUnit,
                                animSpriteRenderers[i].transform.localPosition.z);

                            // Then rotate
                            if (ObjData.Rotation.HasValue && ObjData.Rotation.Value != 0) {
                                animSpriteRenderers[i].transform.RotateAround(transform.TransformPoint(transformOrigin), new Vector3(0, 0, 1), ObjData.Rotation.Value * ((mirroredX ^ mirroredY) ? -1f : 1f));
                            }
                        }
                    }

                    // Get visibility
                    animSpriteRenderers[i].enabled = isVisible;
                    animSpriteRenderers[i].color = ObjData.IsDisabled ? new Color(1, 1, 1, 0.5f) : Color.white;
                }

                if (frameUpdated) {
                    // Remove unused sprites
                    for (int i = anim.Frames[frame].SpriteLayers.Length; i < animSpriteRenderers.Length; i++) {
                        animSpriteRenderers[i].sprite = null;
                        animSpriteRenderers[i].enabled = false;
                    }

                    // Remove unused collision layers
                    for (int i = anim.Frames[frame].CollisionLayers.Length; i < animCollisionRenderers.Length; i++) {
                        animCollisionRenderers[i].sprite = null;
                        animCollisionRenderers[i].enabled = false;
                    }
                }
                if (frameUpdated || collisionUpdated) {
                    // Update collision
                    for (int i = 0; i < anim.Frames[frame].CollisionLayers.Length; i++) {
                        SetCollisionBox(anim.Frames[frame].CollisionLayers[i], animCollisionRenderers[i], pivot);
                        animCollisionRenderers[i].enabled = CurrentShowCollision;
                    }
                }
            }

            if (ShowGizmo) {
                UpdateGizmoPosition(ObjData.ObjCollision, ObjData.Pivot);
            }
            if (CurrentShowCollision && (frameUpdated || collisionUpdated)) {
                // Update object collision
                var objCol = ObjData.ObjCollision;

                // Update new 
                if ((objCollisionRenderers == null && objCol != null && objCol.Length > 0) || objCol?.Length != objCollisionRenderers?.Length) {
                    // Clear old object collision array
                    ClearSprites(objCollisionRenderers);

                    if (objCol != null && objCol.Length > 0) {
                        if (CurrentShowCollision) {
                            objCollisionRenderers = new SpriteRenderer[objCol.Length];
                            bool is3D = ObjData is Unity_Object_3D && LevelEditorData.Level.IsometricData != null;

                            // Instantiate prefabs
                            for (int i = 0; i < objCol.Length; i++) {
                                objCollisionRenderers[i] = Instantiate(prefabBox, transform).GetComponent<SpriteRenderer>();
                                objCollisionRenderers[i].sortingOrder = is3D ? 0 : Layer;

                                objCollisionRenderers[i].transform.localPosition = new Vector3(0, 0, is3D ? 0.1f : (objCol.Length - i));
                                objCollisionRenderers[i].transform.localRotation = Quaternion.identity;
                                objCollisionRenderers[i].transform.localScale = Vector3.one * ObjData.Scale;
                            }
                        }
                    } else {
                        objCollisionRenderers = null;
                    }
                }

                // Update object collision boxes
                if (objCollisionRenderers != null) {
                    for (var i = 0; i < objCollisionRenderers.Length; i++) {
                        SetCollisionBox(objCol[i], objCollisionRenderers[i], ObjData.Pivot);
                        objCollisionRenderers[i].enabled = CurrentShowCollision;
                    }
                }
            } else if(!CurrentShowCollision) {
                if (objCollisionRenderers != null) {
                    for (var i = 0; i < objCollisionRenderers.Length; i++) {
                        objCollisionRenderers[i].enabled = false;
                    }
                }
            }

            if (frameUpdated) {
                // Update the follow sprite line (Rayman 1 only)
                if (ObjData is Unity_Object_R1 r1 && anim != null) {
                    var animLayer = anim.Frames[r1.AnimationFrame].SpriteLayers.ElementAtOrDefault(r1.EventData.FollowSprite);
                    var imgDescr = r1.ObjManager.DES.ElementAtOrDefault(r1.DESIndex)?.Data?.ImageDescriptors.ElementAtOrDefault(animLayer?.ImageIndex ?? -1);

                    followSpriteLine.localPosition = new Vector2(
                        ((animLayer?.XPosition ?? 0) + (imgDescr?.HitBoxOffsetX ?? 0)) / (float)LevelEditorData.Level.PixelsPerUnit,
                        -((animLayer?.YPosition ?? 0) + (imgDescr?.HitBoxOffsetY ?? 0)) / (float)LevelEditorData.Level.PixelsPerUnit - (r1.EventData.OffsetHY / (float)LevelEditorData.Level.PixelsPerUnit));

                    var w = (animSpriteRenderers.ElementAtOrDefault(r1.EventData.FollowSprite)?.sprite == null) ? 0 : imgDescr?.HitBoxWidth ?? 0;
                    followSpriteLine.localScale = new Vector2(w, 1f);
                }
            }

            // Update the collider size for when selecting the events
            if (frameUpdated || collisionUpdated) {
                if (anim != null || objCollisionRenderers?.Any() == true) {
                    var sprites = anim != null ? animSpriteRenderers : objCollisionRenderers;

                    // Set box collider size to be the combination of all parts
                    float leftX = 0, bottomY = 0, rightX = 0, topY = 0;
                    float w = 1f;
                    float h = 1f;
                    float centerX = 0f;
                    float centerY = 0f;
                    bool first = true;
                    bool hasSprites = false;
                    foreach (SpriteRenderer part in sprites) {
                        if (part.sprite == null)
                            continue;

                        hasSprites = true;

                        Bounds b = part.bounds;
                        if (boxCollider3D != null) {
                            b = part.sprite?.bounds ?? b;
                            var scl = part.transform.localScale;
                            if (part.flipX) scl.x = scl.x * -1;
                            if (part.flipY) scl.y = scl.y * -1;
                            b = new Bounds((part.transform.localPosition + Vector3.Scale(b.center, scl)) * LevelEditorData.Level.PixelsPerUnit, Vector3.Scale(b.size, scl) * LevelEditorData.Level.PixelsPerUnit);
                        } else {
                            b = new Bounds(transform.InverseTransformPoint(b.center) * LevelEditorData.Level.PixelsPerUnit, transform.InverseTransformVector(b.size) * LevelEditorData.Level.PixelsPerUnit);
                        }
                        if (b.min.x < leftX || first) leftX = b.min.x;
                        if (b.min.y < bottomY || first) bottomY = b.min.y;
                        if (b.max.x > rightX || first) rightX = b.max.x;
                        if (b.max.y > topY || first) topY = b.max.y;

                        if (first)
                            first = false;
                    }

                    if (hasSprites) {
                        w = (rightX - leftX) / LevelEditorData.Level.PixelsPerUnit;
                        h = (topY - bottomY) / LevelEditorData.Level.PixelsPerUnit;
                        centerX = leftX / LevelEditorData.Level.PixelsPerUnit + w / 2f;
                        centerY = topY / LevelEditorData.Level.PixelsPerUnit - h / 2f;
                        w = Mathf.Abs(w);
                        h = Mathf.Abs(h);
                        if (boxCollider != null) {
                            boxCollider.size = new Vector2(w, h);
                            boxCollider.offset = new Vector2(centerX, centerY);

                        } else if (boxCollider3D != null) {
                            boxCollider3D.size = new Vector3(w, h, 0.1f);
                            boxCollider3D.center = new Vector2(centerX, centerY);
                        }
                    }
                } else {
                    SetGizmoBoxCollider();
                }
            }
            if (ShowGizmo && anim == null && !CurrentShowCollision) {
                SetGizmoBoxCollider();
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
                    var hy = -(r2bj.EventData.CollisionData?.OffsetHY ?? 0);

                    offsetCrossHY.localPosition = new Vector2(pivot.x / LevelEditorData.Level.PixelsPerUnit, hy / (float)LevelEditorData.Level.PixelsPerUnit);
                }
            }

            if (PrevObjType != ObjData.Type)
            {
                PrevObjType = ObjData.Type;
                InitGizmo();
            }
            bool enableBoxCollider = EnableBoxCollider;
            if (boxCollider != null) {
                // Update visibility
                boxCollider.enabled = enableBoxCollider;

                // Set new midpoint
                midpoint = new Vector3(transform.position.x + boxCollider.offset.x, transform.position.y + boxCollider.offset.y, 0);
            } else if (boxCollider3D != null) {
                // Update visibility
                boxCollider3D.enabled = enableBoxCollider;

                // Set new midpoint
                midpoint = transform.TransformPoint(boxCollider3D.center);
            }
            // Set link line to cube
            lineRend.SetPosition(0, midpoint);
            lineRend.SetPosition(1, linkCube.position);

            // Update link colors
            if (ObjData.EditorLinkGroup == 0)
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
                isVisible && 
                // Make sure links are set to show
                Settings.ShowLinks && 
                // Only show active links on web
                !(Settings.HideUnusedLinks && ObjData.EditorLinkGroup == 0) && 
                // Only show if available
                ObjData.CanBeLinkedToGroup;

            lineRend.enabled = showLinks;
            void SetGameObjectActive(GameObject gao, bool active) {
                if(gao.activeSelf != active) gao.SetActive(active);
            }
            SetGameObjectActive(linkCube.gameObject, showLinks);

            // Change the offsets visibility
            bool showOffsets = ShowOffsets;
            SetGameObjectActive(offsetOrigin.gameObject, showOffsets);
            SetGameObjectActive(offsetCrossBX.gameObject, showOffsets && offsetCrossBX.transform.position != Vector3.zero);
            SetGameObjectActive(offsetCrossHY.gameObject, showOffsets && (ObjData is Unity_Object_R1 || ObjData is Unity_Object_R2) && offsetCrossHY.transform.position != Vector3.zero);

            var engineVersion = LevelEditorData.CurrentSettings.EngineVersion;
            followSpriteLine.gameObject.SetActive(
                ShowCollision && 
                ObjData is Unity_Object_R1 r1o && 
                r1o.EventData.GetFollowEnabled(LevelEditorData.CurrentSettings) && 
                !(engineVersion == EngineVersion.R1_PS1_JP || engineVersion == EngineVersion.R1_PS1_JPDemoVol3 || engineVersion == EngineVersion.R1_PS1_JPDemoVol6 || engineVersion == EngineVersion.R1_Saturn));

            // Update one-way link lines
            if (oneWayLinkLines != null)
                for (var i = 0; i < oneWayLinkLines.Length; i++)
                    oneWayLinkLines[i].enabled = (enableBoxCollider && Settings.ShowLinks && ObjData.CanBeLinked && connectedOneWayLinkLines[i]) || ForceShowOneWayLinks;

            HasInitialized = true;
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
            if (oneWayLinkLines != null) {
                foreach (var link in oneWayLinkLines) {
                    if(link != null) Destroy(link.gameObject);
                }
            }
            // Remove all children
            ClearSprites(animSpriteRenderers);
            ClearSprites(animCollisionRenderers);
            ClearSprites(objCollisionRenderers);
            // Remove from the position dictionary
            Controller.obj.levelEventController.ObjPositions.Remove(this);
            // Remove self
            Destroy(gameObject);
        }
    }
}