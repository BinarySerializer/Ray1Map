namespace R1Engine
{
    public class GBC_Scene : GBC_BaseBlock
    {
        public GBC_Pointer[] Pointers { get; set; }
        public byte[] UnkData { get; set; } // This data is identical across GBC and PalmOS, so most likely no 16/32-bit values

        // Parsed
        public GBC_PlayField PlayField { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            base.SerializeImpl(s);
            SerializeOffsetTable(s);

            UnkData = s.SerializeArray<byte>(UnkData, 68, name: nameof(UnkData));

            // Parse data from pointers
            // TODO: figure out format
            PlayField = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_PlayField>(PlayField, name: nameof(PlayField)));
        }
    }
}