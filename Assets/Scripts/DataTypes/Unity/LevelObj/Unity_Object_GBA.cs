using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBA : Unity_Object
    {
        public Unity_Object_GBA(GBA_Actor actor, Unity_ObjectManager_GBA objManager)
        {
            // Set properties
            Actor = actor;
            ObjManager = objManager;
        }

        public GBA_Actor Actor { get; }

        public Unity_ObjectManager_GBA ObjManager { get; }

        public GBA_ActorState State => GraphicsData?.States.ElementAtOrDefault(Actor.StateIndex);
        public Unity_ObjectManager_GBA.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(GraphicsDataIndex);
        
        public int GraphicsDataIndex
        {
            get => ObjManager.GraphicsDatas.FindItemIndex(x => x.Index == Actor.GraphicsDataIndex);
            set => Actor.GraphicsDataIndex = (byte)ObjManager.GraphicsDatas[value].Index;
        }


        public override short XPosition
        {
            get => (short)Actor.XPos;
            set => Actor.XPos = (ushort)value;
        }
        public override short YPosition
        {
            get => (short)Actor.YPos;
            set => Actor.YPos = (ushort)value;
        }

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool IsAlwaysEvent { get; set; }
        public override bool IsAlways => IsAlwaysEvent;

        public override string DisplayName => $"{Actor.ActorID}";

        public override bool FlipHorizontally => State?.Flags.HasFlag(GBA_ActorState.ActorStateFlags.HorizontalFlip) ?? false;
        public override bool FlipVertically => State?.Flags.HasFlag(GBA_ActorState.ActorStateFlags.VerticalFlip) ?? false;

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Graphics.Animations.ElementAtOrDefault(CurrentAnimIndex);
        public byte CurrentAnimIndex { get; set; }
        private byte _currentAnimationFrame;
        public override byte CurrentAnimationFrame
        {
            get => _currentAnimationFrame;
            set
            {
                _currentAnimationFrame = value;
                CurrentAnimationFrameFloat = value;
            }
        }

        public int AnimSpeed => CurrentAnimation.AnimSpeed.Value;

        public override IList<Sprite> Sprites => GraphicsData?.Graphics.Sprites;

        public override void UpdateFrame()
        {
            // Increment frame if animating
            if (Settings.AnimateSprites && AnimSpeed > 0)
                CurrentAnimationFrameFloat += (60f / AnimSpeed) * Time.deltaTime;

            // Update the frame
            _currentAnimationFrame = (byte)Mathf.FloorToInt(CurrentAnimationFrameFloat);

            // Loop back to first frame
            if (CurrentAnimationFrame >= CurrentAnimation.Frames.Length)
                CurrentAnimationFrame = 0;
        }

        public override bool ShouldUpdateAnimation()
        {
            // Update the animation index if not loading from memory
            if (!Settings.LoadFromMemory)
                CurrentAnimIndex = State?.AnimationIndex ?? Actor.StateIndex;

            // Check if the animation has changed
            if (PrevAnimIndex != CurrentAnimIndex)
            {
                // Update the animation index
                PrevAnimIndex = CurrentAnimIndex;

                return true;
            }

            return false;
        }

        [Obsolete]
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBA obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBA Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.Actor.ActorID;
                set => Obj.Actor.ActorID = (GBA_R3_ActorID)value;
            }

            public int DES
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public int ETA
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => Obj.Actor.StateIndex;
                set => Obj.Actor.StateIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.GraphicsData?.States?.Length > 0 ? Obj.GraphicsData?.States?.Length ?? 0 : Obj.GraphicsData?.Graphics.Animations.Count ?? 0;

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