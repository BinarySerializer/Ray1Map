using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_TileMapCommand : BinarySerializable
    {
        public ushort Length { get; set; }
        public byte Type { get; set; }

        public ushort Param { get; set; }
        public ushort[] Params { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<ushort>(b =>
            {
                Length = (ushort)b.SerializeBits<int>(Length, 14, name: nameof(Length));
                Type = (byte)b.SerializeBits<int>(Type, 2, name: nameof(Type));
            });

            if (Type == 3)
                Params = s.SerializeArray<ushort>(Params, Length, name: nameof(Params));
            else if (Type == 2)
                Param = s.Serialize<ushort>(Param, name: nameof(Param));
        }
    }
}