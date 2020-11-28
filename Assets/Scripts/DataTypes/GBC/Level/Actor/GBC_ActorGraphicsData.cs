namespace R1Engine
{
    public class GBC_ActorGraphicsData : GBC_BaseBlock
    {
        public byte[] Data { get; set; }

        public GBC_ActionTable ActionTable { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, 11, name: nameof(Data));
            ActionTable = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_ActionTable>(ActionTable, name: $"{nameof(ActionTable)}"));
        }
    }
}