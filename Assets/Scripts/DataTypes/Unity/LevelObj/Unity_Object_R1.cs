using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_R1 : Unity_Object
    {
        public Unity_Object_R1(R1_EventData eventData, Unity_ObjectManager_R1 objManager)
        {
            // Set properties
            EventData = eventData;
            ObjManager = objManager;
            TypeInfo = EventData.Type.GetAttribute<ObjTypeInfoAttribute>();

            // Set editor states
            EventData.RuntimeEtat = EventData.Etat;
            EventData.RuntimeSubEtat = EventData.SubEtat;
            EventData.RuntimeLayer = EventData.Layer;
            EventData.RuntimeXPosition = (ushort)EventData.XPosition;
            EventData.RuntimeYPosition = (ushort)EventData.YPosition;
            EventData.RuntimeCurrentAnimIndex = 0;
            EventData.RuntimeHitPoints = EventData.HitPoints;
        }

        public R1_EventData EventData { get; }

        public Unity_ObjectManager_R1 ObjManager { get; }

        public R1_EventState State => ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data?.ElementAtOrDefault(EventData.RuntimeEtat)?.ElementAtOrDefault(EventData.RuntimeSubEtat);

        public int DESIndex
        {
            get => (ObjManager.UsesPointers ? ObjManager.DES.FindItemIndex(x => x.Pointer == EventData.ImageDescriptorsPointer) : (int)EventData.PC_ImageDescriptorsIndex);
            set
            {
                if (ObjManager.UsesPointers)
                    EventData.ImageDescriptorsPointer = ObjManager.DES[value].Pointer;
                else
                    EventData.PC_ImageDescriptorsIndex = EventData.PC_AnimationDescriptorsIndex = EventData.PC_ImageBufferIndex = (uint)value;
            }
        }

        public int ETAIndex
        {
            get => (ObjManager.UsesPointers ? ObjManager.ETA.FindItemIndex(x => x.Pointer == EventData.ETAPointer) : (int)EventData.PC_ETAIndex);
            set
            {
                if (ObjManager.UsesPointers)
                    EventData.ETAPointer = ObjManager.ETA[value].Pointer;
                else
                    EventData.PC_ETAIndex = (uint)value;
            }
        }

        protected ObjTypeInfoAttribute TypeInfo { get; set; }

        public override short XPosition
        {
            get => (short)EventData.XPosition;
            set => EventData.XPosition = value;
        }
        public override short YPosition
        {
            get => (short)EventData.YPosition;
            set => EventData.YPosition = value;
        }

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);
        public override bool IsAlways => TypeInfo?.Flag == ObjTypeFlag.Always && !(ObjManager.Context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 && EventData.Type == R1_EventType.TYPE_DARK2_PINK_FLY);
        public override bool IsEditor => TypeInfo?.Flag == ObjTypeFlag.Editor;

        // TODO: Check PS1 flags
        // Unk_28 is also some active flag, but it's 0 for Rayman
        public override bool IsActive => EventData.PC_Flags.HasFlag(R1_EventData.PC_EventFlags.SwitchedOn) && EventData.Unk_36 == 1;

        public override string DisplayName => $"{EventData.Type}";

        // TODO: Fix
        public override int Layer => Settings.LoadFromMemory ? -(EventData.EventIndex + (256 * EventData.RuntimeLayer)) : 0;

        public override bool FlipHorizontally
        {
            get
            {
                // If loading from memory, check runtime flags
                if (Settings.LoadFromMemory)
                {
                    if (EventData.PC_Flags.HasFlag(R1_EventData.PC_EventFlags.DetectZone))
                        return true;

                    // TODO: Check PS1 flags

                    return false;
                }

                // Check if it's the pin event and if the hp flag is set
                if (EventData.Type == R1_EventType.TYPE_PUNAISE3 && EventData.HitPoints == 1)
                    return true;

                // If the first command changes its direction to right, flip the event (a bit hacky, but works for trumpets etc.)
                if (EventData.Commands?.Commands?.FirstOrDefault()?.Command == R1_EventCommandType.GO_RIGHT)
                    return true;

                return false;
            }
        }

        public override Unity_ObjAnimation CurrentAnimation => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Animations.ElementAtOrDefault(EventData.RuntimeCurrentAnimIndex);
        public override byte CurrentAnimationFrame
        {
            get => EventData.RuntimeCurrentAnimFrame;
            set
            {
                EventData.RuntimeCurrentAnimFrame = value;
                CurrentAnimationFrameFloat = value;
            }
        }

        public int AnimSpeed => EventData.Type.IsHPFrame() ? 0 : State?.AnimationSpeed ?? 0;

        public override IList<Sprite> Sprites => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.OffsetBX, -EventData.OffsetBY);

        public override void UpdateFrame()
        {
            // Set frame based on hit points for special events
            if (EventData.Type.IsHPFrame())
            {
                EventData.RuntimeCurrentAnimFrame = EventData.HitPoints;
                CurrentAnimationFrameFloat = EventData.HitPoints;
            }
            else if (EventData.Type.UsesEditorFrame())
            {
                CurrentAnimationFrameFloat = EventData.RuntimeCurrentAnimFrame;
            }
            else
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

        public override void ResetFrame()
        {
            if (Settings.LoadFromMemory || EventData.Type.UsesEditorFrame()) 
                return;

            CurrentAnimationFrame = 0;
        }

        [Obsolete]
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_R1 obj)
            {
                Obj = obj;
            }

            private Unity_Object_R1 Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.EventData.Type;
                set => Obj.EventData.Type = (R1_EventType)value;
            }

            public int DES
            {
                get => Obj.DESIndex;
                set => Obj.DESIndex = value;
            }

            public int ETA
            {
                get => Obj.ETAIndex;
                set => Obj.ETAIndex = value;
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

            public int EtatLength => Obj.ObjManager.ETA.ElementAtOrDefault(Obj.ETAIndex)?.Data.Length ?? 0;
            public int SubEtatLength => Obj.ObjManager.ETA.ElementAtOrDefault(Obj.ETAIndex)?.Data.ElementAtOrDefault(Obj.EventData.Etat)?.Length ?? 0;

            public byte OffsetBX
            {
                get => Obj.EventData.OffsetBX;
                set => Obj.EventData.OffsetBX = value;
            }

            public byte OffsetBY
            {
                get => Obj.EventData.OffsetBY;
                set => Obj.EventData.OffsetBY = value;
            }

            public byte OffsetHY
            {
                get => Obj.EventData.OffsetHY;
                set => Obj.EventData.OffsetHY = value;
            }

            public byte FollowSprite
            {
                get => Obj.EventData.FollowSprite;
                set => Obj.EventData.FollowSprite = value;
            }

            public uint HitPoints
            {
                get => Obj.EventData.ActualHitPoints;
                set
                {
                    Obj.EventData.ActualHitPoints = value;
                    Obj.EventData.RuntimeHitPoints = (byte)(value % 256);
                }
            }

            public byte HitSprite
            {
                get => Obj.EventData.HitSprite;
                set => Obj.EventData.HitSprite = value;
            }

            public bool FollowEnabled
            {
                get => Obj.EventData.GetFollowEnabled(Obj.ObjManager.Context.Settings);
                set => Obj.EventData.SetFollowEnabled(Obj.ObjManager.Context.Settings, value);
            }
        }
    }
}