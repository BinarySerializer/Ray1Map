namespace R1Engine
{
    public class GBAIsometric_ObjectType : R1Serializable
    {
        public uint ObjFlags { get; set; }
        public Pointer<GBAIsometric_ObjectTypeData> DataPointer { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjFlags = s.Serialize<uint>(ObjFlags, name: nameof(ObjFlags));
            DataPointer = s.SerializePointer<GBAIsometric_ObjectTypeData>(DataPointer, resolve: true, name: nameof(DataPointer));
        }
    }
}