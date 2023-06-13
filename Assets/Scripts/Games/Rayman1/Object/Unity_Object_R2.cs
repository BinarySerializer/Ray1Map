using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PS1;
using UnityEngine;
using Sprite = UnityEngine.Sprite;

namespace Ray1Map.Rayman1
{
    public class Unity_Object_R2 : Unity_SpriteObject
    {
        public Unity_Object_R2(R2_ObjData eventData, Unity_ObjectManager_R2 objManager)
        {
            // Set properties
            EventData = eventData;
            ObjManager = objManager;
            TypeInfo = EventData.Type.GetAttribute<ObjTypeInfoAttribute>();

            // Set editor states
            EventData.InitialMainEtat = EventData.MainEtat;
            EventData.InitialSubEtat = EventData.SubEtat;
            EventData.InitialHitPoints = EventData.HitPoints;
            EventData.InitialDisplayPrio = EventData.DisplayPrio;
            EventData.InitialXPosition = EventData.XPosition;
            EventData.InitialYPosition = EventData.YPosition;
            EventData.AnimationIndex = 0;
            EventData.MapLayer = EventData.InitialMapLayer; // MapLayer is None in the files

            if (EventData.Type == R2_ObjType.TYPE_WIZ)
                EventData.AnimationFrame = (byte)(EventData.HitPoints - 1);
        }

        public R2_ObjData EventData { get; }

        public Unity_ObjectManager_R2 ObjManager { get; }

        public ObjState CurrentState => GetState(EventData.MainEtat, EventData.SubEtat);
        public ObjState InitialState => GetState(EventData.InitialMainEtat, EventData.InitialSubEtat);
        public ObjState LinkedState => GetState(CurrentState?.NextMainEtat ?? -1, CurrentState?.NextSubEtat ?? -1);

        protected ObjState GetState(int etat, int subEtat) => AnimGroup?.ETA?.ElementAtOrDefault(etat)?.ElementAtOrDefault(subEtat);

        public Unity_ObjectManager_R2.AnimGroup AnimGroup => ObjManager.AnimGroups.ElementAtOrDefault(AnimGroupIndex);

        public int AnimGroupIndex
        {
            get => ObjManager.AnimGroupsLookup.TryGetItem(EventData.AnimSetPointer?.AbsoluteOffset ?? 0, -1);
            set {
                if (value != AnimGroupIndex && EventData.AnimSetPointer != null) {
                    EventData.MainEtat = EventData.InitialMainEtat = 0;
                    EventData.SubEtat = EventData.InitialSubEtat = 0;
                    OverrideAnimIndex = null;
                    EventData.AnimSetPointer = ObjManager.AnimGroups[value].Pointer;
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

        public override BinarySerializable SerializableData => EventData;

        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool IsAlwaysEvent { get; set; }
        public override bool IsAlways => IsAlwaysEvent;
        public override bool IsEditor => (AnimGroup?.Animations?.Any() != true && EventData.Type != R2_ObjType.TYPE_INVALID) || EventData.DisplayPrio == 0;
        public override Unity_ObjectType Type
        {
            get
            {
                switch (EventData.Type)
                {
                    case R2_ObjType.TYPE_GENERATING_DOOR:
                    case R2_ObjType.TYPE_TRIGGER:
                        return Unity_ObjectType.Trigger;

                    case R2_ObjType.TYPE_RAY_POS:
                        return Unity_ObjectType.Waypoint;

                    default:
                        return Unity_ObjectType.Object;
                }
            }
        }

        public override bool IsActive => !Settings.LoadFromMemory || (EventData.Type != R2_ObjType.TYPE_INVALID && EventData.IsActive);
        public override bool CanBeLinkedToGroup => true;
        public override bool CanBeLinked => EventData.UserData_Gendoor != null || EventData.UserData_Trigger != null;

        public override IEnumerable<int> Links
        {
            get
            {
                if (EventData.UserData_Gendoor != null)
                    foreach (var l in EventData.UserData_Gendoor.LinkedObjects)
                        yield return l;

                if (EventData.UserData_Trigger != null)
                    foreach (var l in EventData.UserData_Trigger.LinkedObjects)
                        yield return l;
            }
        }

        public override string PrimaryName => $"TYPE_{(ushort)EventData.Type}";
        public override string SecondaryName => $"{EventData.Type}";
        // TODO: Fix
        public override int? GetLayer(int index) => -(index + (EventData.DisplayPrio * 512));

        public override int? MapLayer => EventData.MapLayer == R2_ObjData.ObjMapLayer.Back ? 2: 3;

        public override float Scale => EventData.MapLayer == R2_ObjData.ObjMapLayer.Back ? 0.5f : 1;
        public override bool FlipHorizontally => Settings.LoadFromMemory ? EventData.FlipX : EventData.InitialFlipX;

        protected IEnumerable<Unity_ObjAnimationCollisionPart> GetObjZDC() {
            var zdcEntry = EventData.Character?.ZDC;

            if (zdcEntry == null)
                yield break;

            // Function at 0x800d8264 (BOX_IN_COLL_ZONES)

            for (int i = 0; i < zdcEntry.Count; i++)
            {
                var zdc = ObjManager.LevData.ZDC?.ElementAtOrDefault(zdcEntry.Index + i);
                var zdcTriggerFlags = ObjManager.LevData.ZDCTriggerFlags?.ElementAtOrDefault(zdcEntry.Index + i);

                if (zdc == null)
                    continue;

                if (zdc.R2_Flags != 0 && (zdc.R2_Flags & EventData.GendoorExtGroup) == 0) 
                    continue;

                var hurtsRay = EventData.Character?.Flags.HitRay == true && 
                               CurrentState?.RayCollision == true && 
                               zdcTriggerFlags?.HasFlag(R2_LevelData.ZDC_TriggerFlags.Rayman) == true;

                // Attempt to set the collision type
                var colType = hurtsRay 
                    ? Unity_ObjAnimationCollisionPart.CollisionType.AttackBox
                    : zdcTriggerFlags?.HasFlag(R2_LevelData.ZDC_TriggerFlags.Poing_0) == true || zdcTriggerFlags?.HasFlag(R2_LevelData.ZDC_TriggerFlags.Poing_1) == true
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

                    addX += img.SpriteXPosition;
                    addY += img.SpriteYPosition;

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
            get => Mathf.Clamp(EventData.AnimationFrame, 0, CurrentAnimation?.Frames.Length - 1 ?? 0);
            set => EventData.AnimationFrame = (byte)value;
        }

        public override int? AnimationIndex
        {
            get => EventData.AnimationIndex;
            set => EventData.AnimationIndex = (byte)(value ?? 0);
        }

        public override int AnimSpeed => CurrentState?.AnimationSpeed ?? 0;

        public override int? GetAnimIndex => OverrideAnimIndex ?? CurrentState?.AnimationIndex;
        protected override int GetSpriteID => AnimGroupIndex;
        public override IList<Sprite> Sprites => ObjManager.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.Character?.OffsetBX ?? 0, -EventData.Character?.OffsetBY ?? 0);

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
                    ResetState();
                }
                else
                {
                    // Update state values to the linked one
                    EventData.MainEtat = state.NextMainEtat;
                    EventData.SubEtat = state.NextSubEtat;

                    // For always objects the game sets the last state in the chain to link to -1 to indicate that the object should now be hidden again, but to avoid the object now displaying in the editor we reset it here
                    if (EventData.MainEtat == 0xFF || EventData.SubEtat == 0xFF)
                        ResetState();
                }
            }
        }

