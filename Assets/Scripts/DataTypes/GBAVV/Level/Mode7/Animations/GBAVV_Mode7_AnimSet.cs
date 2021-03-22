using System.Linq;

namespace R1Engine
{
    public class GBAVV_Mode7_AnimSet : R1Serializable
    {
        public bool IsSpongeBobSpecialAnim { get; set; } // Set before serializing
        public int? OverrideFramesCount { get; set; } // Set before serializing

        public uint Index { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public Pointer FrameOffsetsPointer { get; set; }
        public uint PaletteIndex { get; set; }
        public uint Uint_10 { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public short ZPos { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }
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
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                ZPos = s.Serialize<short>(ZPos, name: nameof(ZPos));
                Width = s.Serialize<short>(Width, name: nameof(Width));
                Height = s.Serialize<short>(Height, name: nameof(Height));
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