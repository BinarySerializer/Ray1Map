using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAVVMode7 : Unity_Object_3D
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

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType_Normal}";
        public override string SecondaryName => null;

        public override bool CanBeLinkedToGroup => true;

        public override ObjectType Type => AnimSetIndex == -1 ? ObjectType.Trigger : ObjectType.Object;

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

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAVVMode7 obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAVVMode7 Obj { get; }

            public ushort Type
            {
                get => Obj.Object.ObjType_Normal;
                set => Obj.Object.ObjType_Normal = (byte)value;
            }

            public int DES
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public int ETA
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.AnimSet?.Animations?.Length ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
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