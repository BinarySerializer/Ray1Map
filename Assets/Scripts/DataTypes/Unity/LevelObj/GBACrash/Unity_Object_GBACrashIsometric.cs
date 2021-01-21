using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBACrashIsometric : Unity_Object_3D
    {
        public Unity_Object_GBACrashIsometric(GBACrash_Isometric_Object obj, Unity_ObjectManager_GBACrashIsometric objManager)
        {
            Object = obj;
            ObjManager = objManager;

            _prevTimeTrialMode = Settings.GBACrash_TimeTrialMode;
            UpdateAnimIndex();
        }

        public void UpdateAnimIndex() => ObjAnimIndex = (int)(Settings.GBACrash_TimeTrialMode && Object.ObjType_TimeTrial != GBACrash_Isometric_Object.GBACrash_Isometric_ObjType.None ? Object.ObjType_TimeTrial : Object.ObjType);

        public GBACrash_Isometric_Object Object { get; }
        public Unity_ObjectManager_GBACrashIsometric ObjManager { get; }

        public int ObjAnimIndex { get; set; }

        public float Height { get; set; }

        public override short XPosition
        {
            get => (short)Object.XPos.AsFloat;
            set => Object.XPos.AsFloat = value;
        }
        public override short YPosition
        {
            get => (short)Object.YPos.AsFloat;
            set => Object.YPos.AsFloat = value;
        }

        public override Vector3 Position
        {
            get => new Vector3(Object.YPos, Object.XPos, Height);
            set
            {
                Object.YPos.AsFloat = value.x;
                Object.XPos.AsFloat = value.y;
                Height = value.z;
            }
        }

        public override string DebugText => $"ObjType_TimeTrial: {Object.ObjType_TimeTrial}{Environment.NewLine}";

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{(int)Object.ObjType}";
        public override string SecondaryName => $"{Object.ObjType}";

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
                var rrrObj = (Unity_Object_GBACrashIsometric)obj;
                rrrObj.ObjAnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBACrashIsometric)obj).ObjAnimIndex;
        }
        #endregion

        #region LegacyEditorWrapper
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBACrashIsometric obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBACrashIsometric Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.Object.ObjType;
                set => Obj.Object.ObjType = (GBACrash_Isometric_Object.GBACrash_Isometric_ObjType)value;
            }

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