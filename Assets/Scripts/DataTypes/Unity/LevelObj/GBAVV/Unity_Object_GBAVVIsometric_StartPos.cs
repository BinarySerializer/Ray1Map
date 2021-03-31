using System;
using BinarySerializer;

namespace R1Engine
{
    public class Unity_Object_GBAVVIsometric_StartPos : Unity_Object_BaseGBAVVIsometric
    {
        public Unity_Object_GBAVVIsometric_StartPos(GBAVV_Isometric_Position obj, int playerIndex, Unity_ObjectManager_GBAVVIsometric objManager) : base(objManager)
        {
            Object = obj;
            PlayerIndex = playerIndex;
        }

        public override void UpdateAnimIndex() => ObjAnimIndex = -1;

        public GBAVV_Isometric_Position Object { get; }
        public int PlayerIndex { get; }

        public override string DebugText => String.Empty;

        public override FixedPointInt XPos
        {
            get => Object.XPos;
            set => Object.XPos = value;
        }
        public override FixedPointInt YPos
        {
            get => Object.YPos;
            set => Object.YPos = value;
        }

        public override BinarySerializable SerializableData => Object;

        public override string PrimaryName => $"Player_{PlayerIndex + 1}";
        public override string SecondaryName => $"Start Position";

        public override ObjectType Type => ObjectType.Waypoint;
        public override bool IsEditor => true;

        protected override void RecalculateUIStates()
        {
            UIStates_HasInitialized = true;
            UIStates = new UIState[0];
        }
    }
}