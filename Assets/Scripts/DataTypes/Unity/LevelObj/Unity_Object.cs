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
        public string DebugText { get; set; }
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
        public abstract string DisplayName { get; }
        public virtual int Layer => 0;
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
        public abstract Unity_ObjAnimation CurrentAnimation { get; }
        public abstract byte CurrentAnimationFrame { get; set; }
        protected float CurrentAnimationFrameFloat { get; set; }
        protected byte? PrevAnimIndex { get; set; }
        public abstract IList<Sprite> Sprites { get; }
        public virtual Vector2 Pivot => Vector2.zero;
        public abstract void UpdateFrame();
        public abstract bool ShouldUpdateAnimation();
        public virtual void ResetFrame()
        {
            if (!Settings.LoadFromMemory) 
                CurrentAnimationFrame = 0;
        }
    }
}