using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_R1Jaguar : Unity_Object
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

        public R1Jaguar_EventInstance Instance { get; set; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }
        public override string DebugText => $"{nameof(Instance.Unk_00)}: {Instance?.Unk_00}{Environment.NewLine}" +
                                            $"{nameof(Instance.Unk_0A)}: {Instance?.Unk_0A}{Environment.NewLine}" +
                                            $"{nameof(Instance.EventIndex)}: {Instance?.EventIndex}{Environment.NewLine}" +
                                            $"{nameof(EventDefinitionPointer)}: {EventDefinitionPointer}{Environment.NewLine}" +
                                            $"IsComplex: {ObjManager.EventDefinitions[EventDefinitionIndex].Definition.ComplexData != null}{Environment.NewLine}" +
                                            $"CAR: {String.Join("-", ObjManager.EventDefinitions[EventDefinitionIndex].Definition.CarData ?? new byte[0])}{Environment.NewLine}" +
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

        public override ObjectType Type => ObjManager.EventDefinitions[EventDefinitionIndex].DisplayName == "MS_ge1" ? ObjectType.Trigger : ObjectType.Object;

        public Unity_ObjGraphics DES => ObjManager.EventDefinitions[EventDefinitionIndex].DES;
        public Unity_ObjectManager_R1Jaguar.State[][] ETA => ObjManager.EventDefinitions[EventDefinitionIndex].ETA;
        public Unity_ObjectManager_R1Jaguar.State State => ETA?.ElementAtOrDefault(RuntimeComplexStateIndex)?.ElementAtOrDefault(RuntimeStateIndex);

        public override BinarySerializable SerializableData => Instance;

        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => ObjManager.EventDefinitions[EventDefinitionIndex].DisplayName;
        public override string SecondaryName => null;
        public override Unity_ObjAnimation CurrentAnimation => DES.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimSpeed => (ForceNoAnimation ? 0 : State?.AnimSpeed ?? 1);
        public override int? GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex;
        protected override int GetSpriteID => EventDefinitionIndex;
        public override IList<Sprite> Sprites => DES.Sprites;

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

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_R1Jaguar obj)
            {
                Obj = obj;
            }

            private Unity_Object_R1Jaguar Obj { get; }

            public ushort Type { get; set; }

            public int DES
            {
                get => Obj.EventDefinitionIndex;
                set => Obj.EventDefinitionIndex = value;
            }

            public int ETA { get; set; }

            public byte Etat
            {
                get => Obj.ComplexStateIndex;
                set => Obj.ComplexStateIndex = Obj.RuntimeComplexStateIndex = value;
            }

            public byte SubEtat
            {
                get => Obj.StateIndex;
                set => Obj.StateIndex = Obj.RuntimeStateIndex = value;
            }

            public int EtatLength => Obj.ObjManager.EventDefinitions.ElementAtOrDefault(Obj.EventDefinitionIndex)?.ETA.Length ?? 0;
            public int SubEtatLength => Obj.ObjManager.EventDefinitions.ElementAtOrDefault(Obj.EventDefinitionIndex)?.ETA.ElementAtOrDefault(Obj.RuntimeComplexStateIndex)?.Length ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
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