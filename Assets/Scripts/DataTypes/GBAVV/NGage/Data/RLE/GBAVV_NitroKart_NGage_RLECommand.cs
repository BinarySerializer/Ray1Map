using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_RLECommand : BinarySerializable
    {
        public byte Type { get; set; } // 0 == Array, 1 == Empty, 2 == Repeat
        public byte Count { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<byte>(bitFunc =>
            {
                Type = (byte)bitFunc(Type, 2, name: nameof(Type));
                Count = (byte)bitFunc(Count, 6, name: nameof(Count));
            });
        }
    }
}