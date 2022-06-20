using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Ray1.Jaguar;
using Sprite = UnityEngine.Sprite;

namespace Ray1Map.Rayman1_Jaguar
{
    public class Unity_Object_R1Jaguar : Unity_SpriteObject
    {
        public Unity_Object_R1Jaguar(Unity_ObjectManager_R1Jaguar objManager, Pointer eventDefinitionPointer)
        {
            // Set properties
            ObjManager = objManager;
            EventDefinitionPointer = eventDefinitionPointer;

            // Set editor states
            RuntimeComplexStateIndex = ComplexStateIndex;
            RuntimeStateIndex = StateIndex;
        }

        public Unity_ObjectManager_R1Jaguar ObjManager { get; }

        public JAG_Event Instance { get; set; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }
        public override string DebugText => $"{nameof(Instance.Ushort_00)}: {Instance?.Ushort_00}{Environment.NewLine}" +
                                            $"{nameof(Instance.Unk_0A)}: {Instance?.Unk_0A}{Environment.NewLine}" +
                                            $"{nameof(Instance.EventIndex)}: {Instance?.EventIndex}{Environment.NewLine}" +
                                            $"{nameof(EventDefinitionPointer)}: {EventDefinitionPointer}{Environment.NewLine}" +
                                            $"IsComplex: {ObjManager.EventDefinitions[EventDefinitionIndex].Definition.ComplexData != null}{Environment.NewLine}" +
                                            $"Byte_25: {ObjManager.EventDefinitions[EventDefinitionIndex].Definition.Byte_25}{Environment.NewLine}" +
                                            $"Byte_26: {ObjManager.EventDefinitions[EventDefinitionIndex].Definition.Byte_26}{Environment.NewLine}" +
                                            $"{nameof(Instance.OffsetX)}: {Instance?.OffsetX}{Environment.NewLine}" +
                                            $"{nameof(Instance.OffsetY)}: {Instance?.OffsetY}{Environment.NewLine}";

        public override bool CanBeLinkedToGroup => true;

        protected Pointer EventDefinitionPointer { get; set; }
        public int EventDefinitionIndex
        {
            get => ObjManager.EventDefinitions.FindIndex(x => x.Pointer == EventDefinitionPointer);
            set {
                if (value != EventDefinitionIndex) {
                    ComplexStateIndex = RuntimeComplexStateIndex = 0;
                    StateIndex = RuntimeStateIndex = 0;
                    OverrideAnimIndex = null;
                    EventDefinitionPointer = ObjManager.EventDefinitions[value].Pointer;
                }
            }
        }

        public byte RuntimeStateIndex { get; set; }
        public byte StateIndex { get; set; }
        public byte RuntimeComplexStateIndex { get; set; }
        public byte ComplexStateIndex { get; set; }

        public bool ForceNoAnimation { get; set; }
        public byte? ForceFrame { get; set; }

        public int LinkIndex { get; set; }

        public override Unity_ObjectType Type => ObjManager.EventDefinitions[EventDefinitionIndex].DisplayName == "MS_ge1" ? Unity_ObjectType.Trigger : Unity_ObjectType.Object;

        public Unity_ObjGraphics DES => ObjManager.EventDefinitions[EventDefinitionIndex].DES;
        public Unity_ObjectManager_R1Jaguar.State[][] ETA => ObjManager.EventDefinitions[EventDefinitionIndex].ETA;
        public Unity_ObjectManager_R1Jaguar.State State => ETA?.ElementAtOrDefault(RuntimeComplexStateIndex)?.ElementAtOrDefault(RuntimeStateIndex);

        public override BinarySerializable[] AdditionalSerializableDatas => new BinarySerializable[]
        {
            ObjManager.EventDefinitions[EventDefinitionIndex].Definition,
            Instance,
        };

        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => ObjManager.EventDefinitions[EventDefinitionIndex].DisplayName;
        public override string SecondaryName => null;
        public override Unity_ObjAnimation CurrentAnimation => DES.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimSpeed => (ForceNoAnimation ? 0 : State?.AnimSpeed ?? 1);
        public override int? GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex;
        protected override int GetSpriteID => EventDefinitionIndex;
        public override IList<Sprite> Sprites => DES.Sprites;

        // TODO: Uncomment this once we can get all collision to align correctly. We should also implement ZDD collision from the states.
        //       The biggest issue is how some colliders attach to the sprites. It seems rather inconsistent. The sprite value should
        //       be an offset in an internal pointer array (16-bit pointers), but it doesn't always work correctly. The game probably
        //       hard-codes some of it since collision checks aren't normalized, so it might not always calculate it relative.
        //private bool _loggedColWarn;
        //public override Unity_ObjAnimationCollisionPart[] ObjCollision
        //{
        //    get
        //    {
        //        Unity_ObjAnimationPart[] sprites = CurrentAnimation?.Frames[AnimationFrame].SpriteLayers;

        //        if (sprites?.Any() != true)
        //            return Array.Empty<Unity_ObjAnimationCollisionPart>();

        //        JAG_Character car = ObjManager.EventDefinitions[EventDefinitionIndex].Definition?.Character;

