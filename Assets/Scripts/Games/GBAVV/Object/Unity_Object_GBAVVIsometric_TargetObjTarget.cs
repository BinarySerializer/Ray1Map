using System;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class Unity_Object_GBAVVIsometric_TargetObjTarget : Unity_Object_BaseGBAVVIsometric
    {
        public Unity_Object_GBAVVIsometric_TargetObjTarget(GBAVV_Isometric_TargetObject obj, Unity_ObjectManager_GBAVVIsometric objManager) : base(objManager)
        {
            Object = obj;
        }

        public override void UpdateAnimIndex() => ObjAnimIndex = -1;

        public GBAVV_Isometric_TargetObject Object { get; }

        public override string DebugText => String.Empty;

        public override FixedPointInt32 XPos
        {
            get => Object.TargetXPos;
            set => Object.TargetXPos = value;
        }
        public override FixedPointInt32 YPos
        {
            get => Object.TargetYPos;
            set => Object.TargetYPos = value;
        }

        public override BinarySerializable SerializableData => Object;

        public override string PrimaryName => $"Type_10_{(int)Object.ObjType}";
        public override string SecondaryName => $"{Object.ObjType} Target";

        public override Unity_ObjectType Type => Unity_ObjectType.Waypoint;
        public override bool IsEditor => true;

        protected override void RecalculateUIStates()
        {
            UIStates_HasInitialized = true;
            UIStates = new UIState[0];
        }
    }
}