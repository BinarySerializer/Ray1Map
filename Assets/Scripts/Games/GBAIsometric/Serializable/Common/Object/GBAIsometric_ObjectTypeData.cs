using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_ObjectTypeData : BinarySerializable
    {
        public Pointer<GBAIsometric_RHR_AnimSet> AnimSetPointer { get; set; }
        public uint ObjectMemorySize { get; set; } // The size of the object struct in memory (the first few properties are always the same, but then it varies between object types how it's filled out)

        public Pointer InitFunctionPointer { get; set; }
        public Pointer FunctionPointer1 { get; set; } // Spyro3: Unloads some anim blocks
        public Pointer FunctionPointer2 { get; set; } // RHR: For Lums this handles you collecting them
        public Pointer FunctionPointer3 { get; set; } // Spyro3: Draws the object frames (obj + child objects)
        public Pointer FunctionPointer4 { get; set; }

        public byte AnimationIndex { get; set; }
        public byte Byte_19 { get; set; }
        public ushort UShort_1A { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_RHR)
                AnimSetPointer = s.SerializePointer<GBAIsometric_RHR_AnimSet>(AnimSetPointer, name: nameof(AnimSetPointer))?.ResolveObject(s);
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                ObjectMemorySize = s.Serialize<uint>(ObjectMemorySize, name: nameof(ObjectMemorySize));

            InitFunctionPointer = s.SerializePointer(InitFunctionPointer, name: nameof(InitFunctionPointer));
            FunctionPointer1 = s.SerializePointer(FunctionPointer1, name: nameof(FunctionPointer1));
            FunctionPointer2 = s.SerializePointer(FunctionPointer2, name: nameof(FunctionPointer2));
            FunctionPointer3 = s.SerializePointer(FunctionPointer3, name: nameof(FunctionPointer3));
            FunctionPointer4 = s.SerializePointer(FunctionPointer4, name: nameof(FunctionPointer4));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_RHR) {
                AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                Byte_19 = s.Serialize<byte>(Byte_19, name: nameof(Byte_19));
                UShort_1A = s.Serialize<ushort>(UShort_1A, name: nameof(UShort_1A));
            }
        }
    }
}