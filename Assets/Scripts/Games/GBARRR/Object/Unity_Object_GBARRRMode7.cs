using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class Unity_Object_GBARRRMode7 : Unity_SpriteObject_3D
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

        public float Height { get; set; }
		public override float Scale => IsRayman ? 0.25f : 0.5f;

        public override Vector3 Position
        {
            get => new Vector3(Object.XPosition, Object.YPosition, Height);
            set
            {
                Object.XPosition = (short)value.x;
                Object.YPosition = (short)value.y;
                Height = value.z;
            }
        }

        public bool ForceNoGraphics { get; }
        public bool IsRayman => Object.ObjectType == GBARRR_Mode7Object.Mode7Type.Rayman && !ForceNoGraphics;

        public override string DebugText => String.Empty;

        public override BinarySerializable SerializableData => Object;
        public override BaseLegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{AnimTypeIndex}";
        public override string SecondaryName => (!IsRayman && Object.ObjectType == GBARRR_Mode7Object.Mode7Type.Rayman) ? "Removed" : $"{Object.ObjectType}";

        public Unity_ObjectManager_GBARRRMode7.GraphicsData GraphicsData => ForceNoGraphics ? null : ObjManager.GraphicsDatas.ElementAtOrDefault(AnimTypeIndex);

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Animation;
        public override int AnimSpeed => GraphicsData?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimTypeIndex;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => GraphicsData?.AnimFrames;


        #region UI States
        protected bool UIStates_HasInitialized { get; set; }
        protected override bool IsUIStateArrayUpToDate => UIStates_HasInitialized;

        protected override void RecalculateUIStates() {
            UIStates_HasInitialized = true;
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
        private class LegacyEditorWrapper : BaseLegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBARRRMode7 obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBARRRMode7 Obj { get; }

            public override ushort Type
            {
                get => (ushort)Obj.AnimTypeIndex;
                set => Obj.AnimTypeIndex = (short)value;
            }

            public override int DES
            {
                get => (ushort)Obj.AnimTypeIndex;
                set => Obj.AnimTypeIndex = (short)value;
            }

            public override int ETA
            {
                get => (ushort)Obj.AnimTypeIndex;
                set => Obj.AnimTypeIndex = (short)value;
            }
        }
        #endregion
    }
}