using BinarySerializer;

namespace Ray1Map.Jade
{
    public class PS2_VU_BoneIndex : BinarySerializable
    {
        public int[] Bones { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			Bones = s.SerializeArray<int>(Bones, 3, name: nameof(Bones));
            s.SerializePadding(4);
		}
    }
}