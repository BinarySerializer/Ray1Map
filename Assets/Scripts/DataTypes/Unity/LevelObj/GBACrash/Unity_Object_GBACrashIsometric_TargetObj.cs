using System;
using System.Collections.Generic;

namespace R1Engine
{
    public class Unity_Object_GBACrashIsometric_TargetObj : Unity_Object_BaseGBACrashIsometric
    {
        public Unity_Object_GBACrashIsometric_TargetObj(GBACrash_Isometric_TargetObject obj, Unity_ObjectManager_GBACrashIsometric objManager, int linkIndex) : base(objManager)
        {
            Object = obj;
            LinkIndex = linkIndex;
        }

        public int LinkIndex { get; }

        public override void UpdateAnimIndex() => ObjAnimIndex = Object.ObjType == GBACrash_Isometric_TargetObject.GBACrash_Isometric_TargetObjType.Barrel ? 22 : 23;

        public GBACrash_Isometric_TargetObject Object { get; }

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

        public override R1Serializable SerializableData => Object;

        public override string PrimaryName => $"Type_10_{(int)Object.ObjType}";
        public override string SecondaryName => $"{Object.ObjType}";

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links
        {
            get
            {
                yield return LinkIndex;
            }
        }
    }
}