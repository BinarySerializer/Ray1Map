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
        public virtual IEnumerable<int> Links => new int[0];
        public virtual IEnumerable<LinkType> LinkTypes => null;
        public bool HasPendingEdits { get; set; }
        public abstract R1Serializable SerializableData { get; }

        public virtual IEnumerable<int> GetLocIndices => new int[0];

        public abstract ILegacyEditorWrapper LegacyWrapper { get; }

        // Attributes
        public virtual bool IsAlways => false;
        public virtual bool IsEditor => false;
        public virtual bool IsActive => true;
        public virtual bool CanBeLinkedToGroup => false;
        public virtual bool CanBeLinked => false;

        // Display properties
        public abstract string PrimaryName { get; } // Official
        public abstract string SecondaryName { get; } // Unofficial
        public string Name => PrimaryName ?? SecondaryName;
        public virtual int? GetLayer(int index) => null;
        public virtual int? MapLayer => null;
        public virtual float Scale => 1f;
        public virtual bool FlipHorizontally => false;
        public virtual bool FlipVertically => false;
        public virtual float? Rotation => null;
        public virtual bool IsVisible
        {
            get
            {
                if (LevelEditorData.Level.Rayman == this)
                    return Settings.ShowRayman;

                if (IsEditor)
                    return Settings.ShowEditorObjects;

                if (IsAlways)
                    return Settings.ShowAlwaysObjects || (Settings.LoadFromMemory && IsActive);

                // Default to visible
                return true;
            }
        }
        public virtual bool IsDisabled => Settings.LoadFromMemory && !IsActive;

        // Events
        public virtual void OnUpdate() { }

        // Animations
        public virtual Unity_ObjAnimationCollisionPart[] ObjCollision => new Unity_ObjAnimationCollisionPart[0];
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
            if (isFinished || AnimSpeed == 0) {
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

        public enum ObjectType {
            Object,
            Trigger,
            Waypoint
        }
        public enum LinkType {
            Unknown,
            WakeUp,
            Sleep,
            Destroy,
            Reset,
            ResetWakeUp
        }
        public virtual ObjectType Type => Unity_Object.ObjectType.Object;

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
                if (value == CurrentUIState || UIStates == null || value >= UIStates.Length || value < 0)
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


        protected class DummyLegacyEditorWrapper : ILegacyEditorWrapper {
            public DummyLegacyEditorWrapper(Unity_Object obj) {
                Obj = obj;
            }

            private Unity_Object Obj { get; }

            public ushort Type { get; set; }

            public int DES { get; set; }

            public int ETA { get; set; }

            public byte Etat { get; set; }

            public byte SubEtat { get; set; }

            public int EtatLength => 0;
            public int SubEtatLength => 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
    }
}