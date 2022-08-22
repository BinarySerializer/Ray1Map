using BinarySerializer;

namespace Ray1Map.Jade
{
    public class PS2_VU_BoneWeight : BinarySerializable
    {
        public float[] Weights { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			Weights = s.SerializeArray<float>(Weights, 3, name: nameof(Weights));
            s.SerializePadding(4);
		}
    }
}