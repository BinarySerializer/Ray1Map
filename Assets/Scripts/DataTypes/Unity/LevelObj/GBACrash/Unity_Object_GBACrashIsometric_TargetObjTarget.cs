using System;

namespace R1Engine
{
    public class Unity_Object_GBACrashIsometric_TargetObjTarget : Unity_Object_BaseGBACrashIsometric
    {
        public Unity_Object_GBACrashIsometric_TargetObjTarget(GBACrash_Isometric_TargetObject obj, Unity_ObjectManager_GBACrashIsometric objManager) : base(objManager)
        {
            Object = obj;
        }

        public override void UpdateAnimIndex() => ObjAnimIndex = -1;

        public GBACrash_Isometric_TargetObject Object { get; }

        public override string DebugText => String.Empty;

        public override FixedPointInt XPos
        {
            get => Object.TargetXPos;
            set => Object.TargetXPos = value;
        }
        public override FixedPointInt YPos
        {
            get => Object.TargetYPos;
            set => Object.TargetYPos = value;
        }

        public override R1Serializable SerializableData => Object;

        public override string PrimaryName => $"Type_10_{(int)Object.ObjType}";
        public override string SecondaryName => $"{Object.ObjType} Target";

        public override ObjectType Type => ObjectType.Waypoint;
        public override bool IsEditor => true;
    }
}