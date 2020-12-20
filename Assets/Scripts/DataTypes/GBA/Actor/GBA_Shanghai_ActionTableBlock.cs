namespace R1Engine
{
    public class GBA_Shanghai_ActionTableBlock : GBA_BaseBlock
    {
        public GBC_ActionTable ActionTable { get; set; }
        public GBA_BatmanVengeance_Puppet Puppet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ActionTable = s.SerializeObject<GBC_ActionTable>(ActionTable, name: nameof(ActionTable));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            Puppet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_BatmanVengeance_Puppet>(Puppet, name: $"{nameof(Puppet)}"));
        }
    }
}