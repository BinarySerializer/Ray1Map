using System;
using BinarySerializer;

namespace R1Engine
{
    public class Unity_Object_GBAVVIsometric_MultiplayerFlag : Unity_Object_BaseGBAVVIsometric
    {
        public Unity_Object_GBAVVIsometric_MultiplayerFlag(GBAVV_Isometric_Position obj, Unity_ObjectManager_GBAVVIsometric objManager) : base(objManager)
        {
            Object = obj;
        }

        public override void UpdateAnimIndex() => ObjAnimIndex = 31;

        public GBAVV_Isometric_Position Object { get; }

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

        public override string PrimaryName => $"Flag";
        public override string SecondaryName => null;
    }
}