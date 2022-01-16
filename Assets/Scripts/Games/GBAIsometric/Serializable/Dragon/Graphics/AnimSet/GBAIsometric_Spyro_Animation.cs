using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_Animation : BinarySerializable
    {
        public ushort FrameOffset { get; set; }
        public byte AnimSpeed { get; set; }
        public bool Loop { get; set; }
        public bool PingPong { get; set; }
        public bool LastBit { get; set; }
        public byte FrameCount { get; set; }
        public bool FramesHaveExtraData { get; set; }

        public GBAIsometric_Spyro_AnimFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FrameOffset = s.Serialize<ushort>(FrameOffset, name: nameof(FrameOffset));
            s.DoBits<byte>(b =>
            {
                AnimSpeed = (byte)b.SerializeBits<int>(AnimSpeed, 5, name: nameof(AnimSpeed));
                Loop = b.SerializeBits<int>(Loop ? 1 : 0, 1, name: nameof(Loop)) == 1;
                PingPong = b.SerializeBits<int>(PingPong ? 1 : 0, 1, name: nameof(PingPong)) == 1;
                LastBit = b.SerializeBits<int>(LastBit ? 1 : 0, 1, name: nameof(LastBit)) == 1;
            });
            s.DoBits<byte>(b => {
                FrameCount = (byte)b.SerializeBits<int>(FrameCount, 7, name: nameof(FrameCount));
                FramesHaveExtraData = b.SerializeBits<int>(FramesHaveExtraData ? 1 : 0, 1, name: nameof(FramesHaveExtraData)) == 1;
            });
            s.DoAt(Offset + 4 * FrameOffset, () => {
                Frames = s.SerializeObjectArray<GBAIsometric_Spyro_AnimFrame>(Frames, FrameCount, onPreSerialize: f => f.HasExtraData = FramesHaveExtraData, name: nameof(Frames));
            });
        }
    }
}