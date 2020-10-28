namespace R1Engine
{
    public class GBARRR_Mode7Object : R1Serializable
    {
        public short Short_00 { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Data = s.SerializeArray<byte>(Data, 32 - (2 * 3), name: nameof(Data));
        }
    }
}