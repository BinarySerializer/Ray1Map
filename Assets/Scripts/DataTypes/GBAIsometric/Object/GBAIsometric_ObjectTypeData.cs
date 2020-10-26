namespace R1Engine
{
    public class GBAIsometric_ObjectTypeData : R1Serializable
    {
        public Pointer<GBAIsometric_AnimSet> AnimSetPointer { get; set; }

        public Pointer InitFunctionPointer { get; set; }
        public Pointer FunctionPointer1 { get; set; }
        public Pointer FunctionPointer2 { get; set; } // For Lums this handles you collecting them
        public Pointer FunctionPointer3 { get; set; }
        public Pointer FunctionPointer4 { get; set; }

        public byte[] UnkData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimSetPointer = s.SerializePointer<GBAIsometric_AnimSet>(AnimSetPointer, resolve: true, name: nameof(AnimSetPointer));

            InitFunctionPointer = s.SerializePointer(InitFunctionPointer, name: nameof(InitFunctionPointer));
            FunctionPointer1 = s.SerializePointer(FunctionPointer1, name: nameof(FunctionPointer1));
            FunctionPointer2 = s.SerializePointer(FunctionPointer2, name: nameof(FunctionPointer2));
            FunctionPointer3 = s.SerializePointer(FunctionPointer3, name: nameof(FunctionPointer3));
            FunctionPointer4 = s.SerializePointer(FunctionPointer4, name: nameof(FunctionPointer4));

            UnkData = s.SerializeArray<byte>(UnkData, 4, name: nameof(UnkData));
        }
    }
}