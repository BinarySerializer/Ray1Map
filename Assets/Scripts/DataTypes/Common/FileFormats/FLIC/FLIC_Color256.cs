using BinarySerializer;

namespace R1Engine
{
    public class FLIC_Color256 : BinarySerializable
    {
        public ushort PacketsCount { get; set; }
        public FLIC_ColorPacket[] Packets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PacketsCount = s.Serialize<ushort>(PacketsCount, name: nameof(PacketsCount));
            Packets = s.SerializeObjectArray<FLIC_ColorPacket>(Packets, PacketsCount, name: nameof(Packets));
        }

        public class FLIC_ColorPacket : BinarySerializable
        {
            public byte Skip { get; set; }
            public byte Count { get; set; }
            public RGB888Color[] Colors { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Skip = s.Serialize<byte>(Skip, name: nameof(Skip));
                Count = s.Serialize<byte>(Count, name: nameof(Count));
                Colors = s.SerializeObjectArray(Colors, Count == 0 ? 256 : Count, name: nameof(Colors));
            }
        }
    }
}