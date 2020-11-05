namespace R1Engine
{
    public class GBAIsometric_RHR_Animation : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte FrameCount { get; set; }
        public ushort StartFrameIndex { get; set; }
        public bool Bool_02 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            s.SerializeBitValues<ushort>(bitfunc => {
                StartFrameIndex = (ushort)bitfunc(StartFrameIndex, 15, name: nameof(StartFrameIndex));
                Bool_02 = bitfunc(Bool_02 ? 1 : 0, 1, name: nameof(Bool_02)) == 1;
            });
        }
    }
}