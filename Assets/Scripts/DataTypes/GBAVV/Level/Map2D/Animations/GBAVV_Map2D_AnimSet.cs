using System.Linq;

namespace R1Engine
{
    public class GBAVV_Map2D_AnimSet : R1Serializable
    {
        public Pointer AnimationsPointer { get; set; }
        public Pointer FramePointersPointer { get; set; }
        public ushort Ushort_08 { get; set; }
        public int AnimationsCount { get; set; }

        // Fusion
        public Pointer Fusion_SelfPointer { get; set; }
        public Pointer Fusion_FramesPointer { get; set; }
        public int Fusion_FramesCount { get; set; }
        public int Fusion_Int_10 { get; set; }

        // Serialized from pointers
        public GBAVV_Map2D_Animation[] Animations { get; set; }
        public Pointer[] FramePointers { get; set; }
        public GBAVV_Map2D_AnimationFrame[] AnimationFrames { get; set; }

        public int GetMinX(int animIndex)
        {
            var framesX = Animations[animIndex].FrameIndexTable.Select(x => AnimationFrames[x]).Where(x => x.TilesCount > 0)
                .SelectMany(f => f.TilePositions.Select(x => x.XPos)).ToArray();

            return framesX.Any() ? framesX.Min() : 0;
        }

        public int GetMinY(int animIndex)
        {
            var framesY = Animations[animIndex].FrameIndexTable.Select(x => AnimationFrames[x]).Where(x => x.TilesCount > 0)
                .SelectMany(f => f.TilePositions.Select(x => x.YPos)).ToArray();

            return framesY.Any() ? framesY.Min() : 0;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.GBAVV_IsFusion)
            {
                Fusion_SelfPointer = s.SerializePointer(Fusion_SelfPointer, name: nameof(Fusion_SelfPointer));
                Fusion_FramesPointer = s.SerializePointer(Fusion_FramesPointer, name: nameof(Fusion_FramesPointer));
                AnimationsCount = s.Serialize<int>(AnimationsCount, name: nameof(AnimationsCount));
                Fusion_FramesCount = s.Serialize<int>(Fusion_FramesCount, name: nameof(Fusion_FramesCount));
                Fusion_Int_10 = s.Serialize<int>(Fusion_Int_10, name: nameof(Fusion_Int_10));
                Animations = s.SerializeObjectArray<GBAVV_Map2D_Animation>(Animations, AnimationsCount, name: nameof(Animations));

                AnimationFrames = s.DoAt(Fusion_FramesPointer, () => s.SerializeObjectArray<GBAVV_Map2D_AnimationFrame>(AnimationFrames, Fusion_FramesCount, name: nameof(AnimationFrames)));
            }
            else
            {
                AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
                FramePointersPointer = s.SerializePointer(FramePointersPointer, name: nameof(FramePointersPointer));
                Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
                AnimationsCount = s.Serialize<ushort>((ushort)AnimationsCount, name: nameof(AnimationsCount));

                Animations = s.DoAt(AnimationsPointer, () => s.SerializeObjectArray<GBAVV_Map2D_Animation>(Animations, AnimationsCount, name: nameof(Animations)));
                FramePointers = s.DoAt(FramePointersPointer, () => s.SerializePointerArray(FramePointers, Animations.SelectMany(x => x.FrameIndexTable).Max() + 1, name: nameof(FramePointers)));

                if (AnimationFrames == null)
                    AnimationFrames = new GBAVV_Map2D_AnimationFrame[FramePointers.Length];

                for (int i = 0; i < AnimationFrames.Length; i++)
                    AnimationFrames[i] = s.DoAt(FramePointers[i], () => s.SerializeObject<GBAVV_Map2D_AnimationFrame>(AnimationFrames[i], name: $"{nameof(AnimationFrames)}[{i}]"));
            }
        }
    }
}