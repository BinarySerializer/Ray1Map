using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRRMode7 : Unity_Object
    {
        public Unity_Object_GBARRRMode7(GBARRR_Mode7Object obj, Unity_ObjectManager_GBARRRMode7 objManager)
        {
            Object = obj;
            ObjManager = objManager;
        }

        public GBARRR_Mode7Object Object { get; }
        public Unity_ObjectManager_GBARRRMode7 ObjManager { get; }

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

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper { get; }

        public override string PrimaryName => $"Type_{(int)Object.ObjectType}";
        public override string SecondaryName => $"{Object.ObjectType}";

        public Unity_ObjectManager_GBARRRMode7.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault((int)Object.ObjectType);

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Animation;
        public override int AnimSpeed => GraphicsData?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => (int)Object.ObjectType;
        public override IList<Sprite> Sprites => GraphicsData?.AnimFrames;

        protected override bool IsUIStateArrayUpToDate => false;
        protected override void RecalculateUIStates() => UIStates = new UIState[0];
    }
}