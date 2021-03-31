using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_ObjectType : BinarySerializable
    {
        public uint ObjFlags { get; set; }
        public Pointer DataPointer { get; set; }

        public GBAIsometric_ObjectTypeData Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjFlags = s.Serialize<uint>(ObjFlags, name: nameof(ObjFlags));
            DataPointer = s.SerializePointer(DataPointer, name: nameof(DataPointer));

            Data = s.DoAt(DataPointer, () => s.SerializeObject<GBAIsometric_ObjectTypeData>(Data, name: nameof(Data)));
        }
    }
}