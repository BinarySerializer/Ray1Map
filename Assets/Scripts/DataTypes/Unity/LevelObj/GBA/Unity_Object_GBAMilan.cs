﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAMilan : Unity_Object
    {
        public Unity_Object_GBAMilan(GBA_Actor actor, Unity_ObjectManager_GBA objManager)
        {
            // Set properties
            Actor = actor;
            ObjManager = objManager;
        }

        public bool IsCaptor => Actor.Type == GBA_Actor.ActorType.Captor;

        public GBA_Actor Actor { get; }

        public Unity_ObjectManager_GBA ObjManager { get; }

        public GBA_Action State => ModelData?.Actions.ElementAtOrDefault(Actor.ActionIndex);
        public Unity_ObjectManager_GBA.ModelData ModelData => ObjManager.ActorModels.ElementAtOrDefault(GraphicsDataIndex);

        public int GraphicsDataIndex
        {
            get => IsCaptor ? -1 : ObjManager.GraphicsDataLookup.TryGetItem(Actor.Index_ActorModel, -1);
            set
            {
                if (value != GraphicsDataIndex)
                {
                    Actor.ActionIndex = 0;
                    OverrideAnimIndex = null;
                    Actor.Index_ActorModel = (byte)ObjManager.ActorModels[value].Index;
                }
            }
        }

        public override short XPosition
        {
            get => Actor.XPos;
            set => Actor.XPos = value;
        }

        public override short YPosition
        {
            get => Actor.YPos;
            set => Actor.YPos = value;
        }

        public override string DebugText => String.Empty;

        public override ObjectType Type => IsCaptor ? ObjectType.Trigger : ObjectType.Object;

        public override R1Serializable SerializableData => Actor;

        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => IsCaptor ? "Captor" : ModelData?.Name ?? $"Actor";
        public override string SecondaryName => null;

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links => Actor.Milan_Links.Select(x => (int)x.LinkedActor);

        public override Unity_ObjAnimation CurrentAnimation => IsCaptor ? null : ModelData?.Graphics.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimSpeed => CurrentAnimation?.AnimSpeed ?? CurrentAnimation?.AnimSpeeds?.ElementAtOrDefault(AnimationFrame) ?? 0;

        public override int? GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex;
        protected override int GetSpriteID => GraphicsDataIndex;
        public override IList<Sprite> Sprites => ModelData?.Graphics.Sprites;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAMilan obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAMilan Obj { get; }

            public ushort Type { get; set; }

            public int DES
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public int ETA
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => (byte)Obj.Actor.ActionIndex;
                set => Obj.Actor.ActionIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.ModelData?.Actions.Length ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }


		#region UI States
		protected int UIStates_GraphicsDataIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => GraphicsDataIndex == UIStates_GraphicsDataIndex;

        protected class GBA_UIState : UIState {
            public GBA_UIState(string displayName, byte stateIndex) : base(displayName) {
                StateIndex = stateIndex;
            }
            public GBA_UIState(string displayName, int animIndex) : base(displayName, animIndex) {}

            public byte StateIndex { get; }

            public override void Apply(Unity_Object obj) {
                if (IsState) {
                    (obj as Unity_Object_GBAMilan).Actor.ActionIndex = StateIndex;
                    obj.OverrideAnimIndex = null;
                } else {
                    obj.OverrideAnimIndex = AnimIndex;
                }
            }

            public override bool IsCurrentState(Unity_Object obj) {

                if (obj.OverrideAnimIndex.HasValue)
                    return !IsState && AnimIndex == obj.OverrideAnimIndex;
                else
                    return IsState && StateIndex == (obj as Unity_Object_GBAMilan).Actor.ActionIndex;
            }
        }

        protected override void RecalculateUIStates() {
            UIStates_GraphicsDataIndex = GraphicsDataIndex;
            var states = ModelData?.Actions;
            var anims = ModelData?.Graphics.Animations;
            HashSet<int> usedAnims = new HashSet<int>();
            List<UIState> uiStates = new List<UIState>();
            if (states != null) {
                for (byte i = 0; i < states.Length; i++) {
                    uiStates.Add(new GBA_UIState($"State {i} (Animation {states[i].AnimationIndex})", stateIndex: i));
                    usedAnims.Add(states[i].AnimationIndex);
                }
            }
            if (anims != null) {
                for (int i = 0; i < anims.Count; i++) {
                    if (usedAnims.Contains(i)) continue;
                    uiStates.Add(new GBA_UIState($"Animation {i}", animIndex: i));
                }
            }

            UIStates = uiStates.ToArray();
        }
		#endregion
	}
}