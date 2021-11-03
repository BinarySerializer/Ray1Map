using System;
using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class Unity_Object_GBAVVIsometric_TargetObj : Unity_Object_BaseGBAVVIsometric
    {
        public Unity_Object_GBAVVIsometric_TargetObj(GBAVV_Isometric_TargetObject obj, Unity_ObjectManager_GBAVVIsometric objManager, int linkIndex) : base(objManager)
        {
            Object = obj;
            LinkIndex = linkIndex;
            FlipHorizontally = Object.XPos.Value - Object.TargetXPos.Value != 0;
        }

        public int LinkIndex { get; }

        public override void UpdateAnimIndex() => ObjAnimIndex = Object.ObjType == GBAVV_Isometric_TargetObject.GBAVV_Isometric_TargetObjType.Barrel ? 22 : 23;

        public GBAVV_Isometric_TargetObject Object { get; }

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

        public override bool FlipHorizontally { get; }

        public override BinarySerializable SerializableData => Object;

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