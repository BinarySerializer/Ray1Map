namespace R1Engine
{
    public class GBAIsometric_Object : R1Serializable
    {
        public short ObjectType { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public short Short_06 { get; set; }
        public short Short_08 { get; set; }

        public byte Byte_0A { get; set; }
        public byte LinkIndex { get; set; } // 0xFF if not linked

        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<short>(ObjectType, name: nameof(ObjectType));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Short_06 = s.Serialize<short>(Short_06, name: nameof(Short_06));
            Short_08 = s.Serialize<short>(Short_08, name: nameof(Short_08));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            LinkIndex = s.Serialize<byte>(LinkIndex, name: nameof(LinkIndex));
            Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
        }
    }
}