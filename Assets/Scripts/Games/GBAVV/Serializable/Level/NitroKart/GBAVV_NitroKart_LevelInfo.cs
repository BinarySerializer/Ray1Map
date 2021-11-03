using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_LevelInfo : BinarySerializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer MapDataPointer { get; set; }
        public Pointer Pointer_04 { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_MapData MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));

            if (SerializeData)
                MapData = s.DoAt(MapDataPointer, () => s.SerializeObject<GBAVV_NitroKart_MapData>(MapData, name: nameof(MapData)));
        }
    }
}