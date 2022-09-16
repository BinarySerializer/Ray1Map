using System.Collections.Generic;
using UnityEngine;

namespace Ray1Map
{
    public abstract class Unity_SpriteObject : Unity_Object
    {
        // Position
        public override Vector3 Position
        {
            get => new Vector3(XPosition, YPosition);
            set
            {
                XPosition = (short)value.x;
                YPosition = (short)value.y;
            }
        }
        public abstract short XPosition { get; set; }
        public abstract short YPosition { get; set; }

        // Links
        public int EditorLinkGroup { get; set; }
        public virtual IEnumerable<int> Links => new int[0];
        public virtual IEnumerable<LinkType> LinkTypes => null;
        public virtual bool CanBeLinkedToGroup => false;
        public virtual bool CanBeLinked => false;

        // Editor
        public virtual BaseLegacyEditorWrapper LegacyWrapper => new BaseLegacyEditorWrapper();

        // Display properties
        public virtual int? GetLayer(int index) => null;
        public virtual int? MapLayer => null;
        public virtual float Scale => 1f;
        public virtual bool FlipHorizontally => false;
        public virtual bool FlipVertically => false;
        public virtual float? Rotation => null;

        // Animations
        public virtual Unity_ObjAnimationCollisionPart[] ObjCollision => new Unity_ObjAnimationCollisionPart[0];
        public virtual float? DetectionRadius => null;
        public virtual DetectionCubeData DetectionCube => null;
        public abstract Unity_ObjAnimation CurrentAnimation { get; }
        public virtual int AnimationFrame { get; set; }
        public virtual int? AnimationIndex { get; set; }
        public abstract int AnimSpeed { get; }
        public float AnimationFrameFloat { get; set; }
        protected int? PrevAnimIndex { get; set; }
        public abstract int? GetAnimIndex { get; }
        private int PrevSpriteID { get; set; }
        protected abstract int GetSpriteID { get; }
        public abstract IList<Sprite> Sprites { get; }
        public virtual Vector2 Pivot => Vector2.zero;
        public void UpdateFrame()
        {
            if (!ShouldUpdateFrame())
                return;

            // Increment frame if animating
            if (Settings.AnimateSprites && AnimSpeed > 0)
                AnimationFrameFloat += (LevelEditorData.FramesPerSecond / AnimSpeed) * Time.deltaTime;

            // Loop around if over the frame limit
            bool isFinished = false;
            if (AnimationFrameFloat >= CurrentAnimation.Frames.Length)
            {
                AnimationFrameFloat %= CurrentAnimation.Frames.Length;
                isFinished = true;
            }

            // Update the frame
            AnimationFrame = Mathf.FloorToInt(AnimationFrameFloat);

            // Trigger animation finished event
            if (isFinished || AnimSpeed == 0)
            {
                OnFinishedAnimation();
            }
        }
        protected virtual bool ShouldUpdateFrame() => true;
        protected virtual void OnFinishedAnimation() { }
        public virtual bool ShouldUpdateAnimation()
        {
            // Update the animation index if not loading from memory
            if (!Settings.LoadFromMemory)
                AnimationIndex = GetAnimIndex;

            // Check if the animation or sprites have changed
            if (PrevAnimIndex != AnimationIndex || PrevSpriteID != GetSpriteID)
            {
                // Update the animation index
                PrevAnimIndex = AnimationIndex;
                PrevSpriteID = GetSpriteID;

                return true;
            }

            return false;
        }
        public virtual void ResetFrame()
        {
            if (!Settings.LoadFromMemory)
            {
                AnimationFrame = 0;
                AnimationFrameFloat = 0;
            }
        }

        public enum LinkType
        {
            Unknown,
            WakeUp,
            Sleep,
            Destroy,
            Reset,
            ResetWakeUp
        }

        public record DetectionCubeData(Vector3 Position, Quaternion Rotation, Vector3 Size);
    }
}