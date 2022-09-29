using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_UV : BinarySerializable, ISerializerShortLog
    {
        public int Pre_JointInfluencesPerVertex { get; set; }

        public float U { get; set; }
        public float V { get; set; }
        public float SkinWeight { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<float>(U, name: nameof(U));
            V = s.Serialize<float>(V, name: nameof(V));

            if (Pre_JointInfluencesPerVertex > 0)
                SkinWeight = s.Serialize<float>(SkinWeight, name: nameof(SkinWeight));
            else
                s.SerializePadding(4); // Indeterminate (repeated data)

            s.SerializePadding(4); // Indeterminate (repeated data)
        }

        public float UCorrected => U - 2056;
        public float VCorrected => V - 2056;
        public float SkinWeightCorrected => SkinWeight - 2056;

        public string ShortLog => Pre_JointInfluencesPerVertex > 0
            ? $"UV({UCorrected}, {VCorrected})+SkinWeight({SkinWeightCorrected})"
            : $"UV({UCorrected}, {VCorrected})";
    }
}