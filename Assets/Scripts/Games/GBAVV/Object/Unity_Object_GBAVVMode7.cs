using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAVV
{
    public class Unity_Object_GBAVVMode7 : Unity_SpriteObject_3D
    {
        public Unity_Object_GBAVVMode7(Unity_ObjectManager_GBAVVMode7 objManager, GBAVV_Mode7_Object obj)
        {
            ObjManager = objManager;
            Object = obj;

            _prevTimeTrialMode = Settings.GBAVV_Crash_TimeTrialMode;
            UpdateAnimIndex();
        }

        public void UpdateAnimIndex() => AnimSetIndex = (int)(Settings.GBAVV_Crash_TimeTrialMode ? Object.ObjType_TimeTrial : Object.ObjType_Normal);

        public Unity_ObjectManager_GBAVVMode7 ObjManager { get; }
        public GBAVV_Mode7_Object Object { get; set; }

        public override short XPosition
        {
            get => (short)Object.XPos;
            set => Object.XPos = value;
        }

        public override short YPosition
        {
            get => (short)Object.ZPos;
            set => Object.ZPos = value;
        }

        public override Vector3 Position
        {
            get => new Vector3(Object.XPos, Object.ZPos, -Object.YPos);
            set
            {
                Object.XPos = (int)value.x;
                Object.ZPos = (int)value.y;
                Object.YPos = -(int)value.z;
            }
        }

        public override string DebugText => $"ObjType_TimeTrial0: {Object.ObjType_TimeTrial}{Environment.NewLine}";

        public Unity_ObjectManager_GBAVVMode7.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndex);
        public Unity_ObjectManager_GBAVVMode7.AnimSet.Animation Animation => AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

        public override BinarySerializable SerializableData => Object;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType_Normal}";
        public override string SecondaryName => null;

        public override bool CanBeLinkedToGroup => true;

        public override Unity_ObjectType Type => AnimSetIndex == -1 ? Unity_ObjectType.Trigger : Unity_ObjectType.Object;

        public bool _prevTimeTrialMode;
        public override void OnUpdate()
        {
            if (_prevTimeTrialMode == Settings.GBAVV_Crash_TimeTrialMode)
                return;

            _prevTimeTrialMode = Settings.GBAVV_Crash_TimeTrialMode;
            UpdateAnimIndex();
        }

        private int _animSetIndex;
        private byte _animIndex;

        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                if (AnimSetIndex == -1)
                    return;

                _animSetIndex = value;
                AnimIndex = 0;
                FreezeFrame = false;
            }
        }

        public byte AnimIndex
        {
            get => _animIndex;
            set
            {
                _animIndex = value;
                FreezeFrame = false;
            }
        }

        public bool FreezeFrame { get; set; }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => AnimSet?.Collision;

        public override Unity_ObjAnimation CurrentAnimation => Animation?.ObjAnimation;
        public override int AnimSpeed => 4;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAVVMode7 obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAVVMode7 Obj { get; }

            public override ushort Type
            {
                get => Obj.Object.ObjType_Normal;
                set => Obj.Object.ObjType_Normal = (byte)value;
            }

            public override int DES
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public override int ETA
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public override byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public override int SubEtatLength => Obj.AnimSet?.Animations?.Length ?? 0;
        }

        #region UI States

        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class GBAVV_UIState : UIState
        {
            public GBAVV_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAVVMode7)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAVVMode7)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAVV_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}