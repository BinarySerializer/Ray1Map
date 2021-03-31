using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_Frogger_AdditionalLevelData : BinarySerializable
    {
        public bool SerializeData1 { get; set; } // Set before serializing
        public bool SerializeData2 { get; set; } // Set before serializing

        public int Int_00 { get; set; }
        public int MemoryPointer_04 { get; set; }
        public Pointer MapInfo1Pointer { get; set; }
        public Pointer MapInfo2Pointer { get; set; }

        // Serialized from pointers
        public GBAVV_Generic_MapInfo MapInfo1 { get; set; }
        public GBAVV_Generic_MapInfo MapInfo2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            MemoryPointer_04 = s.Serialize<int>(MemoryPointer_04, name: nameof(MemoryPointer_04));
            MapInfo1Pointer = s.SerializePointer(MapInfo1Pointer, name: nameof(MapInfo1Pointer));
            MapInfo2Pointer = s.SerializePointer(MapInfo2Pointer, name: nameof(MapInfo2Pointer));

            MapInfo1 = s.DoAt(MapInfo1Pointer, () => s.SerializeObject<GBAVV_Generic_MapInfo>(MapInfo1, x => x.SerializeData = SerializeData1, name: nameof(MapInfo1)));
            MapInfo2 = s.DoAt(MapInfo2Pointer, () => s.SerializeObject<GBAVV_Generic_MapInfo>(MapInfo2, x => x.SerializeData = SerializeData2, name: nameof(MapInfo2)));
        }
    }
}