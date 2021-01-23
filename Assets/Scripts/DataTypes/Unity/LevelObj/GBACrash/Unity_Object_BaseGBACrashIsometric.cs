using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_Object_BaseGBACrashIsometric : Unity_Object_3D
    {
        protected Unity_Object_BaseGBACrashIsometric(Unity_ObjectManager_GBACrashIsometric objManager)
        {
            ObjManager = objManager;

            _prevTimeTrialMode = Settings.GBACrash_TimeTrialMode;
        }

        public abstract void UpdateAnimIndex();

        public Unity_ObjectManager_GBACrashIsometric ObjManager { get; }

        public int ObjAnimIndex { get; set; }

        public abstract FixedPointInt XPos { get; set; }
        public abstract FixedPointInt YPos { get; set; }
        public float Height { get; set; }

        public override short XPosition
        {
            get => (short)XPos.AsFloat;
            set => XPos.AsFloat = value;
        }
        public override short YPosition
        {
            get => (short)YPos.AsFloat;
            set => YPos.AsFloat = value;
        }

        public override Vector3 Position
        {
            get => new Vector3(YPos, XPos, Height);
            set
            {
                YPos.AsFloat = value.x;
                XPos.AsFloat = value.y;
                Height = value.z;
            }
        }

        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool _prevTimeTrialMode;
        public override void OnUpdate()
        {
            if (_prevTimeTrialMode == Settings.GBACrash_TimeTrialMode)
                return;

            _prevTimeTrialMode = Settings.GBACrash_TimeTrialMode;
            UpdateAnimIndex();
        }

        public Unity_ObjectManager_GBACrashIsometric.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(ObjAnimIndex);

        public override Unity_ObjAnimation CurrentAnimation => Sprites?.Count < 1 ? null : GraphicsData?.Animation;
        public override int AnimSpeed => GraphicsData?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => ObjAnimIndex;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => GraphicsData?.AnimFrames;

        #region UI States
        protected bool UIStates_HasInitialized { get; set; }
        protected override bool IsUIStateArrayUpToDate => UIStates_HasInitialized;

        protected override void RecalculateUIStates() {
            UIStates_HasInitialized = true;
            UIStates = ObjManager?.GraphicsDatas?.Select((x, i) => (UIState)new GBACrashIsometric_UIState($"{i}", i)).ToArray() ?? new UIState[0];
        }

        protected class GBACrashIsometric_UIState : UIState {
            public GBACrashIsometric_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj) {
                var crashObj = (Unity_Object_GBACrashIsometric_Obj)obj;
                crashObj.ObjAnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBACrashIsometric_Obj)obj).ObjAnimIndex;
        }
        #endregion

        #region LegacyEditorWrapper
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_BaseGBACrashIsometric obj)
            {
                Obj = obj;
            }

            private Unity_Object_BaseGBACrashIsometric Obj { get; }

            public ushort Type { get; set; }

            public int DES
            {
                get => Obj.ObjAnimIndex;
                set => Obj.ObjAnimIndex = value;
            }

            public int ETA
            {
                get => Obj.ObjAnimIndex;
                set => Obj.ObjAnimIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat { get; set; }

            public int EtatLength => 0;
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