using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Frogger_AdditionalLevelInfo : BinarySerializable
    {
        public bool SerializeData1 { get; set; } // Set before serializing
        public bool SerializeData2 { get; set; } // Set before serializing

        public int Int_00 { get; set; }
        public int Int_04 { get; set; }
        public int Int_08 { get; set; }
        public Pointer LevelDataPointer { get; set; }

        // Serialized from pointers
        public GBAVV_Frogger_AdditionalLevelData LevelData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
            LevelDataPointer = s.SerializePointer(LevelDataPointer, name: nameof(LevelDataPointer));

            LevelData = s.DoAt(LevelDataPointer, () => s.SerializeObject<GBAVV_Frogger_AdditionalLevelData>(LevelData, x =>
            {
                x.SerializeData1 = SerializeData1;
                x.SerializeData2 = SerializeData2;
            }, name: nameof(LevelData)));
        }
    }
}