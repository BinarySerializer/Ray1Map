using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_UV : BinarySerializable
    {
        public int Pre_JointInfluencesPerVertex { get; set; }

        public ushort U { get; set; }
        public ushort V { get; set; }
        public ushort SkinWeight { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<ushort>(U, name: nameof(U));
            s.Align(4, Offset);
            V = s.Serialize<ushort>(V, name: nameof(V));
            s.Align(4, Offset);

            if (Pre_JointInfluencesPerVertex > 0)
            {
                SkinWeight = s.Serialize<ushort>(SkinWeight, name: nameof(SkinWeight));
                s.Align(4, Offset);
                s.SerializePadding(4); // Repeated data
            }
            else
            {
                s.SerializePadding(8); // Repeated data
            }
        }

        public float UFloat => ((U - 0x8000) / 4096f);// / QFloat;
        public float VFloat => ((V - 0x8000) / 4096f);// / QFloat;
        public float SkinWeightFloat => ((SkinWeight - 0x8000) / 4096f);

		//public override string ShortLog => $"PS2_GIF_UV({U:X4}, {V:X4} | {UFloat}, {VFloat})";
    }
}