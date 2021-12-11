using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_LevelInfo : BinarySerializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer MapDataPointer { get; set; }
        public Pointer MusicPointer { get; set; } // TODO: Serialize a GAX2 song from here

        // Serialized from pointers
        public GBAVV_NitroKart_MapData MapData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
            MusicPointer = s.SerializePointer(MusicPointer, name: nameof(MusicPointer));

            if (SerializeData)
                MapData = s.DoAt(MapDataPointer, () => s.SerializeObject<GBAVV_NitroKart_MapData>(MapData, name: nameof(MapData)));
        }
    }
}