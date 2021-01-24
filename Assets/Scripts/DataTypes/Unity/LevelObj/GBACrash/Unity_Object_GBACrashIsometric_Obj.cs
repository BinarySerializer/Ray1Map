using System;

namespace R1Engine
{
    public class Unity_Object_GBACrashIsometric_Obj : Unity_Object_BaseGBACrashIsometric
    {
        public Unity_Object_GBACrashIsometric_Obj(GBACrash_Isometric_Object obj, Unity_ObjectManager_GBACrashIsometric objManager) : base(objManager)
        {
            Object = obj;
        }

        public override void UpdateAnimIndex() => ObjAnimIndex = (int)(Settings.GBACrash_TimeTrialMode && Object.ObjType_TimeTrial != GBACrash_Isometric_Object.GBACrash_Isometric_ObjType.None ? Object.ObjType_TimeTrial : Object.ObjType);

        public GBACrash_Isometric_Object Object { get; }

        public override string DebugText => $"ObjType_TimeTrial: {Object.ObjType_TimeTrial}{Environment.NewLine}" +
                                            $"Height: {Height}{Environment.NewLine}" +
                                            $"AnimX: {GraphicsData?.CrashAnim?.XPos?.AsFloat}{Environment.NewLine}" +
                                            $"AnimY: {GraphicsData?.CrashAnim?.YPos?.AsFloat}{Environment.NewLine}";

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

        public override R1Serializable SerializableData => Object;

        public override string PrimaryName => $"Type_0C_{(int)Object.ObjType}";
        public override string SecondaryName => $"{Object.ObjType}";
    }
}