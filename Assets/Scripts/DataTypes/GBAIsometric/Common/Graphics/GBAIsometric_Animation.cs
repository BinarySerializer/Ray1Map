namespace R1Engine
{
    public class GBAIsometric_Animation : R1Serializable
    {
        public byte Speed { get; set; }
        public byte FrameCount { get; set; }
        public ushort StartFrameIndex { get; set; }
        public bool FlipX { get; set; }

        public byte Spyro_Byte_02 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
            {
                Speed = s.Serialize<byte>(Speed, name: nameof(Speed));
                FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
                s.SerializeBitValues<ushort>(bitfunc => {
                    StartFrameIndex = (ushort)bitfunc(StartFrameIndex, 15, name: nameof(StartFrameIndex));
                    FlipX = bitfunc(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
                });
            }
            else
            {
                StartFrameIndex = s.Serialize<ushort>(StartFrameIndex, name: nameof(StartFrameIndex));
                Spyro_Byte_02 = s.Serialize<byte>(Spyro_Byte_02, name: nameof(Spyro_Byte_02));
                FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            }
        }
    }
}