using System;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class Unity_Object_GBAVVIsometric_Obj : Unity_Object_BaseGBAVVIsometric
    {
        public Unity_Object_GBAVVIsometric_Obj(GBAVV_Isometric_Object obj, Unity_ObjectManager_GBAVVIsometric objManager) : base(objManager)
        {
            Object = obj;
        }

        public override void UpdateAnimIndex() => ObjAnimIndex = (int)(Settings.GBAVV_Crash_TimeTrialMode && Object.ObjType_TimeTrial != GBAVV_Isometric_Object.GBAVV_Isometric_ObjType.None ? Object.ObjType_TimeTrial : Object.ObjType);

        public GBAVV_Isometric_Object Object { get; }

        public override string DebugText => $"ObjType_TimeTrial: {Object.ObjType_TimeTrial}{Environment.NewLine}" +
                                            $"Height: {Height}{Environment.NewLine}" +
                                            $"AnimX: {GraphicsData?.CrashAnim?.XPos?.AsFloat}{Environment.NewLine}" +
                                            $"AnimY: {GraphicsData?.CrashAnim?.YPos?.AsFloat}{Environment.NewLine}";

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

        public override string PrimaryName => $"Type_0C_{(int)Object.ObjType}";
        public override string SecondaryName => $"{Object.ObjType}";
    }
}