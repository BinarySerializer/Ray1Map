namespace R1Engine
{
    public class GBARRR_Actor : R1Serializable
    {
        public short Unk1 { get; set; } // y position
        public short Unk2 { get; set; } // x position
        public byte[] Data1 { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte Byte_26 { get; set; }
        public byte Byte_27 { get; set; }
        public byte Byte_28 { get; set; }
        public byte Byte_29 { get; set; }
        public byte Byte_2A { get; set; }
        public byte[] Data2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<short>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<short>(Unk2, name: nameof(Unk2));
            Data1 = s.SerializeArray<byte>(Data1, 30, name: nameof(Data1));
            XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
            Byte_26 = s.Serialize<byte>(Byte_26, name: nameof(Byte_26));
            Byte_27 = s.Serialize<byte>(Byte_27, name: nameof(Byte_27));
            Byte_28 = s.Serialize<byte>(Byte_28, name: nameof(Byte_28));
            Byte_29 = s.Serialize<byte>(Byte_29, name: nameof(Byte_29));
            Byte_2A = s.Serialize<byte>(Byte_2A, name: nameof(Byte_2A));
            Data2 = s.SerializeArray<byte>(Data2, 21, name: nameof(Data2));
        }
    }
}