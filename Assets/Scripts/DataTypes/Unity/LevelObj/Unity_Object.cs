using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_Object
    {
        // Position
        public abstract short XPosition { get; set; }
        public abstract short YPosition { get; set; }

        // Editor
        public abstract string DebugText { get; }
        public int EditorLinkGroup { get; set; }
        public bool HasPendingEdits { get; set; }

        // TODO: Remove this once we rework the editor
        [Obsolete]
        public abstract ILegacyEditorWrapper LegacyWrapper { get; }

        // Attributes
        public virtual bool IsAlways => false;
        public virtual bool IsEditor => false;
        public virtual bool IsActive => true;

        // Display properties
        public abstract string PrimaryName { get; } // Official
        public abstract string SecondaryName { get; } // Unofficial
        public string Name => PrimaryName ?? SecondaryName;
        public virtual int? GetLayer(int index) => null;
        public virtual int? MapLayer => null;
        public virtual float Scale => 1f;
        public virtual bool FlipHorizontally => false;
        public virtual bool FlipVertically => false;
        public virtual bool IsVisible
        {
            get
            {
                if (LevelEditorData.Level.Rayman == this)
                    return Settings.ShowRayman;

                if (IsEditor)
                    return Settings.ShowEditorEvents;

                if (IsAlways)
                    return Settings.ShowAlwaysEvents || (Settings.LoadFromMemory && IsActive);

                // Default to visible
                return true;
            }
        }
        public virtual bool IsDisabled => Settings.LoadFromMemory && !IsActive;

        // Animations
        // TODO: Expose current frame and use that mostly
        public abstract Unity_ObjAnimation CurrentAnimation { get; }
        public virtual byte AnimationFrame { get; set; }
        public virtual byte AnimationIndex { get; set; }
        public abstract byte AnimSpeed { get; }
        public float AnimationFrameFloat { get; set; }
        protected byte? PrevAnimIndex { get; set; }
        public abstract byte GetAnimIndex { get; }
        public abstract IList<Sprite> Sprites { get; }
        public virtual Vector2 Pivot => Vector2.zero;
        public void UpdateFrame()
        {
            if (!ShouldUpdateFrame())
                return;

            // Increment frame if animating
            if (Settings.AnimateSprites && AnimSpeed > 0)
                AnimationFrameFloat += (60f / AnimSpeed) * Time.deltaTime;

            // Loop around if over the frame limit
            bool isFinished = false;
            if (AnimationFrameFloat >= CurrentAnimation.Frames.Length) {
                AnimationFrameFloat %= CurrentAnimation.Frames.Length;
                isFinished = true;
            }

            // Update the frame
            AnimationFrame = (byte)Mathf.FloorToInt(AnimationFrameFloat);

            // Trigger animation finished event
            if (isFinished) {
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

            // Check if the animation has changed
            if (PrevAnimIndex != AnimationIndex)
            {
                // Update the animation index
                PrevAnimIndex = AnimationIndex;

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

        public abstract string[] UIStateNames { get; }
        public abstract int CurrentUIState { get; set; }
        public byte? OverrideAnimIndex { get; set; }
    }
}