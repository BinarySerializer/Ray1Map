namespace R1Engine
{
    public class GBA_Milan_ActorModel : GBA_BaseBlock
    {
        public string ActorID { get; set; }

        // Parsed from offsets
        public GBA_Milan_BasePuppet BasePuppet { get; set; }
        public GBA_Milan_ActionTable ActionTable { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ActorID = s.SerializeString(ActorID, length: 4, name: nameof(ActorID));
            s.SerializeArray<byte>(new byte[8], 8, name: "Padding");
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            BasePuppet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBA_Milan_BasePuppet>(BasePuppet, name: nameof(BasePuppet)));
            ActionTable = s.DoAt(OffsetTable.GetPointer(1), () => s.SerializeObject<GBA_Milan_ActionTable>(ActionTable, name: nameof(ActionTable)));
        }
    }
}