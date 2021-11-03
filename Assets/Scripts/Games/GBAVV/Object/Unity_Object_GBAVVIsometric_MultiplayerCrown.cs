using System;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class Unity_Object_GBAVVIsometric_MultiplayerCrown : Unity_Object_BaseGBAVVIsometric
    {
        public Unity_Object_GBAVVIsometric_MultiplayerCrown(GBAVV_Isometric_Position obj, Unity_ObjectManager_GBAVVIsometric objManager) : base(objManager)
        {
            Object = obj;
        }

        public override void UpdateAnimIndex() => ObjAnimIndex = 32;

        public GBAVV_Isometric_Position Object { get; }

        public override string DebugText => String.Empty;

        public override FixedPointInt32 XPos
        {
            get => Object.XPos;
            set => Object.XPos = value;
        }
        public override FixedPointInt32 YPos
        {
            get => Object.YPos;
            set => Object.YPos = value;
        }

        public override BinarySerializable SerializableData => Object;

        public override string PrimaryName => $"Crown";
        public override string SecondaryName => null;
    }
}