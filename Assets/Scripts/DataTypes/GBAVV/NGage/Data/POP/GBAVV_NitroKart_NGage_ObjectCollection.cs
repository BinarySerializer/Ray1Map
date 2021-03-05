namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_ObjectCollection : R1Serializable
    {
        public Pointer ObjectsPointer { get; set; }
        public int ObjectsCount { get; set; }

        // Serialized from pointers
        // TODO: Objects (36 bytes each)

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));
            ObjectsCount = s.Serialize<int>(ObjectsCount, name: nameof(ObjectsCount));
        }
    }
}