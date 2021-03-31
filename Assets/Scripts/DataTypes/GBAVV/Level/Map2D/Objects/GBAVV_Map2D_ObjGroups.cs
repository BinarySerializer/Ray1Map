using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Map2D_ObjGroups : BinarySerializable
    {
        public ushort Ushort_00 { get; set; }
        public ushort ObjectsCount { get; set; }
        public Pointer ObjectsPointer { get; set; }

        // Serialized from pointers
        public GBAVV_Map2D_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            ObjectsCount = s.Serialize<ushort>(ObjectsCount, name: nameof(ObjectsCount));
            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));

            Objects = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<GBAVV_Map2D_Object>(Objects, ObjectsCount, name: nameof(Objects)));
        }
    }
}