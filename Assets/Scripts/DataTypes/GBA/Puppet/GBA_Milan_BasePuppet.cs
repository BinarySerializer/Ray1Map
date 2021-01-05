namespace R1Engine
{
    public class GBA_Milan_BasePuppet : GBA_BaseBlock
    {
        // Parsed from offsets
        public GBA_Milan_Puppet Puppet { get; set; }

        public override void SerializeBlock(SerializerObject s) { }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Puppet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Milan_Puppet>(Puppet, name: nameof(Puppet)));
            // Block 1 has some data
        }
    }
}