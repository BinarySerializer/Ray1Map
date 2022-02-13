using BinarySerializer;

namespace Ray1Map.Jade {
    public class COL_OK3_Node : BinarySerializable {
        public uint BoxIndex { get; set; }
        public COL_OK3_Node Son { get; set; }
        public COL_OK3_Node Next { get; set; }

        public int Int_Child_0 { get; set; }
        public int Int_Child_1 { get; set; }
        public int Int_Child_2 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            BoxIndex = s.Serialize<uint>(BoxIndex, name: nameof(BoxIndex));
            Int_Child_0 = s.Serialize<int>(Int_Child_0, name: nameof(Int_Child_0));
            int currentInd = Int_Child_0;
            if (currentInd == 1) {
                Son = s.SerializeObject<COL_OK3_Node>(Son, name: nameof(Son));
                Int_Child_1 = s.Serialize<int>(Int_Child_1, name: nameof(Int_Child_1));
                currentInd = Int_Child_1;
            }
            if (currentInd == 2) {
                Next = s.SerializeObject<COL_OK3_Node>(Next, name: nameof(Next));
                Int_Child_2 = s.Serialize<int>(Int_Child_2, name: nameof(Int_Child_2));
            }
        }
    }
}