﻿namespace R1Engine
{
    public class GBACrash_Object : R1Serializable
    {
        public GBACrash_ObjType ObjType { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public ushort ObjParamsIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjType = s.Serialize<GBACrash_ObjType>(ObjType, name: nameof(ObjType));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            ObjParamsIndex = s.Serialize<ushort>(ObjParamsIndex, name: nameof(ObjParamsIndex));
        }
    }
}