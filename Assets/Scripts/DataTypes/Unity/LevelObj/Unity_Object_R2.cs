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
            set {
                if (value != AnimGroupIndex) {
                    EventData.Etat = EventData.RuntimeEtat = 0;
                    EventData.SubEtat = EventData.RuntimeSubEtat = 0;
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

        public override string DebugText => $"UShort_00: {EventData.UShort_00}{Environment.NewLine}" +
                                            $"UShort_02: {EventData.UShort_02}{Environment.NewLine}" +
                                            $"UShort_04: {EventData.UShort_04}{Environment.NewLine}" +
                                            $"UShort_06: {EventData.UShort_06}{Environment.NewLine}" +
                                            $"UShort_08: {EventData.UShort_08}{Environment.NewLine}" +
                                            $"UShort_0A: {EventData.UShort_0A}{Environment.NewLine}" +
                                            $"UnkStateRelatedValue: {EventData.UnkStateRelatedValue}{Environment.NewLine}" +
                                            $"Unk_22: {EventData.Unk_22}{Environment.NewLine}" +
                                            $"MapLayer: {EventData.MapLayer}{Environment.NewLine}" +
                                            $"Unk1: {EventData.Unk1}{Environment.NewLine}" +
                                            $"Unk2: {String.Join("-", EventData.Unk2)}{Environment.NewLine}" +
                                            $"RuntimeUnk1: {EventData.EventIndex}{Environment.NewLine}" +
                                            $"EventType: {EventData.EventType}{Environment.NewLine}" +
                                            $"RuntimeOffset1: {EventData.RuntimeOffset1}{Environment.NewLine}" +
                                            $"RuntimeOffset2: {EventData.RuntimeOffset2}{Environment.NewLine}" +
                                            $"RuntimeBytes1: {String.Join("-", EventData.RuntimeBytes1)}{Environment.NewLine}" +
                                            $"Unk_58: {EventData.Unk_58}{Environment.NewLine}" +
                                            $"Unk3: {String.Join("-", EventData.Unk3)}{Environment.NewLine}" +
                                            $"Unk4: {String.Join("-", EventData.Unk4)}{Environment.NewLine}" +
                                            $"Flags: {String.Join(", ", EventData.Flags.GetFlags())}{Environment.NewLine}" +
                                            $"Unk5: {String.Join("-", EventData.Unk5)}{Environment.NewLine}" +
                                            $"ZDC.ZDCIndex: {EventData.CollisionData?.ZDC.ZDCIndex}{Environment.NewLine}" +
                                            $"ZDC.ZDCCount: {EventData.CollisionData?.ZDC.ZDCCount}{Environment.NewLine}";

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool IsAlwaysEvent { get; set; }
        public override bool IsAlways => IsAlwaysEvent;
        public override bool IsEditor => !AnimGroup.Animations.Any() && EventData.EventType != R1_R2EventType.None;

        public override string PrimaryName => $"TYPE_{(ushort)EventData.EventType}";
        public override string SecondaryName => $"{EventData.EventType}";
        // TODO: Fix
        public override int? GetLayer(int index) => -(index + (EventData.Layer * 512));

        public override int? MapLayer => EventData.MapLayer == R1_R2EventData.ObjMapLayer.Back ? 2: 3;

        public override float Scale => EventData.MapLayer == R1_R2EventData.ObjMapLayer.Back ? 0.5f : 1;
        public override bool FlipHorizontally => EventData.IsFlippedHorizontally;

        protected IEnumerable<Unity_ObjAnimationCollisionPart> GetObjZDC() {
            var zdcEntry = EventData.CollisionData?.ZDC;

            if (zdcEntry == null)
                yield break;

            // Hard-coded for gendoors
            if (EventData.EventType == R1_R2EventType.Gendoor_Spawn || EventData.EventType == R1_R2EventType.Gendoor_Trigger) {
                // Function at 0x800e26c0

                int zdcIndex;
                var flags = EventData.UnkFlags & 0xfc;

                if (flags == 0x04)
                    zdcIndex = zdcEntry.ZDCIndex;
                else if (flags == 0x08)
                    zdcIndex = zdcEntry.ZDCIndex + 1;
                else if (flags == 0x10)
                    zdcIndex = zdcEntry.ZDCIndex + 2;
                else if (flags == 0x20)
                    zdcIndex = zdcEntry.ZDCIndex + 3;
                else if (flags == 0x40)
                    zdcIndex = zdcEntry.ZDCIndex + 4;
                else
                    yield break;

                var zdc = ObjManager.ZDC?.ElementAtOrDefault(zdcIndex);

                if (zdc != null) {
                    yield return new Unity_ObjAnimationCollisionPart {
                        XPosition = zdc.XPosition,
                        YPosition = zdc.YPosition,
                        Width = zdc.Width,
                        Height = zdc.Height,
                        Type = Unity_ObjAnimationCollisionPart.CollisionType.Gendoor
                    };
                }
            } else {
                // Function at 0x800d7f90

                for (int i = 0; i < zdcEntry.ZDCCount; i++) {
                    var zdc = ObjManager.ZDC?.ElementAtOrDefault(zdcEntry.ZDCIndex + i);

                    if (zdc != null) {
                        // Relative to the event origin
                        if (zdc.LayerIndex == 0x1F) {
                            yield return new Unity_ObjAnimationCollisionPart {
                                XPosition = zdc.XPosition,
                                YPosition = zdc.YPosition,
                                Width = zdc.Width,
                                Height = zdc.Height,
                                Type = Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox
                            };
                        }
                        // Relative to an animation layer
                        else {
                            Unity_ObjAnimationPart p = CurrentAnimation?.Frames[AnimationFrame].SpriteLayers.ElementAtOrDefault(zdc.LayerIndex);
                            int addX = 0, addY = 0;
                            if (p != null) {
                                /*int w = 0, h = 0;
                                if ((p.IsFlippedHorizontally || p.IsFlippedVertically) && p.ImageIndex < Sprites.Count) {
                                    var spr = Sprites[p.ImageIndex];
                                    w = spr?.texture?.width ?? 0;
                                    h = spr?.texture?.height ?? 0;
                                }*/
                                addX = p.XPosition;// + (p.IsFlippedHorizontally ? w : 0);
                                addY = p.YPosition;// - (p.IsFlippedVertically ? h : 0);
                                if (p.ImageIndex < ObjManager.ImageDescriptors.Length) {
                                    var img = ObjManager.ImageDescriptors[p.ImageIndex];
                                    addX += img.HitBoxOffsetX;
                                    addY += img.HitBoxOffsetY;
                                }
                            }
                            yield return new Unity_ObjAnimationCollisionPart
                            {
                                XPosition = zdc.XPosition + addX,
                                YPosition = zdc.YPosition + addY,
                                Width = zdc.Width,
                                Height = zdc.Height,
                                Type = Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox
                            };
                        }
                    }
                }
            }
        }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => GetObjZDC().ToArray();

        public override Unity_ObjAnimation CurrentAnimation => AnimGroup?.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
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

        public override int AnimSpeed => State?.AnimationSpeed ?? 0;

        public override int? GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex;
        public override IList<Sprite> Sprites => ObjManager.Sprites;
        public override Vector2 Pivot => new Vector2(EventData.CollisionData?.OffsetBX ?? 0, -EventData.CollisionData?.OffsetBY ?? 0);

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

            public uint HitPoints { get; set; }

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
                        uiStates.Add(new R2_UIState($"State {i}-{j}", i, j));
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
                    r2obj.EventData.Etat = r2obj.EventData.RuntimeEtat = Etat;
                    r2obj.EventData.SubEtat = r2obj.EventData.RuntimeSubEtat = SubEtat;
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
                        && Etat == (obj as Unity_Object_R2).EventData.RuntimeEtat
                        && SubEtat == (obj as Unity_Object_R2).EventData.RuntimeSubEtat;
            }
        }
        #endregion
    }
}