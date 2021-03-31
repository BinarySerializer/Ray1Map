using BinarySerializer;

namespace R1Engine
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
            s.SerializeBitValues<byte>(bitFunc =>
            {
                AnimSpeed = (byte)bitFunc(AnimSpeed, 5, name: nameof(AnimSpeed));
                Loop = bitFunc(Loop ? 1 : 0, 1, name: nameof(Loop)) == 1;
                PingPong = bitFunc(PingPong ? 1 : 0, 1, name: nameof(PingPong)) == 1;
                LastBit = bitFunc(LastBit ? 1 : 0, 1, name: nameof(LastBit)) == 1;
            });
            s.SerializeBitValues<byte>(bitFunc => {
                FrameCount = (byte)bitFunc(FrameCount, 7, name: nameof(FrameCount));
                FramesHaveExtraData = bitFunc(FramesHaveExtraData ? 1 : 0, 1, name: nameof(FramesHaveExtraData)) == 1;
            });
            s.DoAt(Offset + 4 * FrameOffset, () => {
                Frames = s.SerializeObjectArray<GBAIsometric_Spyro_AnimFrame>(Frames, FrameCount, onPreSerialize: f => f.HasExtraData = FramesHaveExtraData, name: nameof(Frames));
            });
        }
    }
}