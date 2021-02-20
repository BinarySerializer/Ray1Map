namespace R1Engine
{
    public class GBAVV_Map2D_Animation : R1Serializable
    {
        public Pointer FrameTablePointer { get; set; }

        public AnimRect HitBox { get; set; }
        public AnimRect RenderBox { get; set; }

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
            if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Fusion)
            {
                FrameTablePointer = s.SerializePointer(FrameTablePointer, name: nameof(FrameTablePointer));
                Fusion_FrameIndexTablePointer = s.SerializePointer(Fusion_FrameIndexTablePointer, name: nameof(Fusion_FrameIndexTablePointer));
                Fusion_PalettePointer = s.SerializePointer(Fusion_PalettePointer, name: nameof(Fusion_PalettePointer));
                RenderBox = s.SerializeObject<AnimRect>(RenderBox, name: nameof(RenderBox)); // Hitbox?
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
                HitBox = s.SerializeObject<AnimRect>(HitBox, name: nameof(HitBox));
                RenderBox = s.SerializeObject<AnimRect>(RenderBox, name: nameof(RenderBox));
                PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
                AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
                FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
                Byte_13 = s.Serialize<byte>(Byte_13, name: nameof(Byte_13));
                s.SerializeArray<byte>(new byte[4], 4, name: "Padding");
            }

            FrameIndexTable = s.DoAt(Fusion_FrameIndexTablePointer, () => s.SerializeArray<ushort>(FrameIndexTable, FramesCount, name: nameof(FrameIndexTable)));
        }

        public class AnimRect : R1Serializable
        {
            public short X { get; set; }
            public short Y { get; set; }
            public short Width { get; set; }
            public short Height { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                X = s.Serialize<short>(X, name: nameof(X));
                Y = s.Serialize<short>(Y, name: nameof(Y));

                if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash1)
                {
                    Width = s.Serialize<sbyte>((sbyte)Width, name: nameof(Width));
                    Height = s.Serialize<sbyte>((sbyte)Height, name: nameof(Height));
                    s.Serialize<ushort>(default, name: "Padding");
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash2)
                {
                    Width = s.Serialize<short>(Width, name: nameof(Width));
                    Height = s.Serialize<short>(Height, name: nameof(Height));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Fusion)
                {
                    Width = s.Serialize<sbyte>((sbyte)Width, name: nameof(Width));
                    Height = s.Serialize<sbyte>((sbyte)Height, name: nameof(Height));
                }
            }
        }
    }
}