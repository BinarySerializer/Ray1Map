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
            EventData.InitialEtat = EventData.Etat;
            EventData.InitialSubEtat = EventData.SubEtat;
            EventData.InitialHitPoints = EventData.HitPoints;
            EventData.InitialDisplayPrio = EventData.DisplayPrio;
            EventData.InitialXPosition = EventData.XPosition;
            EventData.InitialYPosition = EventData.YPosition;
            EventData.RuntimeCurrentAnimIndex = 0;
            EventData.RuntimeMapLayer = EventData.MapLayer;

            if (EventData.EventType == R1_R2EventType.Ting)
                EventData.RuntimeCurrentAnimFrame = (byte)(EventData.HitPoints - 1);
        }

        public R1_R2EventData EventData { get; }

        public Unity_ObjectManager_R2 ObjManager { get; }

        public R1_EventState CurrentState => GetState(EventData.Etat, EventData.SubEtat);
        public R1_EventState InitialState => GetState(EventData.InitialEtat, EventData.InitialSubEtat);
        public R1_EventState LinkedState => GetState(CurrentState?.LinkedEtat ?? -1, CurrentState?.LinkedSubEtat ?? -1);

        protected R1_EventState GetState(int etat, int subEtat) => AnimGroup?.ETA?.ElementAtOrDefault(etat)?.ElementAtOrDefault(subEtat);

        public Unity_ObjectManager_R2.AnimGroup AnimGroup => ObjManager.AnimGroups.ElementAtOrDefault(AnimGroupIndex);

        public int AnimGroupIndex
        {
            get => ObjManager.AnimGroupsLookup.TryGetItem(EventData.AnimGroupPointer?.AbsoluteOffset ?? 0, -1);
            set {
                if (value != AnimGroupIndex && EventData.AnimGroupPointer != null) {
                    EventData.Etat = EventData.InitialEtat = 0;
                    EventData.SubEtat = EventData.InitialSubEtat = 0;
                    OverrideAnimIndex = null;
                    EventData.AnimGroupPointer = ObjManager.AnimGroups[value].Pointer;
                }
            }
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

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => EventData;

        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool IsAlwaysEvent { get; set; }
        public override bool IsAlways => IsAlwaysEvent;
        public override bool IsEditor => (AnimGroup?.Animations?.Any() != true && EventData.EventType != R1_R2EventType.Invalid) || EventData.DisplayPrio == 0;
        public override ObjectType Type
        {
            get
            {
                switch (EventData.EventType)
                {
                    case R1_R2EventType.Gendoor:
                    case R1_R2EventType.Trigger:
                        return ObjectType.Trigger;

                    case R1_R2EventType.RaymanPosition:
                        return ObjectType.Waypoint;

                    default:
                        return ObjectType.Object;
                }
            }
        }

        public override bool IsActive => !Settings.LoadFromMemory || (EventData.EventType != R1_R2EventType.Invalid && (EventData.RuntimeFlags1.HasFlag(R1_R2EventData.PS1_R2Demo_EventRuntimeFlags1.SwitchedOn)));
        public override bool CanBeLinkedToGroup => true;
        public override bool CanBeLinked => EventData.ParamsGendoor != null || EventData.ParamsTrigger != null;

        public override IEnumerable<int> Links
        {
            get
            {
                if (EventData.ParamsGendoor != null)
                    foreach (var l in EventData.ParamsGendoor.LinkedObjects)
                        yield return l;

                if (EventData.ParamsTrigger != null)
                    foreach (var l in EventData.ParamsTrigger.LinkedObjects)
                        yield return l;
            }
        }

        public override string PrimaryName => $"TYPE_{(ushort)EventData.EventType}";
        public override string SecondaryName => $"{EventData.EventType}";
        // TODO: Fix
        public override int? GetLayer(int index) => -(index + (EventData.DisplayPrio * 512));

        public override int? MapLayer => EventData.RuntimeMapLayer == R1_R2EventData.ObjMapLayer.Back ? 2: 3;

        public override float Scale => EventData.RuntimeMapLayer == R1_R2EventData.ObjMapLayer.Back ? 0.5f : 1;
        public override bool FlipHorizontally => Settings.LoadFromMemory ? EventData.RuntimeFlipX : EventData.Flags.HasFlag(R1_R2EventData.PS1_R2Demo_EventFlags.FlippedHorizontally);

        protected IEnumerable<Unity_ObjAnimationCollisionPart> GetObjZDC() {
            var zdcEntry = EventData.CollisionData?.ZDC;

            if (zdcEntry == null)
                yield break;

            // Function at 0x800d8264 (BOX_IN_COLL_ZONES)

            for (int i = 0; i < zdcEntry.ZDCCount; i++)
            {
                var zdc = ObjManager.LevData.ZDC?.ElementAtOrDefault(zdcEntry.ZDCIndex + i);
                var zdcTriggerFlags = ObjManager.LevData.ZDCTriggerFlags?.ElementAtOrDefault(zdcEntry.ZDCIndex + i);

                if (zdc == null)
                    continue;

                if (zdc.ZDC_Flags != 0 && (zdc.ZDC_Flags & EventData.ZDCFlags) == 0) 
                    continue;

                var hurtsRay = EventData.CollisionData?.Flags.HasFlag(R1_R2EventCollision.EventFlags.HurtsRayman) == true && 
                               CurrentState?.ZDCFlags.HasFlag(R1_EventState.R1_ZDCFlags.DetectRay) == true && 
                               zdcTriggerFlags?.HasFlag(R1_R2LevDataFile.ZDC_TriggerFlags.Rayman) == true;

                // Attempt to set the collision type
                var colType = hurtsRay 
                    ? Unity_ObjAnimationCollisionPart.CollisionType.AttackBox
                    : zdcTriggerFlags?.HasFlag(R1_R2LevDataFile.ZDC_TriggerFlags.Poing_0) == true || zdcTriggerFlags?.HasFlag(R1_R2LevDataFile.ZDC_TriggerFlags.Poing_1) == true
                        ? Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox
                        : Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox;

                // Relative to the event origin
                if (zdc.LayerIndex == 0x1F)
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
                // Relative to an animation layer
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

                    var img = ObjManager.ImageDescriptors.ElementAtOrDefault(p.ImageIndex);

                    if (img == null)
                        continue;

                    addX += img.HitBoxOffsetX;
                    addY += img.HitBoxOffsetY;

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

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => GetObjZDC().ToArray();

        public override Unity_ObjAnimation CurrentAnimation => AnimGroup?.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimationFrame
        {
            get => Mathf.Clamp(EventData.RuntimeCurrentAnimFrame, 0, CurrentAnimation?.Frames.Length - 1 ?? 0);
            set => EventData.RuntimeCurrentAnimFrame = (byte)value;
        }

        public override int? AnimationIndex
        {
            get => EventData.RuntimeCurrentAnimIndex;
            set => EventData.RuntimeCurrentAnimIndex = (byte)(value ?? 0);
        }

        public override int AnimSpeed => CurrentState?.AnimationSpeed ?? 0;

        public override int? GetAnimIndex => OverrideAnimIndex ?? CurrentState?.AnimationIndex;
        protected override int GetSpriteID => AnimGroupIndex;
        public override IList<Sprite> Sprites => ObjManager.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.CollisionData?.OffsetBX ?? 0, -EventData.CollisionData?.OffsetBY ?? 0);

        protected HashSet<R1_EventState> EncounteredStates { get; } = new HashSet<R1_EventState>(); // Keep track of "encountered" states if we have state switching set to loop to avoid entering an infinite loop
        protected R1_EventState PrevInitialState { get; set; }

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
                    ResetState();
                }
                else
                {
                    // Update state values to the linked one
                    EventData.Etat = state.LinkedEtat;
                    EventData.SubEtat = state.LinkedSubEtat;

                    // For always objects the game sets the last state in the chain to link to -1 to indicate that the object should now be hidden again, but to avoid the object now displaying in the editor we reset it here
                    if (EventData.Etat == 0xFF || EventData.SubEtat == 0xFF)
                        ResetState();
                }
            }
        }

        protected void ResetState()
        {
            // Reset the state
            EventData.Etat = EventData.InitialEtat;
            EventData.SubEtat = EventData.InitialSubEtat;

            // Clear encountered states
            EncounteredStates.Clear();
        }

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
                set => Obj.EventData.Etat = Obj.EventData.InitialEtat = value;
            }

            public byte SubEtat
            {
                get => Obj.EventData.SubEtat;
                set => Obj.EventData.SubEtat = Obj.EventData.InitialSubEtat = value;
            }

            public int EtatLength => Obj.AnimGroup?.ETA?.Length ?? 0;
            public int SubEtatLength => Obj.AnimGroup?.ETA.ElementAtOrDefault(Obj.EventData.Etat)?.Length ?? 0;

            public byte OffsetBX
            {
                get => Obj.EventData.CollisionData?.OffsetBX ?? 0;
                set
                {
                    if (Obj.EventData.CollisionData != null)
                        Obj.EventData.CollisionData.OffsetBX = value;
                }
            }

            public byte OffsetBY
            {
                get => Obj.EventData.CollisionData?.OffsetBY ?? 0;
                set
                {
                    if (Obj.EventData.CollisionData != null)
                        Obj.EventData.CollisionData.OffsetBY = value;
                }
            }

            public byte OffsetHY
            {
                get => Obj.EventData.CollisionData?.OffsetHY ?? 0;
                set
                {
                    if (Obj.EventData.CollisionData != null)
                        Obj.EventData.CollisionData.OffsetHY = value;
                }
            }

            public byte FollowSprite { get; set; }

            public uint HitPoints
            {
                get => Obj.EventData.HitPoints;
                set => Obj.EventData.HitPoints = (byte)value;
            }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }

        #region UI States
        protected int UIStates_AnimGroupIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimGroupIndex == UIStates_AnimGroupIndex;

        protected override void RecalculateUIStates() {
            UIStates_AnimGroupIndex = AnimGroupIndex;
            List<UIState> uiStates = new List<UIState>();
            HashSet<int> usedAnims = new HashSet<int>();
            var eta = AnimGroup?.ETA;
            if (eta != null) {
                for (byte i = 0; i < eta.Length; i++) {
                    for (byte j = 0; j < eta[i].Length; j++) {
                        usedAnims.Add(eta[i][j].AnimationIndex);
                        uiStates.Add(new R2_UIState($"State {i}-{j} (Animation {eta[i][j].AnimationIndex})", i, j));
                    }
                }
            }
            var anims = AnimGroup?.Animations;
            if (anims != null) {
                for (int i = 0; i < anims.Length; i++) {
                    if (usedAnims.Contains(i)) continue;
                    uiStates.Add(new R2_UIState($"Animation {i}", i));
                }
            }

            UIStates = uiStates.ToArray();
        }

        protected class R2_UIState : UIState {
            public R2_UIState(string displayName, byte etat, byte subEtat) : base(displayName) {
                Etat = etat;
                SubEtat = subEtat;
            }
            public R2_UIState(string displayName, int animIndex) : base(displayName, animIndex) {}

            public byte Etat { get; }
            public byte SubEtat { get; }

            public override void Apply(Unity_Object obj) {
                if (IsState) {
                    var r2obj = obj as Unity_Object_R2;
                    r2obj.EventData.Etat = r2obj.EventData.InitialEtat = Etat;
                    r2obj.EventData.SubEtat = r2obj.EventData.InitialSubEtat = SubEtat;
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
                        && Etat == ((Unity_Object_R2)obj).EventData.InitialEtat
                        && SubEtat == ((Unity_Object_R2)obj).EventData.InitialSubEtat;
            }
        }
        #endregion
    }
}