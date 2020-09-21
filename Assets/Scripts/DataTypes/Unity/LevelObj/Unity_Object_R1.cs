using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_R1 : Unity_Object
    {
        public Unity_Object_R1(R1_EventData eventData, Unity_ObjectManager_R1 objManager, int? ETAIndex = null)
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

        public R1_EventState State => ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data?.ElementAtOrDefault(EventData.RuntimeEtat)?.ElementAtOrDefault(EventData.RuntimeSubEtat);

        public int DESIndex
        {
            get => (ObjManager.UsesPointers ? ObjManager.DES.FindItemIndex(x => x.Pointer == EventData.ImageDescriptorsPointer) : (int)EventData.PC_ImageDescriptorsIndex);
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
                    EventData.Etat = EventData.RuntimeEtat = 0;
                    EventData.SubEtat = EventData.RuntimeSubEtat = 0;
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
              $"Runtime_ZdcIndex.ZDCCount: {EventData.Runtime_TypeZDC?.ZDCCount}{Environment.NewLine}" +
              $"Runtime_ZdcIndex.ZDCIndex: {EventData.Runtime_TypeZDC?.ZDCIndex}{Environment.NewLine}" +
              $"State.SoundIndex: {State.SoundIndex}{Environment.NewLine}" +
              $"State.ZDCData: {State.ZDCFlags}{Environment.NewLine}" +
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
                    
                    Unity_ObjAnimationPart p = CurrentAnimation?.Frames[AnimationFrame].SpriteLayers.ElementAtOrDefault(zdc.LayerIndex);
                    
                    int addX = 0, addY = 0;
                    
                    if (p != null) {
                        /*int w = 0, h = 0;
                            if ((p.IsFlippedHorizontally || p.IsFlippedVertically) && p.ImageIndex < Sprites.Count) {
                                var spr = Sprites[p.ImageIndex];
                                w = spr?.texture?.width ?? 0;
                                h = spr?.texture?.height ?? 0;
                            }*/

                        addX = p.XPosition;// + (p.IsFlippedHorizontally ? w - zdc.Width : 0);
                        addY = p.YPosition;// - (p.IsFlippedVertically ? h - zdc.Height : 0);
                        var imgDescr = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.ImageDescriptors.ElementAtOrDefault(p.ImageIndex);

                        if (imgDescr != null) {
                            addX += imgDescr.HitBoxOffsetX;
                            addY += imgDescr.HitBoxOffsetY;
                        }
                    }
                    yield return new Unity_ObjAnimationCollisionPart {
                        XPosition = zdc.XPosition + addX,
                        YPosition = zdc.YPosition + addY,
                        Width = zdc.Width,
                        Height = zdc.Height,
                        Type = colType
                    };
                }
            }
            else if (EventData.HitSprite < 253)
            {
                var animLayer = CurrentAnimation?.Frames[AnimationFrame].SpriteLayers.ElementAtOrDefault(EventData.HitSprite);
                var imgDescr = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.ImageDescriptors.ElementAtOrDefault(animLayer?.ImageIndex ?? -1);

                if (imgDescr != null)
                {
                    yield return new Unity_ObjAnimationCollisionPart()
                    {
                        XPosition = animLayer?.XPosition ?? 0,
                        YPosition = animLayer?.YPosition ?? 0,
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

        public override Unity_ObjAnimation CurrentAnimation => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Animations.ElementAtOrDefault(AnimationIndex);
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

        public override byte GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex ?? 0;
        public override IList<Sprite> Sprites => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.OffsetBX, -EventData.OffsetBY);

		public override string[] UIStateNames {
            get {
                List<string> stateNames = new List<string>();
                HashSet<int> usedAnims = new HashSet<int>();
                var eta = ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data;
                if (eta != null) {
                    for (int i = 0; i < eta.Length; i++) {
                        for (int j = 0; j < eta[i].Length; j++) {
                            usedAnims.Add(eta[i][j].AnimationIndex);
                            stateNames.Add($"State {i}-{j}");
                        }
                    }
                }
                var anims = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Animations;
                if (anims != null) {
                    for (int i = 0; i < anims.Count; i++) {
                        if (usedAnims.Contains(i)) continue;
                        stateNames.Add("(Unused) Animation " + i);
                    }
                }
                return stateNames.ToArray();
            }
        }

		public override int CurrentUIState {
            get {
                var eta = ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data;
                if (OverrideAnimIndex.HasValue) {
                    int currentState = eta?.Sum(e => e.Length) ?? 0;
                    HashSet<int> usedAnims = new HashSet<int>();
                    if (eta != null) {
                        for (int i = 0; i < eta.Length; i++) {
                            for (int j = 0; j < eta[i].Length; j++) {
                                usedAnims.Add(eta[i][j].AnimationIndex);
                            }
                        }
                    }
                    var anims = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Animations;
                    if (anims != null) {
                        for (int i = 0; i < anims.Count; i++) {
                            if (usedAnims.Contains(i)) continue;
                            if (i == OverrideAnimIndex) {
                                return currentState;
                            } else if (i > OverrideAnimIndex) {
                                return 0;
                            }
                            currentState++;
                        }
                    }
                    return 0;
                } else {
                    int stateCount = 0;
                    if (eta != null) {
                        for (int i = 0; i < eta.Length; i++) {
                            if (EventData.Etat == i) {
                                if (EventData.SubEtat < eta[i].Length) {
                                    return stateCount + EventData.SubEtat;
                                } else return 0;
                            }
                            stateCount += eta[i].Length;
                        }
                    }
                    return 0;
                }
            }
            set {
                if (value != CurrentUIState) {
                    HashSet<int> usedAnims = new HashSet<int>();
                    var eta = ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data;
                    int stateCount = 0;
                    if (eta != null) {
                        for (int i = 0; i < eta.Length; i++) {
                            for (int j = 0; j < eta[i].Length; j++) {
                                if (value == stateCount) {
                                    EventData.Etat = EventData.RuntimeEtat = (byte)i;
                                    EventData.SubEtat = EventData.RuntimeSubEtat = (byte)j;
                                    OverrideAnimIndex = null;
                                    return;
                                }
                                usedAnims.Add(eta[i][j].AnimationIndex);
                                stateCount++;
                            }
                        }
                    }
                    var anims = ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Animations;
                    if (anims != null) {
                        for (int i = 0; i < anims.Count; i++) {
                            if (usedAnims.Contains(i)) continue;
                            if (value == stateCount) {
                                OverrideAnimIndex = (byte)i;
                                return;
                            }
                            stateCount++;
                        }
                    }
                }
            }
        }

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