using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Crash1_CutsceneEntry : BinarySerializable
    {
        public Pointer FramePointersPointer { get; set; }
        public int FramesCount { get; set; }

        public Pointer[] FramePointers { get; set; }
        public GBAVV_Crash1_CutsceneFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FramePointersPointer = s.SerializePointer(FramePointersPointer, name: nameof(FramePointersPointer));
            FramesCount = s.Serialize<int>(FramesCount, name: nameof(FramesCount));

            FramePointers = s.DoAt(FramePointersPointer, () => s.SerializePointerArray(FramePointers, FramesCount, name: nameof(FramePointers)));

            if (Frames == null)
                Frames = new GBAVV_Crash1_CutsceneFrame[FramesCount];

            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = s.DoAt(FramePointers[i], () => s.SerializeObject<GBAVV_Crash1_CutsceneFrame>(Frames[i], name: $"{nameof(Frames)}[{i}]"));
        }
    }
}