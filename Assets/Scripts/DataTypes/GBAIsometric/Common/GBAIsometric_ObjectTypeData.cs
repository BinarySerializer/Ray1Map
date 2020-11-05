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

        public byte[] UnkData { get; set; }

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

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
                UnkData = s.SerializeArray<byte>(UnkData, 4, name: nameof(UnkData));
        }
    }
}