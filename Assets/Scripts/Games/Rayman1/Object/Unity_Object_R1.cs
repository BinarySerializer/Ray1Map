using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
using UnityEngine;
using Sprite = UnityEngine.Sprite;

namespace Ray1Map.Rayman1
{
    public class Unity_Object_R1 : Unity_SpriteObject
    {
        public Unity_Object_R1(ObjData eventData, Unity_ObjectManager_R1 objManager, int? ETAIndex = null, WorldInfo worldInfo = null)
        {
            // Set properties
            EventData = eventData;
            ObjManager = objManager;
            TypeInfo = EventData.Type.GetAttribute<ObjTypeInfoAttribute>();
            WorldInfo = worldInfo;

            // Set editor states
            EventData.InitialMainEtat = EventData.MainEtat;
            EventData.InitialSubEtat = EventData.SubEtat;
            EventData.DisplayPrio = objManager.GetDisplayPrio(EventData.Type, EventData.HitPoints, EventData.DisplayPrio);
            EventData.InitialXPosition = (short)EventData.XPosition;
            EventData.InitialYPosition = (short)EventData.YPosition;
            EventData.AnimationIndex = 0;
            EventData.InitialHitPoints = EventData.HitPoints;
            UpdateZDC();

            // Set random frame
            if (EventData.Type.UsesRandomFrame())
                ForceFrame = (byte)ObjManager.GetNextRandom(CurrentAnimation?.Frames.Length ?? 1);

            // Find matching name from event sheet
            SecondaryName = ObjManager.FindMatchingEventInfo(EventData)?.Name;

            if (ETAIndex.HasValue) {
                if (ObjManager.UsesPointers)
                    EventData.ETAPointer = ObjManager.ETA[ETAIndex.Value].PrimaryPointer;
                else
                    EventData.PCPacked_ETAIndex = (uint)ETAIndex.Value;
            }
        }

        public ObjData EventData { get; }
        public WorldInfo WorldInfo { get; }
        public byte ForceFrame { get; set; }

        public Unity_ObjectManager_R1 ObjManager { get; }

        public ObjState CurrentState => GetState(EventData.MainEtat, EventData.SubEtat);
        public ObjState InitialState => GetState(EventData.InitialMainEtat, EventData.InitialSubEtat);
        public ObjState LinkedState => GetState(CurrentState?.NextMainEtat ?? -1, CurrentState?.NextSubEtat ?? -1);

        protected ObjState GetState(int etat, int subEtat) => ObjManager.ETA.ElementAtOrDefault(ETAIndex)?.Data?.ElementAtOrDefault(etat)?.ElementAtOrDefault(subEtat);

        public int DESIndex
        {
            get => (ObjManager.UsesPointers ? ObjManager.DESLookup.TryGetItem(EventData.SpritesPointer?.AbsoluteOffset ?? 0, -1) : (int)EventData.PCPacked_SpritesIndex);
            set {
                if (value != DESIndex) {
                    OverrideAnimIndex = null;

                    if (ObjManager.UsesPointers)
                    {
                        EventData.SpritesPointer = ObjManager.DES[value].Data.ImageDescriptorPointer;
                        EventData.AnimationsPointer = ObjManager.DES[value].Data.AnimationDescriptorPointer;
                        EventData.ImageBufferPointer = ObjManager.DES[value].Data.ImageBufferPointer;
                    }
                    else
                    {
                        EventData.PCPacked_SpritesIndex = EventData.PCPacked_AnimationsIndex = EventData.PCPacked_ImageBufferIndex = (uint)value;
                    }
                }
            }
        }

