using System;
using System.Collections.Generic;
using System.Linq;
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
        public virtual Unity_ObjAnimationCollisionPart[] ObjCollision => new Unity_ObjAnimationCollisionPart[0];
        public abstract Unity_ObjAnimation CurrentAnimation { get; }
        public virtual int AnimationFrame { get; set; }
        public virtual int AnimationIndex { get; set; }
        public abstract int AnimSpeed { get; }
        public float AnimationFrameFloat { get; set; }
        protected int? PrevAnimIndex { get; set; }
        public abstract int GetAnimIndex { get; }
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

		#region UI States
		public string[] UIStateNames {
            get {
                if (!IsUIStateArrayUpToDate) {
                    RecalculateUIStates();
                }

                return UIStates.Select(x => x.DisplayName).ToArray();
            }
        }
        public int CurrentUIState {
            get {
                if (!IsUIStateArrayUpToDate) {
                    RecalculateUIStates();
                }

                int i;
                i = UIStates.FindItemIndex(x => x.IsCurrentState(this));

                return i == -1 ? 0 : i;
            }
            set {
                if (value == CurrentUIState || UIStates == null || value >= UIStates.Length)
                    return;

                UIStates[value]?.Apply(this);
            }
        }
        public int? OverrideAnimIndex { get; set; }
        protected abstract bool IsUIStateArrayUpToDate { get; }
        protected UIState[] UIStates { get; set; }
        protected abstract void RecalculateUIStates();

        protected abstract class UIState {
            public UIState(string displayName) {
                DisplayName = displayName;
                IsState = true;
            }
            public UIState(string displayName, int animIndex) {
                DisplayName = displayName;
                IsState = false;
                AnimIndex = animIndex;
            }

            public string DisplayName { get; protected set; }
            public bool IsState { get; protected set; }
            public int AnimIndex { get; protected set; }

            public abstract void Apply(Unity_Object obj);
            public abstract bool IsCurrentState(Unity_Object obj);
        }
		#endregion
	}
}