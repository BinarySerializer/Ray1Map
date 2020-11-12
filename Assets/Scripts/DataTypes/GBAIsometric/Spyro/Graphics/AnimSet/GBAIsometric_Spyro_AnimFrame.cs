namespace R1Engine
{
    public class GBAIsometric_Spyro_AnimFrame : R1Serializable
    {
        public ushort FrameImageIndex { get; set; }
        public int UnkBitFieldValue { get; set; }
        public byte Byte02 { get; set; }
        public byte Byte03 { get; set; }

        public bool HasExtraData { get; set; } // Set in onPreSerialize
        public ushort ExtraData_04 { get; set; }
        public ushort ExtraData_06 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                FrameImageIndex = (ushort)bitFunc(FrameImageIndex, 11, name: nameof(FrameImageIndex));
                UnkBitFieldValue = bitFunc(UnkBitFieldValue, 5, name: nameof(UnkBitFieldValue));
            });
            Byte02 = s.Serialize<byte>(Byte02, name: nameof(Byte02));
            Byte03 = s.Serialize<byte>(Byte03, name: nameof(Byte03));

            if (HasExtraData) {
                ExtraData_04 = s.Serialize<ushort>(ExtraData_04, name: nameof(ExtraData_04));
                ExtraData_06 = s.Serialize<ushort>(ExtraData_06, name: nameof(ExtraData_06));
            }
        }
    }
}