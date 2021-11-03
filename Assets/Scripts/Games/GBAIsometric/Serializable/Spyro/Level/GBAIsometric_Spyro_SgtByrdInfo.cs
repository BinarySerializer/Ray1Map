using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_SgtByrdInfo : BinarySerializable
    {
        public ushort LevelID { get; set; }
        public ushort Ushort_02 { get; set; } // Always 2
        public GBAIsometric_Spyro_DataBlockIndex ObjectTableIndex { get; set; }
        public ushort LevelDataID { get; set; }
        public ushort Ushort_08 { get; set; }
        public ushort Ushort_0A { get; set; }

        // Parsed
        public GBAIsometric_Spyro_ObjectTable ObjectTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LevelID = s.Serialize<ushort>(LevelID, name: nameof(LevelID));
            Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
            ObjectTableIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(ObjectTableIndex, name: nameof(ObjectTableIndex));
            LevelDataID = s.Serialize<ushort>(LevelDataID, name: nameof(LevelDataID));
            Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
            Ushort_0A = s.Serialize<ushort>(Ushort_0A, name: nameof(Ushort_0A));

            ObjectTable = ObjectTableIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_ObjectTable>(ObjectTable, name: nameof(ObjectTable)));
        }
    }
}