        //        if (car == null)
        //            return Array.Empty<Unity_ObjAnimationCollisionPart>();

        //        var col = new List<Unity_ObjAnimationCollisionPart>();

        //        if (car.Type != JAG_CollideType.nib)
        //        {
        //            int spriteIndex = (car.ColSprite / 2) - 1;
        //            Unity_ObjAnimationPart p = sprites.ElementAtOrDefault(spriteIndex);

        //            if (p == null && car.ColSprite != 0)
        //            {
        //                if (!_loggedColWarn)
        //                {
        //                    UnityEngine.Debug.LogWarning($"Sprite {spriteIndex} is out of bounds for {PrimaryName} for car collision");
        //                    _loggedColWarn = true;
        //                }
        //            }
        //            else
        //            {
        //                col.Add(new Unity_ObjAnimationCollisionPart()
        //                {
        //                    XPosition = car.ColX + (p?.XPosition ?? 0),
        //                    YPosition = car.ColY + (p?.YPosition ?? 0),
        //                    Width = car.ColWidth,
        //                    Height = car.ColHeight,
        //                });
        //            }
        //        }

        //        foreach (JAG_CharacterCollide collide in car.Collides)
        //        {
        //            int spriteIndex = (car.ColSprite / 2) - 1;
        //            Unity_ObjAnimationPart p = sprites.ElementAtOrDefault(spriteIndex);

        //            if (p == null)
        //            {
        //                if (!_loggedColWarn)
        //                {
        //                    UnityEngine.Debug.LogWarning($"Sprite {spriteIndex} is out of bounds for {PrimaryName} for collide");
        //                    _loggedColWarn = true;
        //                }
        //                continue;
        //            }

        //            col.Add(new Unity_ObjAnimationCollisionPart()
        //            {
        //                XPosition = collide.XPos + p.XPosition,
        //                YPosition = collide.YPos + p.YPosition,
        //                Width = collide.Width,
        //                Height = collide.Height,
        //            });
        //        }

        //        return col.ToArray();
        //    }
        //}

        protected override bool ShouldUpdateFrame()
        {
            if (ForceFrame != null && ForceNoAnimation)
            {
                AnimationFrame = ForceFrame.Value;
                AnimationFrameFloat = ForceFrame.Value;
                return false;
            }

            return true;
        }

        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_R1Jaguar obj)
            {
                Obj = obj;
            }

            private Unity_Object_R1Jaguar Obj { get; }

            public override int DES
            {
                get => Obj.EventDefinitionIndex;
                set => Obj.EventDefinitionIndex = value;
            }

            public override byte Etat
            {
                get => Obj.ComplexStateIndex;
                set => Obj.ComplexStateIndex = Obj.RuntimeComplexStateIndex = value;
            }

            public override byte SubEtat
            {
                get => Obj.StateIndex;
                set => Obj.StateIndex = Obj.RuntimeStateIndex = value;
            }

            public override int EtatLength => Obj.ObjManager.EventDefinitions.ElementAtOrDefault(Obj.EventDefinitionIndex)?.ETA.Length ?? 0;
            public override int SubEtatLength => Obj.ObjManager.EventDefinitions.ElementAtOrDefault(Obj.EventDefinitionIndex)?.ETA.ElementAtOrDefault(Obj.RuntimeComplexStateIndex)?.Length ?? 0;
        }

        #region UI States
        protected int UIStates_EventDefinitionIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => EventDefinitionIndex == UIStates_EventDefinitionIndex;

        protected class R1Jaguar_UIState : UIState {
            public R1Jaguar_UIState(string displayName, byte complexStateIndex, byte stateIndex) : base(displayName) {
                ComplexStateIndex = complexStateIndex;
                StateIndex = stateIndex;
            }

            public byte ComplexStateIndex { get; }
            public byte StateIndex { get; }

            public override void Apply(Unity_Object obj) {
                if (IsState) {
                    var r1jo = obj as Unity_Object_R1Jaguar;
                    r1jo.RuntimeComplexStateIndex = r1jo.ComplexStateIndex = ComplexStateIndex;
                    r1jo.RuntimeStateIndex = r1jo.StateIndex = StateIndex;
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
                        && ComplexStateIndex == (obj as Unity_Object_R1Jaguar).RuntimeComplexStateIndex
                        && StateIndex == (obj as Unity_Object_R1Jaguar).RuntimeStateIndex;
            }
        }

        protected override void RecalculateUIStates() {
            UIStates_EventDefinitionIndex = EventDefinitionIndex;
            //HashSet<int> usedAnims = new HashSet<int>();
            List<UIState> uiStates = new List<UIState>();

            var eta = ETA;
            if (eta != null) {
                for (byte i = 0; i < eta.Length; i++) {
                    for (byte j = 0; j < eta[i].Length; j++) {
                        if (eta[i][j].Name != null) {
                            uiStates.Add(new R1Jaguar_UIState($"State {i}-{j}: {eta[i][j].Name}", i, j));
                        } else {
                            uiStates.Add(new R1Jaguar_UIState($"State {i}-{j}", i, j));
                        }
                    }
                }
            }

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}