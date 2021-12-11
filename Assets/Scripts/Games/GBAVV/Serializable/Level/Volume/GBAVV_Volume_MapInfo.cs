using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Volume_MapInfo : BinarySerializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer MapPointer { get; set; }
        public Pointer Pointer_04 { get; set; } // TODO: GAX2 music?
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }

        // Serialized from pointers
        public GBAVV_Map Map { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapPointer = s.SerializePointer(MapPointer, name: nameof(MapPointer));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpiderMan3)
            {
                Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
                Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
            }

            if (SerializeData)
                Map = s.DoAt(MapPointer, () => s.SerializeObject<GBAVV_Map>(Map, name: nameof(Map)));
        }
    }
}