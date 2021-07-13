using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_SpriteAnimationFrame : BinarySerializable
    {
        public byte SpriteIndex { get; set; }
        public byte Byte_01 { get; set; }
        public sbyte SByte_02 { get; set; }
        public byte FrameDelay { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SpriteIndex = s.Serialize<byte>(SpriteIndex, name: nameof(SpriteIndex));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            SByte_02 = s.Serialize<sbyte>(SByte_02, name: nameof(SByte_02));
            FrameDelay = s.Serialize<byte>(FrameDelay, name: nameof(FrameDelay));
        }
    }
}