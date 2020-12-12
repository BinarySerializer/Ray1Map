using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_SNES : Unity_Object
    {
        public Unity_Object_SNES(Unity_ObjectManager_SNES objManager)
        {
            ObjManager = objManager;
        }

        public Unity_ObjectManager_SNES ObjManager { get; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }

        public int StateIndex { get; set; }

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => null;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Rayman";
        public override string SecondaryName => null;

        public Unity_ObjectManager_SNES.State State => ObjManager.States.ElementAtOrDefault(StateIndex);

        public override Unity_ObjAnimation CurrentAnimation => State?.Animation;
        public override int AnimSpeed => 2; // TODO: Fix
        public override int? GetAnimIndex => StateIndex;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => ObjManager.Sprites;


        #region UI States
        protected int UIStates_StateIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => StateIndex == UIStates_StateIndex;

        protected override void RecalculateUIStates() {
            UIStates_StateIndex = StateIndex;
            UIStates = ObjManager?.States?.Select((x, i) => (UIState)new SNES_UIState($"{i}", i)).ToArray() ?? new UIState[0];
        }

        protected class SNES_UIState : UIState {
            public SNES_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj) {
                var snesObj = (Unity_Object_SNES)obj;
                snesObj.StateIndex = (short)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_SNES)obj).StateIndex;
        }
        #endregion

        #region LegacyEditorWrapper
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_SNES obj)
            {
                Obj = obj;
            }

            private Unity_Object_SNES Obj { get; }

            public ushort Type { get; set; }

            public int DES { get; set; }

            public int ETA { get; set; }

            public byte Etat
            {
                get => (byte)Obj.StateIndex;
                set => Obj.StateIndex = value;
            }

            public byte SubEtat { get; set; }

            public int EtatLength => Obj.ObjManager.States.Length;
            public int SubEtatLength => 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
        #endregion
    }
}