using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAMadTrax : Unity_Object
    {
        public Unity_Object_GBAMadTrax(Unity_ObjectManager_GBAMadTrax objManager)
        {
            ObjManager = objManager;
        }

        public Unity_ObjectManager_GBAMadTrax ObjManager { get; }

        public int CurrentSprite { get; set; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => null;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Rayman";
        public override string SecondaryName => null;

        public Unity_ObjectManager_GBAMadTrax.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(CurrentSprite);

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Animation;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => CurrentSprite;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => GraphicsData?.Sprites;


        #region UI States
        protected bool UIStates_HasInitialized { get; set; }
        protected override bool IsUIStateArrayUpToDate => UIStates_HasInitialized;

        protected override void RecalculateUIStates() {
            UIStates_HasInitialized = true;
            UIStates = ObjManager?.GraphicsDatas?.Select((x, i) => (UIState)new GBAMadTrax_UIState($"{x.Pointer}", i)).ToArray() ?? new UIState[0];
        }

        protected class GBAMadTrax_UIState : UIState {
            public GBAMadTrax_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj) {
                var madTraxObj = (Unity_Object_GBAMadTrax)obj;
                madTraxObj.CurrentSprite = (short)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBAMadTrax)obj).CurrentSprite;
        }
        #endregion

        #region LegacyEditorWrapper
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAMadTrax obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAMadTrax Obj { get; }

            public ushort Type { get; set; }

            public int DES { get; set; }

            public int ETA { get; set; }

            public byte Etat
            {
                get => (byte)Obj.CurrentSprite;
                set => Obj.CurrentSprite = value;
            }

            public byte SubEtat { get; set; }

            public int EtatLength => Obj.ObjManager.GraphicsDatas.Length;
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