using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRRMode7Waypoint : Unity_Object
    {
        public Unity_Object_GBARRRMode7Waypoint(GBARRR_Mode7Waypoint obj, Unity_ObjectManager objManager)
        {
            Object = obj;
            ObjManager = objManager;

            // Default to no entries
            UIStates = new UIState[0];
        }

        public GBARRR_Mode7Waypoint Object { get; }
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

		public override ObjectType Type => ObjectType.Waypoint;
		public override bool IsEditor => true;

		public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => null;

        public override string PrimaryName => $"Waypoint";
        public override string SecondaryName => null;

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;
        protected override bool IsUIStateArrayUpToDate => true;
        protected override void RecalculateUIStates() { }
    }
}