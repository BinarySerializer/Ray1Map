namespace R1Engine
{
    public class GBA_Milan_ActorModel : GBA_BaseBlock
    {
        public string ModelIdentifier { get; set; }

        // Parsed from offsets
        public GBA_Milan_BasePuppet BasePuppet { get; set; }
        public GBA_Milan_ActionTable ActionTable { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ModelIdentifier = s.SerializeString(ModelIdentifier, length: 12, name: nameof(ModelIdentifier));
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            BasePuppet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Milan_BasePuppet>(BasePuppet, name: nameof(BasePuppet)));
            ActionTable = s.DoAt(OffsetTable.GetPointer(1), () => s.SerializeObject<GBA_Milan_ActionTable>(ActionTable, name: nameof(ActionTable)));
        }
    }
}