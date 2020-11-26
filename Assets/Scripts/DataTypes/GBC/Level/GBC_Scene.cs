namespace R1Engine
{
    public class GBC_Scene : GBC_BaseBlock
    {
        public byte[] UnkData0 { get; set; }
        public byte ExitsCount { get; set; }
        public byte[] UnkData1 { get; set; } // This data is identical across GBC and PalmOS, so most likely no 16/32-bit values
        public byte[] Exits { get; set; } // exit block index = exit + 1
        
        // Parsed
        public GBC_PlayField PlayField { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // TODO: Parse data
            UnkData0 = s.SerializeArray<byte>(UnkData0, 4, name: nameof(UnkData0));
            ExitsCount = s.Serialize<byte>(ExitsCount, name: nameof(ExitsCount));
            UnkData1 = s.SerializeArray<byte>(UnkData1, 61, name: nameof(UnkData0));
            Exits = s.SerializeArray<byte>(Exits, ExitsCount, name: nameof(Exits));

            // TODO: Parse remaining referenced data. There are references to other scenes (next level/bonus level), with the index in the actor, and on GBC there can be a reference to the GBC_VignetteList struct.
            // Parse data from pointers
            PlayField = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_PlayField>(PlayField, name: nameof(PlayField)));
        }
    }
}