using System.Collections.Generic;

namespace R1Engine
{
    public class GBARRR_Mode7AnimSet : R1Serializable
    {
        public int Length { get; set; } // set in OnPreSerialize

        public Pointer[] FramePointers { get; set; }
        public GBARRR_Mode7AnimationFrame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FramePointers = s.SerializePointerArray(FramePointers, Length, name: nameof(FramePointers));
            if (Frames == null) {
                Frames = new GBARRR_Mode7AnimationFrame[Length];
                for (int i = 0; i < Length; i++) {
                    Frames[i] = s.DoAt(FramePointers[i], () => s.SerializeObject<GBARRR_Mode7AnimationFrame>(Frames[i], name: $"{nameof(Frames)}[{i}]"));
                }
            }
        }
    }
}