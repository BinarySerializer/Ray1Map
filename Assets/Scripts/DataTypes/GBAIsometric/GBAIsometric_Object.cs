namespace R1Engine
{
    public class GBAIsometric_Object : R1Serializable
    {
        public short Short_00 { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public short Short_06 { get; set; }
        public short Short_08 { get; set; }

        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Short_06 = s.Serialize<short>(Short_06, name: nameof(Short_06));
            Short_08 = s.Serialize<short>(Short_08, name: nameof(Short_08));
            Data = s.SerializeArray<byte>(Data, 6, name: nameof(Data));
        }
    }
}