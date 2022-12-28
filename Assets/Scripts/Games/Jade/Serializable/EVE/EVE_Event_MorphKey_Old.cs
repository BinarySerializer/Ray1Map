using BinarySerializer;

namespace Ray1Map.Jade
{
    public class EVE_Event_MorphKey_Old : BinarySerializable
    {
        public int MorphBone { get; set; }
        public int Channel { get; set; }
        public int Target1 { get; set; }
        public int Target2 { get; set; }
        public float Prog { get; set; }
        public float Factor { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MorphBone = s.Serialize<int>(MorphBone, name: nameof(MorphBone));
            Channel = s.Serialize<int>(Channel, name: nameof(Channel));
            Target1 = s.Serialize<int>(Target1, name: nameof(Target1));
            Target2 = s.Serialize<int>(Target2, name: nameof(Target2));
            Prog = s.Serialize<float>(Prog, name: nameof(Prog));
            Factor = s.Serialize<float>(Factor, name: nameof(Factor));
        }
    }
}