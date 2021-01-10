using System.Linq;

namespace R1Engine
{
    public class GBACrash_AnimSet : R1Serializable
    {
        public Pointer AnimationsPointer { get; set; }
        public Pointer FramePointersPointer { get; set; }
        public ushort Ushort_08 { get; set; }
        public ushort AnimationsCount { get; set; }

        // Serialized from pointers

        public GBACrash_Animation[] Animations { get; set; }
        public Pointer[] FramePointers { get; set; }
        public GBACrash_AnimationFrame[] AnimationFrames { get; set; }

        public int GetMinX(int animIndex) => Animations[animIndex].FrameTable.Select(x => AnimationFrames[x]).SelectMany(f => f.TilePositions.Select(x => x.XPos)).Min();
        public int GetMinY(int animIndex) => Animations[animIndex].FrameTable.Select(x => AnimationFrames[x]).SelectMany(f => f.TilePositions.Select(x => x.YPos)).Min();

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            FramePointersPointer = s.SerializePointer(FramePointersPointer, name: nameof(FramePointersPointer));
            Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
            AnimationsCount = s.Serialize<ushort>(AnimationsCount, name: nameof(AnimationsCount));

            Animations = s.DoAt(AnimationsPointer, () => s.SerializeObjectArray<GBACrash_Animation>(Animations, AnimationsCount, name: nameof(Animations)));
            FramePointers = s.DoAt(FramePointersPointer, () => s.SerializePointerArray(FramePointers, Animations.SelectMany(x => x.FrameTable).Max() + 1, name: nameof(FramePointers)));

            if (AnimationFrames == null)
                AnimationFrames = new GBACrash_AnimationFrame[FramePointers.Length];

            for (int i = 0; i < AnimationFrames.Length; i++)
                AnimationFrames[i] = s.DoAt(FramePointers[i], () => s.SerializeObject<GBACrash_AnimationFrame>(AnimationFrames[i], name: $"{nameof(AnimationFrames)}[{i}]"));
        }
    }
}