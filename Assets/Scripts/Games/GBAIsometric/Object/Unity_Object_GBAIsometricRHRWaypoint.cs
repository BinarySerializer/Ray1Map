using System;
using System.Collections.Generic;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class Unity_Object_GBAIsometricRHRWaypoint : Unity_SpriteObject_3D
    {
        public Unity_Object_GBAIsometricRHRWaypoint(GBAIsometric_RHR_Waypoint waypoint, Unity_ObjectManager_GBAIsometricRHR objManager)
        {
            Waypoint = waypoint;
            ObjManager = objManager;
        }

        public override Unity_ObjectType Type => Unity_ObjectType.Waypoint;
        public GBAIsometric_RHR_Waypoint Waypoint { get; }
        public Unity_ObjectManager_GBAIsometricRHR ObjManager { get; }

        public override short XPosition
        {
            get => (short)Waypoint.XPosValue;
            set => Waypoint.XPosValue = value;
        }
        public override short YPosition
        {
            get => (short)Waypoint.YPosValue;
            set => Waypoint.YPosValue = value;
        }
		public override Vector3 Position {
            get => new Vector3(Waypoint.XPosValue, Waypoint.YPosValue, Waypoint.HeightValue);
            set {
                Waypoint.XPosValue = value.x;
                Waypoint.YPosValue = value.y;
                Waypoint.HeightValue = value.z;

            }
        }

		public override bool IsEditor => true;
        public override string DebugText => String.Empty;

        public override BinarySerializable SerializableData => Waypoint;

        public override string PrimaryName => $"Waypoint";
        public override string SecondaryName => null;

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;

        protected bool UIStates_HasInitialized { get; set; }
        protected override bool IsUIStateArrayUpToDate => UIStates_HasInitialized;
        protected override void RecalculateUIStates()
        {
            UIStates_HasInitialized = true;
            UIStates = new UIState[0];
        }
    }
}