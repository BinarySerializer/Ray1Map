namespace R1Engine
{
    public class GBC_PalmOS_PlayField : GBC_PalmOS_Block 
    {
        public uint UnkCount { get; set; }
        public ushort Ushort_02 { get; set; }
        public ushort Ushort_04 { get; set; }
        public ushort MapIndex { get; set; }

        // TODO: More values + structs with length UnkCount and size 8 bytes

        public override void SerializeImpl(SerializerObject s)
        {
            UnkCount = s.Serialize<uint>(UnkCount, name: nameof(UnkCount));
            Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));
            Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
            MapIndex = s.Serialize<ushort>(MapIndex, name: nameof(MapIndex));
        }
    }
}