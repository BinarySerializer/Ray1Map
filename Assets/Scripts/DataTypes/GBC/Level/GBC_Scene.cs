namespace R1Engine
{
    public class GBC_Scene : GBC_BaseBlock
    {
        public byte[] UnkData { get; set; } // This data is identical across GBC and PalmOS, so most likely no 16/32-bit values

        // Parsed
        public GBC_PlayField PlayField { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            // TODO: Parse data
            UnkData = s.SerializeArray<byte>(UnkData, 68, name: nameof(UnkData));

            // TODO: Parse remaining referenced data. There are references to other scenes (next level/bonus level), with the index in the actor, and on GBC there can be a reference to the GBC_VignetteList struct.
            // Parse data from pointers
            PlayField = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_PlayField>(PlayField, name: nameof(PlayField)));
        }
    }
}