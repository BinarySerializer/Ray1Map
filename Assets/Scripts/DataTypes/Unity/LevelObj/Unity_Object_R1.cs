using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    // TODO: On save we have to set (Sub)Etat to Runtime(Sub)Etat and possibly other runtime values as well

    public class Unity_Object_R1 : Unity_Object
    {
        public Unity_Object_R1(R1_EventData eventData, Unity_ObjectManager_R1 objManager, int? ETAIndex = null)
        {
            // Set properties
            EventData = eventData;
            ObjManager = objManager;
            TypeInfo = EventData.Type.GetAttribute<ObjTypeInfoAttribute>();

            // Set editor states
            EventData.InitialEtat = EventData.Etat;
            EventData.InitialSubEtat = EventData.SubEtat;
            EventData.RuntimeLayer = EventData.Layer;
            EventData.InitialXPosition = (short)EventData.XPosition;
            EventData.InitialYPosition = (short)EventData.YPosition;
            EventData.RuntimeCurrentAnimIndex = 0;
            EventData.InitialHitPoints = EventData.HitPoints;
            UpdateZDC();

            // Find matching name from event sheet
            SecondaryName = ObjManager.FindMatchingEventInfo(EventData)?.Name;

            if (ETAIndex.HasValue) {
                if (ObjManager.UsesPointers)
                    EventData.ETAPointer = ObjManager.ETA[ETAIndex.Value].Pointer;
                else
                    EventData.PC_ETAIndex = (uint)ETAIndex.Value;
            }
        }

        public R1_EventData EventData { get; }

        public Unity_ObjectManager_R1 ObjManager { get; }

        public R1_EventState State => ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data?.ElementAtOrDefault(EventData.Etat)?.ElementAtOrDefault(EventData.SubEtat);

        public int DESIndex
        {
            get => (ObjManager.UsesPointers ? ObjManager.DES.FindItemIndex(x => x.Pointer.AbsoluteOffset == EventData.ImageDescriptorsPointer.AbsoluteOffset) : (int)EventData.PC_ImageDescriptorsIndex);
            set {
                if (value != DESIndex) {
                    OverrideAnimIndex = null;

                    if (ObjManager.UsesPointers)
                        EventData.ImageDescriptorsPointer = ObjManager.DES[value].Pointer;
                    else
                        EventData.PC_ImageDescriptorsIndex = EventData.PC_AnimationDescriptorsIndex = EventData.PC_ImageBufferIndex = (uint)value;
                }
            }
        }

        public int ETAIndex
        {
            get => (ObjManager.UsesPointers ? ObjManager.ETA.FindItemIndex(x => x.Pointer == EventData.ETAPointer) : (int)EventData.PC_ETAIndex);
            set {
                if (value != ETAIndex) {
                    EventData.Etat = EventData.InitialEtat = 0;
                    EventData.SubEtat = EventData.InitialSubEtat = 0;
                    OverrideAnimIndex = null;

                    if (ObjManager.UsesPointers)
                        EventData.ETAPointer = ObjManager.ETA[value].Pointer;
                    else
                        EventData.PC_ETAIndex = (uint)value;
                }
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
        public override string DebugText => 
              $"RuntimePos: {EventData.InitialXPosition}, {EventData.InitialYPosition}{Environment.NewLine}" +
              $"Layer: {EventData.Layer}{Environment.NewLine}" +
              $"RuntimeLayer: {EventData.RuntimeLayer}{Environment.NewLine}" +
              $"{Environment.NewLine}" +
              $"Etat: {EventData.Etat}{Environment.NewLine}" +
              $"Etat: {EventData.SubEtat}{Environment.NewLine}" +
              $"RuntimeEtat: {EventData.InitialEtat}{Environment.NewLine}" +
              $"RuntimeSubEtat: {EventData.InitialSubEtat}{Environment.NewLine}" +
              //$"{Environment.NewLine}" +
              //$"Unk_24: {EventData.Unk_24}{Environment.NewLine}" +
              //$"Unk_28: {EventData.Unk_28}{Environment.NewLine}" +
              //$"Unk_32: {EventData.Unk_32}{Environment.NewLine}" +
              //$"Unk_36: {EventData.Unk_36}{Environment.NewLine}" +
              //$"{Environment.NewLine}" +
              //$"Unk_48: {EventData.Unk_48}{Environment.NewLine}" +
              //$"Unk_54: {EventData.Unk_54}{Environment.NewLine}" +
              //$"Unk_56: {EventData.Unk_56}{Environment.NewLine}" +
              //$"Unk_58: {EventData.Unk_58}{Environment.NewLine}" +
              //$"{Environment.NewLine}" +
              //$"Unk_64: {EventData.Unk_64}{Environment.NewLine}" +
              //$"Unk_66: {EventData.Unk_66}{Environment.NewLine}" +
              //$"{Environment.NewLine}" +
              //$"Unk_74: {EventData.Unk_74}{Environment.NewLine}" +
              //$"Unk_76: {EventData.Unk_76}{Environment.NewLine}" +
              //$"Unk_78: {EventData.Unk_78}{Environment.NewLine}" +
              //$"Unk_80: {EventData.Unk_80}{Environment.NewLine}" +
              //$"Unk_82: {EventData.Unk_82}{Environment.NewLine}" +
              //$"Unk_84: {EventData.Unk_84}{Environment.NewLine}" +
              //$"Unk_86: {EventData.Unk_86}{Environment.NewLine}" +
              //$"Unk_88: {EventData.Unk_88}{Environment.NewLine}" +
              //$"Unk_90: {EventData.Unk_90}{Environment.NewLine}" +
              $"Runtime_ZdcIndex.ZDCCount: {EventData.Runtime_TypeZDC?.ZDCCount}{Environment.NewLine}" +
              $"Runtime_ZdcIndex.ZDCIndex: {EventData.Runtime_TypeZDC?.ZDCIndex}{Environment.NewLine}" +
              $"Unk_94: {EventData.Unk_94}{Environment.NewLine}" +
              $"{Environment.NewLine}" +
              $"Flags: {EventData.PC_Flags}{Environment.NewLine}";

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);
        public override bool IsAlways => TypeInfo?.Flag == ObjTypeFlag.Always && !(ObjManager.Context.Settings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 && EventData.Type == R1_EventType.TYPE_DARK2_PINK_FLY);
        public override bool IsEditor => TypeInfo?.Flag == ObjTypeFlag.Editor;

        // TODO: Check PS1 flags
        // Unk_28 is also some active flag, but it's 0 for Rayman
        public override bool IsActive => EventData.PC_Flags.HasFlag(R1_EventData.PC_EventFlags.SwitchedOn) && EventData.Unk_36 == 1;
        public override string PrimaryName => (ushort)EventData.Type < 262 ? $"{EventData.Type.ToString().Replace("TYPE_","")}" : $"TYPE_{(ushort)EventData.Type}";
        public override string SecondaryName { get; }

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

        protected IEnumerable<Unity_ObjAnimationCollisionPart> GetObjZDC()
        {
            var engineVersion = ObjManager.Context.Settings.EngineVersion;

            // Ignore earlier games
            if (engineVersion == EngineVersion.R1_PS1_JP ||
                engineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                engineVersion == EngineVersion.R1_PS1_JPDemoVol6 ||
                engineVersion == EngineVersion.R1_Saturn)
                yield break;

            // Make sure the current state and type supports collision
            if (State == null || State.ZDCFlags == 0 || (ObjManager.EventFlags != null && ObjManager.EventFlags.ElementAtOrDefault((ushort)EventData.Type).HasFlag(R1_EventFlags.NoCollision)))
                yield break;

            // Attempt to set the collision type
            var colType = (ObjManager.EventFlags != null && ObjManager.EventFlags.ElementAtOrDefault((ushort)EventData.Type).HasFlag(R1_EventFlags.HurtsRayman)) 
                ? Unity_ObjAnimationCollisionPart.CollisionType.AttackBox 
                : State.ZDCFlags.HasFlag(R1_EventState.R1_ZDCFlags.DetectFist) 
                    ? Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox 
                    : Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox;

            if (EventData.HitSprite > 253)
            {
                var typeZdc = EventData.Runtime_TypeZDC;

                for (int i = 0; i < (typeZdc?.ZDCCount ?? 0); i++)
                {
                    var zdc = ObjManager.ZDCData?.ElementAtOrDefault(typeZdc.ZDCIndex + i);

                    if (zdc == null) 
                        continue;

                    // Relative to the event origin
                    if (zdc.LayerIndex == 0xFF)
                    {
                        yield return new Unity_ObjAnimationCollisionPart
                        {
                            XPosition = zdc.XPosition,
                            YPosition = zdc.YPosition,
                            Width = zdc.Width,
                            Height = zdc.Height,
                            Type = colType
                        };
                    }
                    else
                    {
                        Unity_ObjAnimationPart p = CurrentAnimation?.Frames[AnimationFrame].SpriteLayers.ElementAtOrDefault(zdc.LayerIndex);

                        if (p == null)
                            continue;

                        /*int w = 0, h = 0;
                            if ((p.IsFlippedHorizontally || p.IsFlippedVertically) && p.ImageIndex < Sprites.Count) {
                                var spr = Sprites[p.ImageIndex];
                                w = spr?.texture?.width ?? 0;
                                h = spr?.texture?.height ?? 0;
                            }*/

                        var addX = p.XPosition;
                        var addY = p.YPosition;

                        var imgDescr = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.ImageDescriptors.ElementAtOrDefault(p.ImageIndex);

                        if (imgDescr == null)
                            continue;

                        addX += imgDescr.HitBoxOffsetX;
                        addY += imgDescr.HitBoxOffsetY;

                        if (imgDescr.IsDummySprite())
                            continue;

                        yield return new Unity_ObjAnimationCollisionPart
                        {
                            XPosition = zdc.XPosition + addX,
                            YPosition = zdc.YPosition + addY,
                            Width = zdc.Width,
                            Height = zdc.Height,
                            Type = colType
                        };
                    }
                }
            }
            else if (EventData.HitSprite < 253)
            {
                var animLayer = CurrentAnimation?.Frames[AnimationFrame].SpriteLayers.ElementAtOrDefault(EventData.HitSprite);
                var imgDescr = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.ImageDescriptors.ElementAtOrDefault(animLayer?.ImageIndex ?? -1);

                if (imgDescr != null && !imgDescr.IsDummySprite())
                {
                    yield return new Unity_ObjAnimationCollisionPart()
                    {
                        XPosition = (animLayer?.XPosition ?? 0) + (imgDescr.HitBoxOffsetX),
                        YPosition = (animLayer?.YPosition ?? 0) + (imgDescr.HitBoxOffsetY),
                        Width = imgDescr.HitBoxWidth,
                        Height = imgDescr.HitBoxHeight,
                        Type = colType
                    };
                }
            }
            else
            {
                // Do nothing - the game hard-codes these
            }
        }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => GetObjZDC().ToArray();

        public override Unity_ObjAnimation CurrentAnimation => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimationFrame
        {
            get => EventData.RuntimeCurrentAnimFrame;
            set => EventData.RuntimeCurrentAnimFrame = (byte)value;
        }

        public override int? AnimationIndex
        {
            get => EventData.RuntimeCurrentAnimIndex;
            set => EventData.RuntimeCurrentAnimIndex = (byte)(value ?? 0);
        }

        public override int AnimSpeed => (EventData.Type.IsHPFrame() ? 0 : State?.AnimationSpeed ?? 0);

        public override int? GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex ?? 0;
        protected override int GetSpriteID => DESIndex;
        public override IList<Sprite> Sprites => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Sprites;
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
            if (Settings.LoadFromMemory)
                return;

            if (Settings.StateSwitchingMode != StateSwitchingMode.None)
            {
                // Get the current state
                var state = State;

                // Check if we've reached the end of the linking chain and we're looping
                if (Settings.StateSwitchingMode == StateSwitchingMode.Loop && 
                    EventData.Etat == state.LinkedEtat && 
                    EventData.SubEtat == state.LinkedSubEtat)
                {
                    // Reset the state
                    EventData.Etat = EventData.InitialEtat;
                    EventData.SubEtat = EventData.InitialSubEtat;
                }
                else
                {
                    // Update state values to the linked one
                    EventData.Etat = state.LinkedEtat;
                    EventData.SubEtat = state.LinkedSubEtat;
                }
            }
            else
            {
                EventData.Etat = EventData.InitialEtat;
                EventData.SubEtat = EventData.InitialSubEtat;
            }
        }

        public override void ResetFrame()
        {
            if (Settings.LoadFromMemory || EventData.Type.UsesEditorFrame()) 
                return;

            AnimationFrame = 0;
            AnimationFrameFloat = 0;
        }

        protected void UpdateZDC() => EventData.Runtime_TypeZDC = ObjManager.TypeZDC?.ElementAtOrDefault((ushort)EventData.Type) ?? EventData.Runtime_TypeZDC;

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
                set
                {
                    Obj.EventData.Type = (R1_EventType) value;
                    Obj.UpdateZDC();
                }
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
                set => Obj.EventData.Etat = Obj.EventData.InitialEtat = value;
            }

            public byte SubEtat
            {
                get => Obj.EventData.SubEtat;
                set => Obj.EventData.SubEtat = Obj.EventData.InitialSubEtat = value;
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
                    Obj.EventData.InitialHitPoints = (byte)(value % 256);
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

        #region UI States
        protected int UIStates_ETAIndex { get; set; } = -2;
        protected int UIStates_DESIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => DESIndex == UIStates_DESIndex && ETAIndex == UIStates_ETAIndex;

        protected override void RecalculateUIStates() {
            UIStates_DESIndex = DESIndex;
            UIStates_ETAIndex = ETAIndex;
            List<UIState> uiStates = new List<UIState>();
            HashSet<int> usedAnims = new HashSet<int>();
            var eta = ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data;
            if (eta != null) {
                for (byte i = 0; i < eta.Length; i++) {
                    for (byte j = 0; j < eta[i].Length; j++) {
                        usedAnims.Add(eta[i][j].AnimationIndex);
                        uiStates.Add(new R1_UIState($"State {i}-{j}", i, j));
                    }
                }
            }
            var anims = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Animations;
            if (anims != null) {
                for (int i = 0; i < anims.Count; i++) {
                    if (usedAnims.Contains(i)) continue;
                    uiStates.Add(new R1_UIState($"Animation {i}", i));
                }
            }
            UIStates = uiStates.ToArray();
        }

        protected class R1_UIState : UIState {
            public R1_UIState(string displayName, byte etat, byte subEtat) : base(displayName) {
                Etat = etat;
                SubEtat = subEtat;
            }
            public R1_UIState(string displayName, int animIndex) : base(displayName, animIndex) {}

            public byte Etat { get; }
            public byte SubEtat { get; }

			public override void Apply(Unity_Object obj) {
                if (IsState) {
                    var r1obj = obj as Unity_Object_R1;
                    r1obj.EventData.Etat = r1obj.EventData.InitialEtat = Etat;
                    r1obj.EventData.SubEtat = r1obj.EventData.InitialSubEtat = SubEtat;
                    obj.OverrideAnimIndex = null;
                } else {
                    obj.OverrideAnimIndex = AnimIndex;
                }
            }

			public override bool IsCurrentState(Unity_Object obj) {

                if (obj.OverrideAnimIndex.HasValue)
                    return !IsState && AnimIndex == obj.OverrideAnimIndex;
                else
                    return IsState
                        && Etat == ((Unity_Object_R1)obj).EventData.InitialEtat
                        && SubEtat == ((Unity_Object_R1)obj).EventData.InitialSubEtat;

            }
        }
        #endregion
    }
}