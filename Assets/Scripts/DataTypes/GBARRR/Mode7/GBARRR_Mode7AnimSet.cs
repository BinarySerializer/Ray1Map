using System.Collections.Generic;

namespace R1Engine
{
    public class GBARRR_Mode7AnimSet : R1Serializable
    {
        public int Length { get; set; } // set in OnPreSerialize

        public Pointer<GBARRR_Mode7AnimationFrame>[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Frames = s.SerializePointerArray<GBARRR_Mode7AnimationFrame>(Frames, Length, resolve: true, name: nameof(Frames));
        }
    }
}