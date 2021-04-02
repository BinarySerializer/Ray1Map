using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_Node : BinarySerializable {
        public uint EntryIndex { get; set; }
        public COL_Node Child0 { get; set; }
        public COL_Node Child1 { get; set; }

        public int Int_Child_0 { get; set; }
        public int Int_Child_1 { get; set; }
        public int Int_Child_2 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            EntryIndex = s.Serialize<uint>(EntryIndex, name: nameof(EntryIndex));
            Int_Child_0 = s.Serialize<int>(Int_Child_0, name: nameof(Int_Child_0));
            int currentInd = Int_Child_0;
            if (currentInd == 1) {
                Child0 = s.SerializeObject<COL_Node>(Child0, name: nameof(Child0));
                Int_Child_1 = s.Serialize<int>(Int_Child_1, name: nameof(Int_Child_1));
                currentInd = Int_Child_1;
            }
            if (currentInd == 2) {
                Child1 = s.SerializeObject<COL_Node>(Child1, name: nameof(Child1));
                Int_Child_2 = s.Serialize<int>(Int_Child_2, name: nameof(Int_Child_2));
            }
        }
    }
}