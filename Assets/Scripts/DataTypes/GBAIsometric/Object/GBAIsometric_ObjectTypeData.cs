namespace R1Engine
{
    public class GBAIsometric_ObjectTypeData : R1Serializable
    {
        public Pointer<GBAIsometric_AnimSet> AnimSetPointer { get; set; }

        // TODO: More data - function pointers?

        public override void SerializeImpl(SerializerObject s)
        {
            AnimSetPointer = s.SerializePointer<GBAIsometric_AnimSet>(AnimSetPointer, resolve: true, name: nameof(AnimSetPointer));
        }
    }
}