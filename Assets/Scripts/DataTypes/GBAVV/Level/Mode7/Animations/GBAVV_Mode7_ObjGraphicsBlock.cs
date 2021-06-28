using System;
using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.GBA;

namespace R1Engine
{
    public class GBAVV_Mode7_ObjGraphicsBlock : GBAVV_BaseBlock
    {
        public GBAVV_Mode7_AnimSet[] AnimSets { get; set; } // Set before serializing

        public override void SerializeBlock(SerializerObject s)
        {
            var baseOffset = s.CurrentPointer;

            for (var animSetIndex = 0; animSetIndex < AnimSets.Length; animSetIndex++)
            {
                var animSet = AnimSets[animSetIndex];

                if (animSet.FrameOffsets == null)
                {
                    animSet.ObjFrames = new GBAVV_Mode7_ObjFrame[0];
                    continue;
                }

                if (animSet.ObjFrames == null)
                {
                    var frames = new List<GBAVV_Mode7_ObjFrame>();

                    for (var frameIndex = 0; frameIndex < animSet.FrameOffsets.Length; frameIndex++)
                    {
                        var offset = animSet.FrameOffsets[frameIndex];

                        var pointer = offset > GBAConstants.Address_ROM ? new Pointer(offset, Offset.File) : baseOffset + offset;

                        var frame = s.DoAt(pointer, () => s.SerializeObject<GBAVV_Mode7_ObjFrame>(default, name: $"{nameof(AnimSets)}[{animSetIndex}].{nameof(animSet.ObjFrames)}[{frameIndex}]"));

                        frames.Add(frame);
                    }

                    animSet.ObjFrames = frames.ToArray();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            s.Goto(baseOffset + BlockLength);
        }
    }
}