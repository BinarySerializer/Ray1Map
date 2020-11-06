using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometricWaypoint : Unity_Object
    {
        public Unity_Object_GBAIsometricWaypoint(GBAIsometric_RHR_Waypoint waypoint, Unity_ObjectManager_GBAIsometric objManager)
        {
            Waypoint = waypoint;
            ObjManager = objManager;
        }

        public GBAIsometric_RHR_Waypoint Waypoint { get; }
        public Unity_ObjectManager_GBAIsometric ObjManager { get; }

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

        public override bool IsEditor => true;
        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => Waypoint;
        public override ILegacyEditorWrapper LegacyWrapper { get; }

        public override string PrimaryName => $"waypoint";
        public override string SecondaryName => null;

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;
        protected override bool IsUIStateArrayUpToDate => false;
        protected override void RecalculateUIStates() => UIStates = new UIState[0];
    }
}