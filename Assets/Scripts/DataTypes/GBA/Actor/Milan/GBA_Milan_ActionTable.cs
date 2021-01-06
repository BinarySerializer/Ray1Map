namespace R1Engine
{
    public class GBA_Milan_ActionTable : GBA_BaseBlock
    {
        // Parsed from offsets
        public GBA_Milan_Action[] Actions { get; set; }

        public override void SerializeBlock(SerializerObject s) { }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (Actions == null)
                Actions = new GBA_Milan_Action[OffsetTable.OffsetsCount];

            for (int i = 0; i < Actions.Length; i++)
                Actions[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<GBA_Milan_Action>(Actions[i], name: $"{nameof(Actions)}[{i}]"));
        }
    }
}