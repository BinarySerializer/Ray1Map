using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_Object_BaseGBAVVIsometric : Unity_SpriteObject_3D
    {
        protected Unity_Object_BaseGBAVVIsometric(Unity_ObjectManager_GBAVVIsometric objManager)
        {
            ObjManager = objManager;

            _prevTimeTrialMode = Settings.GBAVV_Crash_TimeTrialMode;
        }

        public abstract void UpdateAnimIndex();

        public Unity_ObjectManager_GBAVVIsometric ObjManager { get; }

        public int ObjAnimIndex { get; set; }

        public abstract FixedPointInt32 XPos { get; set; }
        public abstract FixedPointInt32 YPos { get; set; }
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

        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public bool _prevTimeTrialMode;
        public override void OnUpdate()
        {
            if (_prevTimeTrialMode == Settings.GBAVV_Crash_TimeTrialMode)
                return;

            _prevTimeTrialMode = Settings.GBAVV_Crash_TimeTrialMode;
            UpdateAnimIndex();
        }

        public Unity_ObjectManager_GBAVVIsometric.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(ObjAnimIndex);

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
            UIStates = ObjManager?.GraphicsDatas?.Select((x, i) => (UIState)new GBAVVIsometric_UIState($"{i}", i)).ToArray() ?? new UIState[0];
        }

        protected class GBAVVIsometric_UIState : UIState {
            public GBAVVIsometric_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj) {
                var crashObj = (Unity_Object_GBAVVIsometric_Obj)obj;
                crashObj.ObjAnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBAVVIsometric_Obj)obj).ObjAnimIndex;
        }
        #endregion

        #region LegacyEditorWrapper
        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_BaseGBAVVIsometric obj)
            {
                Obj = obj;
            }

            private Unity_Object_BaseGBAVVIsometric Obj { get; }

            public override int DES
            {
                get => Obj.ObjAnimIndex;
                set => Obj.ObjAnimIndex = value;
            }

            public override int ETA
            {
                get => Obj.ObjAnimIndex;
                set => Obj.ObjAnimIndex = value;
            }
        }
        #endregion
    }
}