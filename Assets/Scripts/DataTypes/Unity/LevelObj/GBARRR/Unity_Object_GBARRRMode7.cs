using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRRMode7 : Unity_Object
    {
        public Unity_Object_GBARRRMode7(GBARRR_Mode7Object obj, Unity_ObjectManager_GBARRRMode7 objManager, bool forceNoGraphics)
        {
            Object = obj;
            ObjManager = objManager;
            ForceNoGraphics = forceNoGraphics;
        }

        public GBARRR_Mode7Object Object { get; }
        public Unity_ObjectManager_GBARRRMode7 ObjManager { get; }

        public short AnimTypeIndex
        {
            get => (short)Object.ObjectType;
            set => Object.ObjectType = (GBARRR_Mode7Object.Mode7Type)value;
        }

        public override short XPosition
        {
            get => Object.XPosition;
            set => Object.XPosition = value;
        }
        public override short YPosition
        {
            get => Object.YPosition;
            set => Object.YPosition = value;
        }

        public bool ForceNoGraphics { get; }
        public bool IsRayman => Object.ObjectType == GBARRR_Mode7Object.Mode7Type.Rayman && !ForceNoGraphics;

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{AnimTypeIndex}";
        public override string SecondaryName => (!IsRayman && Object.ObjectType == GBARRR_Mode7Object.Mode7Type.Rayman) ? "Removed" : $"{Object.ObjectType}";

        public Unity_ObjectManager_GBARRRMode7.GraphicsData GraphicsData => ForceNoGraphics ? null : ObjManager.GraphicsDatas.ElementAtOrDefault(AnimTypeIndex);

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Animation;
        public override int AnimSpeed => GraphicsData?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => AnimTypeIndex;
        public override IList<Sprite> Sprites => GraphicsData?.AnimFrames;


        #region UI States
        protected int UIStates_AnimTypeIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimTypeIndex == UIStates_AnimTypeIndex;

        protected override void RecalculateUIStates() {
            UIStates_AnimTypeIndex = AnimTypeIndex;
            if (ForceNoGraphics) {
                UIStates = new UIState[0];
            } else {
                UIStates = ObjManager?.GraphicsDatas?.Select((x, i) => (UIState)new RRRMode7_UIState($"{i} ({(GBARRR_Mode7Object.Mode7Type)i})", i)).ToArray() ?? new UIState[0];
            }
        }

        protected class RRRMode7_UIState : UIState {
            public RRRMode7_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj) {
                var rrrObj = (Unity_Object_GBARRRMode7)obj;
                rrrObj.AnimTypeIndex = (short)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBARRRMode7)obj).AnimTypeIndex;
        }
        #endregion

        #region LegacyEditorWrapper
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBARRRMode7 obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBARRRMode7 Obj { get; }

            public ushort Type
            {
                get => (ushort)Obj.AnimTypeIndex;
                set => Obj.AnimTypeIndex = (short)value;
            }

            public int DES
            {
                get => (ushort)Obj.AnimTypeIndex;
                set => Obj.AnimTypeIndex = (short)value;
            }

            public int ETA
            {
                get => (ushort)Obj.AnimTypeIndex;
                set => Obj.AnimTypeIndex = (short)value;
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