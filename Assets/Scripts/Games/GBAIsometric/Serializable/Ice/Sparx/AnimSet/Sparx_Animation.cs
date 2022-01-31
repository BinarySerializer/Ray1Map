using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class Sparx_Animation : BinarySerializable
    {
        public int FramesCount { get; set; }
        public Pointer FramesPointer { get; set; }

        public Sparx_Frame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FramesCount = s.Serialize<int>(FramesCount, name: nameof(FramesCount));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));

            s.DoAt(FramesPointer, () => Frames = s.SerializeObjectArray<Sparx_Frame>(Frames, FramesCount, name: nameof(Frames)));
        }
    }
}