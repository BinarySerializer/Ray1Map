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

        // TODO: Update for PS1
        public override string DebugText => Settings.LoadFromMemory 
            ? $"Pos: {EventData.XPosition}, {EventData.YPosition}{Environment.NewLine}" +
              $"RuntimePos: {EventData.RuntimeXPosition}, {EventData.RuntimeYPosition}{Environment.NewLine}" +
              $"Layer: {EventData.Layer}{Environment.NewLine}" +
              $"RuntimeLayer: {EventData.RuntimeLayer}{Environment.NewLine}" +
              $"{Environment.NewLine}" +
              $"Unk_24: {EventData.Unk_24}{Environment.NewLine}" +
              $"Unk_28: {EventData.Unk_28}{Environment.NewLine}" +
              $"Unk_32: {EventData.Unk_32}{Environment.NewLine}" +
              $"Unk_36: {EventData.Unk_36}{Environment.NewLine}" +
              $"{Environment.NewLine}" +
              $"Unk_48: {EventData.Unk_48}{Environment.NewLine}" +
              $"Unk_54: {EventData.Unk_54}{Environment.NewLine}" +
              $"Unk_56: {EventData.Unk_56}{Environment.NewLine}" +
              $"Unk_58: {EventData.Unk_58}{Environment.NewLine}" +
              $"{Environment.NewLine}" +
              $"Unk_64: {EventData.Unk_64}{Environment.NewLine}" +
              $"Unk_66: {EventData.Unk_66}{Environment.NewLine}" +
              $"{Environment.NewLine}" +
              $"Unk_74: {EventData.Unk_74}{Environment.NewLine}" +
              $"Unk_76: {EventData.Unk_76}{Environment.NewLine}" +
              $"Unk_78: {EventData.Unk_78}{Environment.NewLine}" +
              $"Unk_80: {EventData.Unk_80}{Environment.NewLine}" +
              $"Unk_82: {EventData.Unk_82}{Environment.NewLine}" +
              $"Unk_84: {EventData.Unk_84}{Environment.NewLine}" +
              $"Unk_86: {EventData.Unk_86}{Environment.NewLine}" +
              $"Unk_88: {EventData.Unk_88}{Environment.NewLine}" +
              $"Unk_90: {EventData.Unk_90}{Environment.NewLine}" +
              $"Runtime_ZdcIndex: {EventData.Runtime_ZdcIndex}{Environment.NewLine}" +
              $"Unk_94: {EventData.Unk_94}{Environment.NewLine}" +
              $"{Environment.NewLine}" +
              $"Flags: {Convert.ToString((byte)EventData.PC_Flags, 2).PadLeft(8, '0')}{Environment.NewLine}" 
            : $"Flags: {String.Join(", ", EventData.PC_Flags.GetFlags())}";

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);
        public override bool IsAlways => TypeInfo?.Flag == ObjTypeFlag.Always && !(ObjManager.Context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 && EventData.Type == R1_EventType.TYPE_DARK2_PINK_FLY);
        public override bool IsEditor => TypeInfo?.Flag == ObjTypeFlag.Editor;

        // TODO: Check PS1 flags
        // Unk_28 is also some active flag, but it's 0 for Rayman
        public override bool IsActive => EventData.PC_Flags.HasFlag(R1_EventData.PC_EventFlags.SwitchedOn) && EventData.Unk_36 == 1;

        public override string DisplayName => $"{EventData.Type}";

        // TODO: Fix
        public override int? GetLayer(int index) => -(index + (EventData.RuntimeLayer * 512));

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

        public override Unity_ObjAnimation CurrentAnimation => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Animations.ElementAtOrDefault(AnimationIndex);
        public override byte AnimationFrame
        {
            get => EventData.RuntimeCurrentAnimFrame;
            set => EventData.RuntimeCurrentAnimFrame = value;
        }

        public override byte AnimationIndex
        {
            get => EventData.RuntimeCurrentAnimIndex;
            set => EventData.RuntimeCurrentAnimIndex = value;
        }

        public override byte AnimSpeed => (byte)(EventData.Type.IsHPFrame() ? 0 : State?.AnimationSpeed ?? 0);

        public override byte GetAnimIndex => State?.AnimationIndex ?? 0;
        public override IList<Sprite> Sprites => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.OffsetBX, -EventData.OffsetBY);

        protected override bool ShouldUpdateFrame()
        {
            // Set frame based on hit points for special events
            if (EventData.Type.IsHPFrame())
            {
                EventData.RuntimeCurrentAnimFrame = EventData.HitPoints;
                AnimationFrameFloat = EventData.HitPoints;
                return false;
            }
            else if (EventData.Type.UsesEditorFrame())
            {
                AnimationFrameFloat = EventData.RuntimeCurrentAnimFrame;
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override void OnFinishedAnimation()
        {
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

        public override void ResetFrame()
        {
            if (Settings.LoadFromMemory || EventData.Type.UsesEditorFrame()) 
                return;

            AnimationFrame = 0;
            AnimationFrameFloat = 0;
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