using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Mode7_AnimSet : BinarySerializable
    {
        public bool IsSpongeBobSpecialAnim { get; set; } // Set before serializing
        public int? OverrideFramesCount { get; set; } // Set before serializing

        public uint Index { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public Pointer FrameOffsetsPointer { get; set; }
        public uint PaletteIndex { get; set; }
        public uint Uint_10 { get; set; }
        public short HitBox_XPos { get; set; }
        public short HitBox_YPos { get; set; }
        public short HitBox_ZPos { get; set; }
        public short HitBox_Width { get; set; }
        public short HitBox_Height { get; set; }
        public byte[] Bytes_1E { get; set; }

        public int SpongeBobSpecial_Int_00 { get; set; }
        public int SpongeBobSpecial_Int_04 { get; set; }

        // Serialized from pointers
        public GBAVV_Mode7_Animation[] Animations { get; set; }
        public uint[] FrameOffsets { get; set; }
        public GBAVV_Mode7_ObjFrame[] ObjFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (IsSpongeBobSpecialAnim)
            {
                SpongeBobSpecial_Int_00 = s.Serialize<int>(SpongeBobSpecial_Int_00, name: nameof(SpongeBobSpecial_Int_00));
                SpongeBobSpecial_Int_04 = s.Serialize<int>(SpongeBobSpecial_Int_04, name: nameof(SpongeBobSpecial_Int_04));
            }
            else
            {
                Index = s.Serialize<uint>(Index, name: nameof(Index));
            }

            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            FrameOffsetsPointer = s.SerializePointer(FrameOffsetsPointer, name: nameof(FrameOffsetsPointer));
            PaletteIndex = s.Serialize<uint>(PaletteIndex, name: nameof(PaletteIndex));
            Uint_10 = s.Serialize<uint>(Uint_10, name: nameof(Uint_10));

            if (!IsSpongeBobSpecialAnim)
            {
                // Hitbox values?
                HitBox_XPos = s.Serialize<short>(HitBox_XPos, name: nameof(HitBox_XPos));
                HitBox_YPos = s.Serialize<short>(HitBox_YPos, name: nameof(HitBox_YPos));
                HitBox_ZPos = s.Serialize<short>(HitBox_ZPos, name: nameof(HitBox_ZPos));
                HitBox_Width = s.Serialize<short>(HitBox_Width, name: nameof(HitBox_Width));
                HitBox_Height = s.Serialize<short>(HitBox_Height, name: nameof(HitBox_Height));
                Bytes_1E = s.SerializeArray<byte>(Bytes_1E, 10, name: nameof(Bytes_1E));
            }
        }

        public void SerializeAnimations(SerializerObject s, int length, int index)
        {
            Animations = s.DoAt(AnimationsPointer, () => s.SerializeObjectArray<GBAVV_Mode7_Animation>(Animations, length, name: $"{nameof(Animations)}[{index}]"));
            FrameOffsets = s.DoAt(FrameOffsetsPointer, () => s.SerializeArray<uint>(FrameOffsets, OverrideFramesCount ?? Animations.Max(x => x.FrameIndex + x.FramesCount), name: $"{nameof(FrameOffsets)}[{index}]"));
        }
    }
}