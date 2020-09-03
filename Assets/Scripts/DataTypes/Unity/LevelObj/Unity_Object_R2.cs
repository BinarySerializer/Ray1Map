using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_R2 : Unity_Object
    {
        public Unity_Object_R2(R1_R2EventData eventData, Unity_ObjectManager_R2 objManager)
        {
            // Set properties
            EventData = eventData;
            ObjManager = objManager;
            TypeInfo = EventData.EventType.GetAttribute<ObjTypeInfoAttribute>();

            // Set editor states
            EventData.RuntimeEtat = EventData.Etat;
            EventData.RuntimeSubEtat = EventData.SubEtat;
            //EventData.RuntimeLayer = EventData.Layer;
            //EventData.RuntimeXPosition = (ushort)EventData.XPosition;
            //EventData.RuntimeYPosition = (ushort)EventData.YPosition;
            EventData.RuntimeCurrentAnimIndex = 0;
        }

        public R1_R2EventData EventData { get; }

        public Unity_ObjectManager_R2 ObjManager { get; }

        public R1_EventState State => AnimGroup?.ETA?.ElementAtOrDefault(EventData.RuntimeEtat)?.ElementAtOrDefault(EventData.RuntimeSubEtat);

        public Unity_ObjectManager_R2.AnimGroup AnimGroup => ObjManager.AnimGroups.ElementAtOrDefault(AnimGroupIndex);

        public int AnimGroupIndex
        {
            get => ObjManager.AnimGroups.FindItemIndex(x => x.Pointer == EventData.AnimGroupPointer);
            set => EventData.AnimGroupPointer = ObjManager.AnimGroups[value].Pointer;
        }

        protected ObjTypeInfoAttribute TypeInfo { get; set; }

        public override short XPosition
        {
            get => EventData.XPosition;
            set => EventData.XPosition = value;
        }
        public override short YPosition
        {
            get => EventData.YPosition;
            set => EventData.YPosition = value;
        }

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool IsAlwaysEvent { get; set; }
        public override bool IsAlways => IsAlwaysEvent;

        public override string DisplayName => $"{EventData.EventType}";
        // TODO: Fix
        public override int? GetLayer(int index) => -(index + (EventData.Layer * 512));
        public override int? MapLayer => EventData.MapLayer - 1;
        public override float Scale => MapLayer == 1 ? 0.5f : 1;
        public override bool FlipHorizontally => EventData.IsFlippedHorizontally;

        public override Unity_ObjAnimation CurrentAnimation => AnimGroup?.DES?.Animations.ElementAtOrDefault(EventData.RuntimeCurrentAnimIndex);
        public override byte CurrentAnimationFrame
        {
            get => EventData.RuntimeCurrentAnimFrame;
            set
            {
                EventData.RuntimeCurrentAnimFrame = value;
                CurrentAnimationFrameFloat = value;
            }
        }

        public int AnimSpeed => State?.AnimationSpeed ?? 0;

        public override IList<Sprite> Sprites => AnimGroup?.DES?.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.CollisionData.OffsetBX, -EventData.CollisionData.OffsetBY);

        public override void UpdateFrame()
        {
            // Increment frame if animating
            if (Settings.AnimateSprites && AnimSpeed > 0)
                CurrentAnimationFrameFloat += (60f / AnimSpeed) * Time.deltaTime;

            // Update the frame
            EventData.RuntimeCurrentAnimFrame = (byte)Mathf.FloorToInt(CurrentAnimationFrameFloat);

            // Loop back to first frame
            if (EventData.RuntimeCurrentAnimFrame >= CurrentAnimation.Frames.Length)
            {
                EventData.RuntimeCurrentAnimFrame = 0;
                CurrentAnimationFrameFloat = 0;

                if (Settings.StateSwitchingMode != StateSwitchingMode.None)
                {
                    // Get the current state
                    var state = State;

                    // Check if we've reached the end of the linking chain and we're looping
                    if (Settings.StateSwitchingMode == StateSwitchingMode.Loop && EventData.RuntimeEtat == state.LinkedEtat && EventData.RuntimeSubEtat == state.LinkedSubEtat)
                    {
                        // Reset the state
                        EventData.RuntimeEtat = EventData.Etat;
                        EventData.RuntimeSubEtat = EventData.SubEtat;
                    }
                    else
                    {
                        // Update state values to the linked one
                        EventData.RuntimeEtat = state.LinkedEtat;
                        EventData.RuntimeSubEtat = state.LinkedSubEtat;
                    }
                }
            }
        }

        public override bool ShouldUpdateAnimation()
        {
            // Update the animation index if not loading from memory
            if (!Settings.LoadFromMemory)
                EventData.RuntimeCurrentAnimIndex = State?.AnimationIndex ?? 0;

            // Check if the animation has changed
            if (PrevAnimIndex != EventData.RuntimeCurrentAnimIndex)
            {
                // Update the animation index
                PrevAnimIndex = EventData.RuntimeCurrentAnimIndex;

                return true;
            }

            return false;
        }

        [Obsolete]
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_R2 obj)
            {
                Obj = obj;
            }

            private Unity_Object_R2 Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.EventData.EventType;
                set => Obj.EventData.EventType = (R1_R2EventType)value;
            }

            public int DES
            {
                get => Obj.AnimGroupIndex;
                set => Obj.AnimGroupIndex = value;
            }

            public int ETA
            {
                get => Obj.AnimGroupIndex;
                set => Obj.AnimGroupIndex = value;
            }

            public byte Etat
            {
                get => Obj.EventData.Etat;
                set => Obj.EventData.Etat = Obj.EventData.RuntimeEtat = value;
            }

            public byte SubEtat
            {
                get => Obj.EventData.SubEtat;
                set => Obj.EventData.SubEtat = Obj.EventData.RuntimeSubEtat = value;
            }

            public int EtatLength => Obj.AnimGroup?.ETA?.Length ?? 0;
            public int SubEtatLength => Obj.AnimGroup?.ETA.ElementAtOrDefault(Obj.EventData.Etat)?.Length ?? 0;

            public byte OffsetBX
            {
                get => Obj.EventData.CollisionData.OffsetBX;
                set => Obj.EventData.CollisionData.OffsetBX = value;
            }

            public byte OffsetBY
            {
                get => Obj.EventData.CollisionData.OffsetBY;
                set => Obj.EventData.CollisionData.OffsetBY = value;
            }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
    }
}