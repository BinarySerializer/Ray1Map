using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_UV16 : BinarySerializable
    {
        public ushort U { get; set; }
        public ushort V { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<ushort>(U, name: nameof(U));
            V = s.Serialize<ushort>(V, name: nameof(V));
        }
    }
}