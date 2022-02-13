using BinarySerializer;

namespace Ray1Map.Jade
{
    public class EVE_Event_PlaySynchro : BinarySerializable
    {
        public byte[] Bytes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes = s.SerializeArray<byte>(Bytes, 12, name: nameof(Bytes));
        }
    }
}