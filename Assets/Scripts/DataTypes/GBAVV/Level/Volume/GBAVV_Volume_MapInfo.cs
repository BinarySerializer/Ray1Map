namespace R1Engine
{
    public class GBAVV_Volume_MapInfo : R1Serializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer MapPointer { get; set; }
        public Pointer Pointer_04 { get; set; }
        public int Int_08 { get; set; }

        // Serialized from pointers
        public GBAVV_Map Map { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapPointer = s.SerializePointer(MapPointer, name: nameof(MapPointer));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));

            if (SerializeData)
                Map = s.DoAt(MapPointer, () => s.SerializeObject<GBAVV_Map>(Map, name: nameof(Map)));
        }
    }
}