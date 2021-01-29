using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_Map2D_AnimSet : R1Serializable
    {
        public Pointer AnimationsPointer { get; set; }
        public Pointer FramePointersPointer { get; set; }
        public ushort Ushort_08 { get; set; }
        public ushort AnimationsCount { get; set; }

        // Serialized from pointers

        public GBAVV_Map2D_Animation[] Animations { get; set; }
        public Pointer[] FramePointers { get; set; }
        public GBAVV_Map2D_AnimationFrame[] AnimationFrames { get; set; }

        public int GetMinX(int animIndex) => Animations[animIndex].FrameTable.Select(x => AnimationFrames[x]).SelectMany(f => f.TilePositions.Select(x => x.XPos)).Min();
        public int GetMinY(int animIndex) => Animations[animIndex].FrameTable.Select(x => AnimationFrames[x]).SelectMany(f => f.TilePositions.Select(x => x.YPos)).Min();

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            FramePointersPointer = s.SerializePointer(FramePointersPointer, name: nameof(FramePointersPointer));
            Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
            AnimationsCount = s.Serialize<ushort>(AnimationsCount, name: nameof(AnimationsCount));

            Animations = s.DoAt(AnimationsPointer, () => s.SerializeObjectArray<GBAVV_Map2D_Animation>(Animations, AnimationsCount, name: nameof(Animations)));
            FramePointers = s.DoAt(FramePointersPointer, () => s.SerializePointerArray(FramePointers, Animations.SelectMany(x => x.FrameTable).Max() + 1, name: nameof(FramePointers)));

            if (AnimationFrames == null)
                AnimationFrames = new GBAVV_Map2D_AnimationFrame[FramePointers.Length];

            for (int i = 0; i < AnimationFrames.Length; i++)
                AnimationFrames[i] = s.DoAt(FramePointers[i], () => s.SerializeObject<GBAVV_Map2D_AnimationFrame>(AnimationFrames[i], name: $"{nameof(AnimationFrames)}[{i}]"));
        }

        public static Vector2Int[] TileShapes { get; } = new Vector2Int[]
        {
            new Vector2Int(0x08, 0x08),
            new Vector2Int(0x10, 0x10),
            new Vector2Int(0x20, 0x20),
            new Vector2Int(0x40, 0x40),
            new Vector2Int(0x10, 0x08),
            new Vector2Int(0x20, 0x08),
            new Vector2Int(0x20, 0x10),
            new Vector2Int(0x40, 0x20),
            new Vector2Int(0x08, 0x10),
            new Vector2Int(0x08, 0x20),
            new Vector2Int(0x10, 0x20),
            new Vector2Int(0x20, 0x40),
        };
    }
}