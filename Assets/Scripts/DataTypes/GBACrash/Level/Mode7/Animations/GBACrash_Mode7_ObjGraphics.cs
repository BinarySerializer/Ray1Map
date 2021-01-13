using System;
using System.Collections.Generic;

namespace R1Engine
{
    public class GBACrash_Mode7_ObjGraphics : GBACrash_BaseBlock
    {
        public GBACrash_Mode7_AnimSet[] AnimSets { get; set; } // Set before serializing

        public override void SerializeBlock(SerializerObject s)
        {
            var baseOffset = s.CurrentPointer;

            for (var animSetIndex = 0; animSetIndex < AnimSets.Length; animSetIndex++)
            {
                var animSet = AnimSets[animSetIndex];

                if (animSet.ObjFrames == null)
                {
                    var frames = new List<GBACrash_Mode7_ObjFrame>();

                    for (var frameIndex = 0; frameIndex < animSet.FrameOffsets.Length; frameIndex++)
                    {
                        var offset = animSet.FrameOffsets[frameIndex];

                        var pointer = animSet.Index == 0 ? new Pointer(offset, Offset.file) : baseOffset + offset;

                        var frame = s.DoAt(pointer, () => s.SerializeObject<GBACrash_Mode7_ObjFrame>(default, name: $"{nameof(AnimSets)}[{animSetIndex}].{nameof(animSet.ObjFrames)}[{frameIndex}]"));

                        frames.Add(frame);
                    }

                    animSet.ObjFrames = frames.ToArray();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}