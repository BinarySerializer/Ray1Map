using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRRMode7 : Unity_Object
    {
        public Unity_Object_GBARRRMode7(GBARRR_Mode7Object obj, Unity_ObjectManager objManager)
        {
            Object = obj;
            ObjManager = objManager;
        }

        public GBARRR_Mode7Object Object { get; }
        public Unity_ObjectManager ObjManager { get; }

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

        public override string PrimaryName => $"Unknown";
        public override string SecondaryName => $"Unknown";

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;
        protected override bool IsUIStateArrayUpToDate => false;
        protected override void RecalculateUIStates() => UIStates = new UIState[0];
    }
}