using BinarySerializer;

namespace Ray1Map.Jade {
    public class COL_Cylinder : BinarySerializable {
        public Jade_Vector Type3_Vector { get; set; }
        public float Type3_Float_04 { get; set; }
        public float Type3_Float_08 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Type3_Vector = s.SerializeObject<Jade_Vector>(Type3_Vector, name: nameof(Type3_Vector));
            Type3_Float_04 = s.Serialize<float>(Type3_Float_04, name: nameof(Type3_Float_04));
            Type3_Float_08 = s.Serialize<float>(Type3_Float_08, name: nameof(Type3_Float_08));
        }
    }
}