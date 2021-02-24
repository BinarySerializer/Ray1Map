namespace R1Engine
{
    public class GBAVV_Map2D_Animation : R1Serializable
    {
        public Pointer FrameTablePointer { get; set; }

        public GBAVV_Map2D_AnimationRect HitBox { get; set; }
        public GBAVV_Map2D_AnimationRect RenderBox { get; set; }

        public byte PaletteIndex { get; set; }
        public byte AnimSpeed { get; set; } // Speed + 1, except for Fusion
        public byte FramesCount { get; set; }
        public byte Byte_13 { get; set; }

        // Fusion
        public Pointer Fusion_FrameIndexTablePointer { get; set; }
        public Pointer Fusion_PalettePointer { get; set; }
        public ushort Fusion_Ushort_12 { get; set; } // Always 0
        public byte Fusion_Byte_16 { get; set; }
        public byte Fusion_Byte_17 { get; set; } // Always 0

        // Serialized from pointers
        public ushort[] FrameIndexTable { get; set; }

        // Fusion
        public GBAVV_Map2D_AnimSet Fusion_AnimSet { get; set; }
        public RGBA5551Color[] Fusion_Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.GBAVV_IsFusion)
            {
                FrameTablePointer = s.SerializePointer(FrameTablePointer, name: nameof(FrameTablePointer));
                Fusion_FrameIndexTablePointer = s.SerializePointer(Fusion_FrameIndexTablePointer, name: nameof(Fusion_FrameIndexTablePointer));
                Fusion_PalettePointer = s.SerializePointer(Fusion_PalettePointer, name: nameof(Fusion_PalettePointer));
                RenderBox = s.SerializeObject<GBAVV_Map2D_AnimationRect>(RenderBox, name: nameof(RenderBox)); // Hitbox?
                Fusion_Ushort_12 = s.Serialize<ushort>(Fusion_Ushort_12, name: nameof(Fusion_Ushort_12));
                AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
                FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
                Fusion_Byte_16 = s.Serialize<byte>(Fusion_Byte_16, name: nameof(Fusion_Byte_16));
                Fusion_Byte_17 = s.Serialize<byte>(Fusion_Byte_17, name: nameof(Fusion_Byte_17));

                Fusion_AnimSet = s.DoAt(FrameTablePointer, () => s.SerializeObject<GBAVV_Map2D_AnimSet>(Fusion_AnimSet, name: nameof(Fusion_AnimSet)));
                Fusion_Palette = s.DoAt(Fusion_PalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(Fusion_Palette, 16, name: nameof(Fusion_Palette)));
            }
            else
            {
                Fusion_FrameIndexTablePointer = s.SerializePointer(Fusion_FrameIndexTablePointer, name: nameof(Fusion_FrameIndexTablePointer));
                HitBox = s.SerializeObject<GBAVV_Map2D_AnimationRect>(HitBox, name: nameof(HitBox));
                RenderBox = s.SerializeObject<GBAVV_Map2D_AnimationRect>(RenderBox, name: nameof(RenderBox));
                PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
                AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
                FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
                Byte_13 = s.Serialize<byte>(Byte_13, name: nameof(Byte_13));
                s.SerializeArray<byte>(new byte[4], 4, name: "Padding");
            }

            FrameIndexTable = s.DoAt(Fusion_FrameIndexTablePointer, () => s.SerializeArray<ushort>(FrameIndexTable, FramesCount, name: nameof(FrameIndexTable)));
        }
    }
}