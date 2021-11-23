using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_NGage_RLECommand : BinarySerializable
    {
        public byte Type { get; set; } // 0 == Array, 1 == Empty, 2 == Repeat
        public byte Count { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<byte>(b =>
            {
                Type = (byte)b.SerializeBits<int>(Type, 2, name: nameof(Type));
                Count = (byte)b.SerializeBits<int>(Count, 6, name: nameof(Count));
            });
        }
    }
}