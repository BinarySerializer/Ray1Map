using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_RHR_Animation : BinarySerializable
    {
        public byte Speed { get; set; }
        public byte FrameCount { get; set; }
        public ushort StartFrameIndex { get; set; }
        public bool FlipX { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Speed = s.Serialize<byte>(Speed, name: nameof(Speed));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            s.DoBits<ushort>(b => {
                StartFrameIndex = (ushort)b.SerializeBits<int>(StartFrameIndex, 15, name: nameof(StartFrameIndex));
                FlipX = b.SerializeBits<int>(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
            });
        }
    }
}