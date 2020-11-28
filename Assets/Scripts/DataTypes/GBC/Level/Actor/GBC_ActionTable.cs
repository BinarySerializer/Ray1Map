namespace R1Engine
{
    public class GBC_ActionTable : GBC_BaseBlock {
        public ushort ActionTableDataOffset { get; set; }
        public GBC_Action[] Actions { get; set; } // States
        public byte[] ActionTableData { get; set; } // TODO: What is this?
        public GBC_PuppetData Puppet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            s.DoEndian(R1Engine.Serialize.BinaryFile.Endian.Little, () => {
                ActionTableDataOffset = s.Serialize<ushort>(ActionTableDataOffset, name: nameof(ActionTableDataOffset));
                Actions = s.SerializeObjectArray<GBC_Action>(Actions, (ActionTableDataOffset - 2) / 3, name: nameof(Actions));
                s.DoAt(BlockStartPointer + ActionTableDataOffset, () => {
                    ActionTableData = s.SerializeArray<byte>(ActionTableData, BlockSize - ActionTableDataOffset, name: nameof(ActionTableData));
                });
                s.Goto(BlockStartPointer + BlockSize);
            });
            Puppet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_PuppetData>(Puppet, name: $"{nameof(Puppet)}"));
        }
    }
}