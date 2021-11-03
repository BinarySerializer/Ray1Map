using BinarySerializer;

namespace Ray1Map.GBA
{
    public class GBA_CaptorData : GBA_BaseBlock
    {
        public GBA_CaptorDataEntry[] DataEntries { get; set; }
        public int Length { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            DataEntries = s.SerializeObjectArray<GBA_CaptorDataEntry>(DataEntries, Length, name: nameof(DataEntries));
        }
    }
}