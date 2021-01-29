namespace R1Engine
{
    public class GBAVV_Mode7_Animation : R1Serializable
    {
        public ushort Ushort_00 { get; set; }
        public ushort FrameIndex { get; set; }
        public ushort FramesCount { get; set; }
        public byte[] Bytes_06 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));
            FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
            Bytes_06 = s.SerializeArray<byte>(Bytes_06, 6, name: nameof(Bytes_06));
        }
    }
}