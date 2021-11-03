using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_NGage_UV : BinarySerializable
    {
        public byte U { get; set; }
        public byte V { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<byte>(U, name: nameof(U));
            V = s.Serialize<byte>(V, name: nameof(V));
        }
    }
}