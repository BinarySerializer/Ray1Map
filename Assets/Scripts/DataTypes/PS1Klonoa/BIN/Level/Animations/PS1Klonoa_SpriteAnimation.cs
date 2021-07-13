using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_SpriteAnimation : BinarySerializable
    {
        public Pointer Pre_OffsetAnchor { get; set; }

        public byte FramesCount { get; set; }
        public byte Flags { get; set; } // 0 or 0x80 - determines play direction?
        public ushort FramesOffset { get; set; }

        public PS1Klonoa_SpriteAnimationFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
            FramesOffset = s.Serialize<ushort>(FramesOffset, name: nameof(FramesOffset));

            s.DoAt(Pre_OffsetAnchor + FramesOffset, () => Frames = s.SerializeObjectArray<PS1Klonoa_SpriteAnimationFrame>(Frames, FramesCount, name: nameof(Frames)));
        }
    }
}