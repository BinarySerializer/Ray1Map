using BinarySerializer;

namespace Ray1Map.GBC
{
    public class GBC_ActionTable : BinarySerializable
    {
        public ushort Unknown1Offset { get; set; }
        public ushort Unknown2Offset { get; set; }
        public byte ActionsCount { get; set; }
        public GBC_Action[] Actions { get; set; } // States
        public ushort[] Data1Offsets { get; set; }
        public GBC_ActionData1[] ActionData1 { get; set; } // TODO: What is this?
        public GBC_ActionData2[] ActionData2 { get; set; } // TODO: What is this?

        public override void SerializeImpl(SerializerObject s)
        {
            Unknown1Offset = s.Serialize<ushort>(Unknown1Offset, name: nameof(Unknown1Offset));
            Unknown2Offset = s.Serialize<ushort>(Unknown2Offset, name: nameof(Unknown2Offset));
            ActionsCount = s.Serialize<byte>(ActionsCount, name: nameof(ActionsCount));

            Actions = s.SerializeObjectArray<GBC_Action>(Actions, ActionsCount, name: nameof(Actions));

            s.DoAt(Offset + Unknown1Offset, () => 
            {
                Data1Offsets = s.SerializeArray<ushort>(Data1Offsets, ActionsCount, name: nameof(Data1Offsets));

                if (ActionData1 == null) 
                    ActionData1 = new GBC_ActionData1[ActionsCount];

                for (int i = 0; i < Data1Offsets.Length; i++)
                {
                    if (Data1Offsets[i] != 0)
                        ActionData1[i] = s.DoAt(Offset + Data1Offsets[i], () => s.SerializeObject<GBC_ActionData1>(ActionData1[i], name: $"{nameof(ActionData1)}[{i}]"));
                }
            });

            s.Goto(Offset + Unknown2Offset);
            ActionData2 = s.SerializeObjectArray<GBC_ActionData2>(ActionData2, ActionsCount, name: nameof(ActionData2));
        }
    }
}