        public int ETAIndex
        {
            get => (ObjManager.UsesPointers ? ObjManager.ETALookup.TryGetItem(EventData.ETAPointer?.AbsoluteOffset ?? 0, -1) : (int)EventData.PCPacked_ETAIndex);
            set {
                if (value != ETAIndex) {
                    EventData.MainEtat = EventData.InitialMainEtat = 0;
                    EventData.SubEtat = EventData.InitialSubEtat = 0;
                    OverrideAnimIndex = null;

                    if (ObjManager.UsesPointers)
                        EventData.ETAPointer = ObjManager.ETA[value].PrimaryPointer;
                    else
                        EventData.PCPacked_ETAIndex = (uint)value;
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

        public override string DebugText => String.Empty;

        public override IEnumerable<int> Links
        {
            get
            {
                yield return WorldInfo.UpIndex;
                yield return WorldInfo.DownIndex;
                yield return WorldInfo.LeftIndex;
                yield return WorldInfo.RightIndex;
            }
        }

        public override BinarySerializable SerializableData => EventData;

        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);
        public override bool IsAlways => TypeInfo?.Flag == ObjTypeFlag.Always && !(ObjManager.Context.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 && EventData.Type == ObjType.TYPE_DARK2_PINK_FLY);
        public override bool IsEditor => TypeInfo?.Flag == ObjTypeFlag.Editor;

        public override bool IsActive => EventData.IsActive && EventData.IsAlive;

        public override bool CanBeLinkedToGroup => !(ObjManager.EventFlags?[(int)EventData.Type].NoLink ?? false) && WorldInfo == null && EventData.Type != ObjType.TYPE_RAYMAN;
        public override bool CanBeLinked => WorldInfo != null;

        public override string PrimaryName => (ushort)EventData.Type < 262 ? $"{EventData.Type.ToString().Replace("TYPE_","")}" : $"TYPE_{(ushort)EventData.Type}";
        public override string SecondaryName { get; }

        public override int? GetLayer(int index) => (index + (EventData.DisplayPrio * 1000));

        public override bool FlipHorizontally
        {
            get
            {
                if (Settings.LoadFromMemory)
                    return EventData.FlipX;

                // Check if it's the pin event and if the hp flag is set
                if (EventData.Type == ObjType.TYPE_PUNAISE3 && EventData.HitPoints == 1)
                    return true;

                // If the first command changes its direction to right, flip the event (a bit hacky, but works for trumpets etc.)
                if (EventData.Commands?.Commands?.FirstOrDefault()?.CommandType == CommandType.GO_RIGHT)
                    return true;

                return false;
            }
        }

        protected IEnumerable<Unity_ObjAnimationCollisionPart> GetObjZDC()
        {
            var engineVersion = ObjManager.Context.GetR1Settings().EngineVersion;

            // Ignore earlier games
            if (engineVersion == EngineVersion.R1_PS1_JP ||
                engineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                engineVersion == EngineVersion.R1_PS1_JPDemoVol6 ||
                engineVersion == EngineVersion.R1_Saturn)
                yield break;

            // Make sure the current state and type supports collision
            if (CurrentState == null || (!CurrentState.RayCollision && !CurrentState.FistCollision) || (ObjManager.EventFlags != null && ObjManager.EventFlags.ElementAtOrDefault((ushort)EventData.Type).NoCollision))
                yield break;

            var hurtsRay = ObjManager.EventFlags != null && ObjManager.EventFlags.ElementAtOrDefault((ushort)EventData.Type).HitRay && CurrentState.RayCollision == true;

            // Attempt to set the collision type
            var colType = hurtsRay 
                ? Unity_ObjAnimationCollisionPart.CollisionType.AttackBox 
                : CurrentState.FistCollision 
                    ? Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox 
                    : Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox;

            if (EventData.HitSprite > 253)
            {
                var typeZdc = EventData.TypeZDC;

                for (int i = 0; i < (typeZdc?.Count ?? 0); i++)
                {
                    var zdc = ObjManager.ZDCData?.ElementAtOrDefault(typeZdc.Index + i);

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

                        addX += imgDescr.SpriteXPosition;
                        addY += imgDescr.SpriteYPosition;

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
                        XPosition = (animLayer?.XPosition ?? 0) + (imgDescr.SpriteXPosition),
                        YPosition = (animLayer?.YPosition ?? 0) + (imgDescr.SpriteYPosition),
                        Width = imgDescr.SpriteWidth,
                        Height = imgDescr.SpriteHeight,
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
            get => EventData.AnimationFrame;
            set => EventData.AnimationFrame = (byte)value;
        }

        public override int? AnimationIndex
        {
            get => EventData.AnimationIndex;
            set => EventData.AnimationIndex = (byte)(value ?? 0);
        }

        public override int AnimSpeed => (EventData.Type.IsHPFrame() ? 0 : CurrentState?.AnimationSpeed ?? 0);

        public override int? GetAnimIndex => OverrideAnimIndex ?? CurrentState?.AnimationIndex ?? 0;
        protected override int GetSpriteID => DESIndex;
        public override IList<Sprite> Sprites => ObjManager.DES.ElementAtOrDefault(DESIndex)?.Data?.Graphics?.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.OffsetBX, -EventData.OffsetBY);

		protected override bool ShouldUpdateFrame()
        {
            // Set frame based on hit points for special events
            if (EventData.Type.IsHPFrame())
            {
                EventData.AnimationFrame = EventData.HitPoints;
                AnimationFrameFloat = EventData.HitPoints;
                return false;
            }
            else if (EventData.Type.UsesEditorFrame())
            {
                AnimationFrameFloat = EventData.AnimationFrame;
                return false;
            }
            else if (EventData.Type.UsesRandomFrame() || EventData.Type.UsesFrameFromLinkChain())
            {
                EventData.AnimationFrame = ForceFrame;
                AnimationFrameFloat = ForceFrame;
                return false;
            }
            else
            {
                return true;
            }
        }

        protected HashSet<ObjState> EncounteredStates { get; } = new HashSet<ObjState>(); // Keep track of "encountered" states if we have state switching set to loop to avoid entering an infinite loop
        protected ObjState PrevInitialState { get; set; }

        protected override void OnFinishedAnimation()
        {
            if (Settings.LoadFromMemory)
                return;

            // Check if the state has been modified
            if (PrevInitialState != InitialState)
            {
                PrevInitialState = InitialState;

                // Clear encountered states
                EncounteredStates.Clear();
            }

            if (Settings.StateSwitchingMode != StateSwitchingMode.None)
            {
                // Get the current state
                var state = CurrentState;

                // Add current state to list of encountered states
                EncounteredStates.Add(state);

                // Check if we've reached the end of the linking chain and we're looping
                if (Settings.StateSwitchingMode == StateSwitchingMode.Loop && EncounteredStates.Contains(LinkedState))
                {
                    // Reset the state
                    EventData.MainEtat = EventData.InitialMainEtat;
                    EventData.SubEtat = EventData.InitialSubEtat;

                    // Clear encountered states
                    EncounteredStates.Clear();
                }
                else
                {
                    // Update state values to the linked one
                    EventData.MainEtat = state.NextMainEtat;
                    EventData.SubEtat = state.NextSubEtat;
                }
            }
            else
            {
                EventData.MainEtat = EventData.InitialMainEtat;
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

        protected void UpdateZDC()
        {
            var zdc = ObjManager.TypeZDC?.ElementAtOrDefault((ushort)EventData.Type);

            if (zdc != null)
                EventData.TypeZDC = new ZDCReference()
                {
                    Count = zdc.Count,
                    Index = zdc.Index
                };
        }
        
        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_R1 obj)
            {
                Obj = obj;
            }

            private Unity_Object_R1 Obj { get; }

            public override ushort Type
            {
                get => (ushort)Obj.EventData.Type;
                set
                {
                    Obj.EventData.Type = (ObjType) value;
                    Obj.UpdateZDC();
                }
            }

            public override int DES
            {
                get => Obj.DESIndex;
                set => Obj.DESIndex = value;
            }

            public override int ETA
            {
                get => Obj.ETAIndex;
                set => Obj.ETAIndex = value;
            }

            public override byte Etat
            {
                get => Obj.EventData.MainEtat;
                set => Obj.EventData.MainEtat = Obj.EventData.InitialMainEtat = value;
            }

            public override byte SubEtat
            {
                get => Obj.EventData.SubEtat;
                set => Obj.EventData.SubEtat = Obj.EventData.InitialSubEtat = value;
            }

            public override int EtatLength => Obj.ObjManager.ETA.ElementAtOrDefault(Obj.ETAIndex)?.Data.Length ?? 0;
            public override int SubEtatLength => Obj.ObjManager.ETA.ElementAtOrDefault(Obj.ETAIndex)?.Data.ElementAtOrDefault(Obj.EventData.MainEtat)?.Length ?? 0;

            public override byte OffsetBX
            {
                get => Obj.EventData.OffsetBX;
                set => Obj.EventData.OffsetBX = value;
            }

            public override byte OffsetBY
            {
                get => Obj.EventData.OffsetBY;
                set => Obj.EventData.OffsetBY = value;
            }

            public override byte OffsetHY
            {
                get => Obj.EventData.OffsetHY;
                set => Obj.EventData.OffsetHY = value;
            }

            public override byte FollowSprite
            {
                get => Obj.EventData.FollowSprite;
                set => Obj.EventData.FollowSprite = value;
            }

            public override uint HitPoints
            {
                get => Obj.EventData.ActualHitPoints;
                set
                {
                    Obj.EventData.ActualHitPoints = value;
                    Obj.EventData.InitialHitPoints = (byte)(value % 256);
                }
            }

            public override byte HitSprite
            {
                get => Obj.EventData.HitSprite;
                set => Obj.EventData.HitSprite = value;
            }

            public override bool FollowEnabled
            {
                get => Obj.EventData.FollowEnabled;
                set => Obj.EventData.FollowEnabled = value;
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
                    for (byte j = 0; j < (eta[i]?.Length ?? 0); j++) {
                        if (eta[i][j] == null)
                            continue;
                        
                        usedAnims.Add(eta[i][j].AnimationIndex);
                        uiStates.Add(new R1_UIState($"State {i}-{j} (Animation {eta[i][j].AnimationIndex})", i, j));
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
                    r1obj.EventData.MainEtat = r1obj.EventData.InitialMainEtat = Etat;
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
                        && Etat == ((Unity_Object_R1)obj).EventData.InitialMainEtat
                        && SubEtat == ((Unity_Object_R1)obj).EventData.InitialSubEtat;

            }
        }
        #endregion
    }
}