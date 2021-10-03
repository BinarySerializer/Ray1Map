using System;
using System.Collections.Generic;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRRMode7Waypoint : Unity_SpriteObject_3D
    {
        public Unity_Object_GBARRRMode7Waypoint(GBARRR_Mode7Waypoint obj, Unity_ObjectManager objManager, int linkedWayPointIndex)
        {
            Object = obj;
            ObjManager = objManager;
            LinkedWayPointIndex = linkedWayPointIndex;

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

        public float Height { get; set; }
        public override float Scale => 0.5f;

        public int LinkedWayPointIndex { get; }

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links
        {
            get
            {
                yield return LinkedWayPointIndex;
            }
        }

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

        public override Unity_ObjectType Type => Unity_ObjectType.Waypoint;
		public override bool IsEditor => true;

		public override string DebugText => String.Empty;

        public override BinarySerializable SerializableData => Object;

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