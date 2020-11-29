namespace R1Engine
{
    public class GBC_ActionTable : GBC_BaseBlock {
        public ushort Unknown1Offset { get; set; }
        public ushort Unknown2Offset { get; set; }
        public byte ActionsCount { get; set; }
        public GBC_Action[] Actions { get; set; } // States
        public ushort[] Data1Offsets { get; set; }
        public GBC_ActionData1[] ActionData1 { get; set; } // TODO: What is this?
        public GBC_ActionData2[] ActionData2 { get; set; } // TODO: What is this?
        public GBC_Puppet Puppet { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            s.DoEndian(R1Engine.Serialize.BinaryFile.Endian.Little, () => {
                Unknown1Offset = s.Serialize<ushort>(Unknown1Offset, name: nameof(Unknown1Offset));
                Unknown2Offset = s.Serialize<ushort>(Unknown2Offset, name: nameof(Unknown2Offset));
                ActionsCount = s.Serialize<byte>(ActionsCount, name: nameof(ActionsCount));
                Actions = s.SerializeObjectArray<GBC_Action>(Actions, ActionsCount, name: nameof(Actions));
                s.DoAt(BlockStartPointer + Unknown1Offset, () => {
                    Data1Offsets = s.SerializeArray<ushort>(Data1Offsets, ActionsCount, name: nameof(Data1Offsets));
                    if(ActionData1 == null) ActionData1 = new GBC_ActionData1[ActionsCount];
                    for (int i = 0; i < Data1Offsets.Length; i++) {
                        if (Data1Offsets[i] != 0) {
                            s.DoAt(BlockStartPointer + Data1Offsets[i], () => {
                                ActionData1[i] = s.SerializeObject<GBC_ActionData1>(ActionData1[i], name: $"{nameof(ActionData1)}[{i}]");
                            });
                        }
                    }
                });
                s.DoAt(BlockStartPointer + Unknown2Offset, () => {
                    ActionData2 = s.SerializeObjectArray<GBC_ActionData2>(ActionData2, ActionsCount, name: nameof(ActionData2));
                });
                s.Goto(BlockStartPointer + BlockSize);
            });
            Puppet = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_Puppet>(Puppet, name: $"{nameof(Puppet)}"));
        }
    }
}