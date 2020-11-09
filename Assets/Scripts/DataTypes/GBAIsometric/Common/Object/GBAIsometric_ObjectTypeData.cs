namespace R1Engine
{
    public class GBAIsometric_ObjectTypeData : R1Serializable
    {
        public Pointer<GBAIsometric_RHR_AnimSet> AnimSetPointer { get; set; }
        public uint Spyro_Uint00 { get; set; }

        public Pointer InitFunctionPointer { get; set; }
        public Pointer FunctionPointer1 { get; set; }
        public Pointer FunctionPointer2 { get; set; } // For Lums this handles you collecting them
        public Pointer FunctionPointer3 { get; set; }
        public Pointer FunctionPointer4 { get; set; }

        public byte AnimationIndex { get; set; }
        public byte Byte_19 { get; set; }
        public ushort UShort_1A { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
                AnimSetPointer = s.SerializePointer<GBAIsometric_RHR_AnimSet>(AnimSetPointer, resolve: true, name: nameof(AnimSetPointer));
            else
                Spyro_Uint00 = s.Serialize<uint>(Spyro_Uint00, name: nameof(Spyro_Uint00));

            InitFunctionPointer = s.SerializePointer(InitFunctionPointer, name: nameof(InitFunctionPointer));
            FunctionPointer1 = s.SerializePointer(FunctionPointer1, name: nameof(FunctionPointer1));
            FunctionPointer2 = s.SerializePointer(FunctionPointer2, name: nameof(FunctionPointer2));
            FunctionPointer3 = s.SerializePointer(FunctionPointer3, name: nameof(FunctionPointer3));
            FunctionPointer4 = s.SerializePointer(FunctionPointer4, name: nameof(FunctionPointer4));

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR) {
                AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                Byte_19 = s.Serialize<byte>(Byte_19, name: nameof(Byte_19));
                UShort_1A = s.Serialize<ushort>(UShort_1A, name: nameof(UShort_1A));
            }
        }
    }
}