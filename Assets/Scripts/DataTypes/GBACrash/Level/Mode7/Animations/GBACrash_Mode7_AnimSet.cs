using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBACrash_Mode7_AnimSet : R1Serializable
    {
        public uint Index { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public Pointer FrameOffsetsPointer { get; set; }
        public uint PaletteIndex { get; set; }
        public uint Uint_10 { get; set; }
        public short Short_14 { get; set; }
        public short Short_16 { get; set; }
        public short Short_18 { get; set; }
        public short Short_1A { get; set; }
        public short Short_1C { get; set; }
        public byte[] Bytes_1E { get; set; }

        // Serialized from pointers
        
        public GBACrash_Mode7_Animation[] Animations { get; set; }
        public uint[] FrameOffsets { get; set; }
        public GBACrash_Mode7_ObjFrame[] ObjFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Index = s.Serialize<uint>(Index, name: nameof(Index));
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            FrameOffsetsPointer = s.SerializePointer(FrameOffsetsPointer, name: nameof(FrameOffsetsPointer));
            PaletteIndex = s.Serialize<uint>(PaletteIndex, name: nameof(PaletteIndex));
            Uint_10 = s.Serialize<uint>(Uint_10, name: nameof(Uint_10));
            Short_14 = s.Serialize<short>(Short_14, name: nameof(Short_14));
            Short_16 = s.Serialize<short>(Short_16, name: nameof(Short_16));
            Short_18 = s.Serialize<short>(Short_18, name: nameof(Short_18));
            Short_1A = s.Serialize<short>(Short_1A, name: nameof(Short_1A));
            Short_1C = s.Serialize<short>(Short_1C, name: nameof(Short_1C));
            Bytes_1E = s.SerializeArray<byte>(Bytes_1E, 10, name: nameof(Bytes_1E));

            s.DoAt(AnimationsPointer, () =>
            {
                if (Animations == null)
                {
                    // Since there is no count we read until we get to an invalid animation
                    var anims = new List<GBACrash_Mode7_Animation>();

                    var currentFrameIndex = 0;
                    var index = 0;

                    while (true)
                    {
                        var anim = s.SerializeObject<GBACrash_Mode7_Animation>(default, name: $"{nameof(Animations)}[{index++}]");

                        if (anim.FrameIndex != currentFrameIndex)
                            break;

                        currentFrameIndex += anim.FramesCount;
                        anims.Add(anim);
                    }

                    Animations = anims.ToArray();
                }
                else
                {
                    s.SerializeObjectArray<GBACrash_Mode7_Animation>(Animations, Animations.Length, name: nameof(Animations));
                }
            });
            FrameOffsets = s.DoAt(FrameOffsetsPointer, () => s.SerializeArray<uint>(FrameOffsets, Animations.Max(x => x.FrameIndex + x.FramesCount), name: nameof(FrameOffsets)));
        }
    }
}