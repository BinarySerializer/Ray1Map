namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class Animation : BinarySerializable
    {
        public byte AnimSpeed { get; set; }
        public bool Oscillate { get; set; }
        public uint FramesCount { get; set; }
        public AnimationFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
            Oscillate = s.Serialize<bool>(Oscillate, name: nameof(Oscillate));
            FramesCount = s.Serialize<uint>(FramesCount, name: nameof(FramesCount));
            Frames = s.SerializeObjectArray<AnimationFrame>(Frames, FramesCount, name: nameof(Frames));
        }
    }
}