namespace R1Engine
{
    public class GBA_Milan_BasePuppet : GBA_BaseBlock
    {
        // Parsed from offsets
        public GBA_Puppet Puppet { get; set; }

        public override void SerializeBlock(SerializerObject s) { }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Puppet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Puppet>(Puppet, name: nameof(Puppet)));
        }
    }
}