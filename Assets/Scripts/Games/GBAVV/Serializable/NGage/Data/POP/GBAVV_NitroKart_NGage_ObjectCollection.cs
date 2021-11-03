using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_NGage_ObjectCollection : BinarySerializable
    {
        public Pointer ObjectsPointer { get; set; }
        public int ObjectsCount { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            ObjectsCount = s.Serialize<int>(ObjectsCount, name: nameof(ObjectsCount));

            Objects = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<GBAVV_NitroKart_Object>(Objects, ObjectsCount, name: nameof(Objects)));
        }
    }
}