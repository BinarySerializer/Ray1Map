using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_PowerpuffGirls_LevelInfo : BinarySerializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer MapPointer { get; set; }
        public int Int_04 { get; set; }

        // Serialized from pointers
        public GBAVV_Map Map { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapPointer = s.SerializePointer(MapPointer, name: nameof(MapPointer));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));

            if (SerializeData)
                Map = s.DoAt(MapPointer, () => s.SerializeObject<GBAVV_Map>(Map, name: nameof(Map)));
        }
    }
}