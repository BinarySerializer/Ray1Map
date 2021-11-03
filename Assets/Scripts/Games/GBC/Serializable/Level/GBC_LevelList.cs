using BinarySerializer;

namespace Ray1Map.GBC
{
    public class GBC_LevelList : GBC_BaseBlock
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte Byte_02 { get; set; }
        public byte Index_FirstMap { get; set; }
        public byte Index_LastMap { get; set; }
        public byte Index_WorldMap { get; set; }
        public byte Index_UbiCliff { get; set; }
        public byte Index_Unknown { get; set; }

        // Parsed
        public GBC_Level Level { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Index_FirstMap = s.Serialize<byte>(Index_FirstMap, name: nameof(Index_FirstMap));
            Index_LastMap = s.Serialize<byte>(Index_LastMap, name: nameof(Index_LastMap));
            Index_WorldMap = s.Serialize<byte>(Index_WorldMap, name: nameof(Index_WorldMap));
            Index_UbiCliff = s.Serialize<byte>(Index_UbiCliff, name: nameof(Index_UbiCliff));
            Index_Unknown = s.Serialize<byte>(Index_Unknown, name: nameof(Index_Unknown));

            // Parse data from pointers
            Level = s.DoAt(DependencyTable.GetPointer(s.GetR1Settings().Level), () => s.SerializeObject<GBC_Level>(Level, name: nameof(Level)));
        }
    }
}