        protected void ResetState()
        {
            // Reset the state
            EventData.MainEtat = EventData.InitialMainEtat;
            EventData.SubEtat = EventData.InitialSubEtat;

            // Clear encountered states
            EncounteredStates.Clear();
        }

        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_R2 obj)
            {
                Obj = obj;
            }

            private Unity_Object_R2 Obj { get; }

            public override ushort Type
            {
                get => (ushort)Obj.EventData.Type;
                set => Obj.EventData.Type = (R2_ObjType)value;
            }

            public override int DES
            {
                get => Obj.AnimGroupIndex;
                set => Obj.AnimGroupIndex = value;
            }

            public override int ETA
            {
                get => Obj.AnimGroupIndex;
                set => Obj.AnimGroupIndex = value;
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

            public override int EtatLength => Obj.AnimGroup?.ETA?.Length ?? 0;
            public override int SubEtatLength => Obj.AnimGroup?.ETA.ElementAtOrDefault(Obj.EventData.MainEtat)?.Length ?? 0;

            public override byte OffsetBX
            {
                get => Obj.EventData.Character?.OffsetBX ?? 0;
                set
                {
                    if (Obj.EventData.Character != null)
                        Obj.EventData.Character.OffsetBX = value;
                }
            }

            public override byte OffsetBY
            {
                get => Obj.EventData.Character?.OffsetBY ?? 0;
                set
                {
                    if (Obj.EventData.Character != null)
                        Obj.EventData.Character.OffsetBY = value;
                }
            }

            public override byte OffsetHY
            {
                get => Obj.EventData.Character?.OffsetHY ?? 0;
                set
                {
                    if (Obj.EventData.Character != null)
                        Obj.EventData.Character.OffsetHY = value;
                }
            }

            public override uint HitPoints
            {
                get => Obj.EventData.HitPoints;
                set => Obj.EventData.HitPoints = (byte)value;
            }
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
                    r2obj.EventData.MainEtat = r2obj.EventData.InitialMainEtat = Etat;
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
                        && Etat == ((Unity_Object_R2)obj).EventData.InitialMainEtat
                        && SubEtat == ((Unity_Object_R2)obj).EventData.InitialSubEtat;
            }
        }
        #endregion
    }
}