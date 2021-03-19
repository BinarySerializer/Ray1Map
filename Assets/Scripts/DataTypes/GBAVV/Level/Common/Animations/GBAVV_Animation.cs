namespace R1Engine
{
    public class GBAVV_Animation : R1Serializable
    {
        public Pointer Fusion_AnimSetPointer { get; set; }

        public GBAVV_AnimationRect HitBox { get; set; }
        public GBAVV_AnimationRect RenderBox { get; set; }

        public byte PaletteIndex { get; set; }
        public byte AnimSpeed { get; set; }
        public byte FramesCount { get; set; }
        public byte Byte_13 { get; set; }

        // Fusion & Nitro Kart (N-Gage)
        public Pointer FrameIndexTablePointer { get; set; }
        public Pointer PalettePointer { get; set; }
        public byte Byte_16 { get; set; }
        public byte Byte_17 { get; set; } // Always 0

        // Serialized from pointers
        public ushort[] FrameIndexTable { get; set; }

        // Fusion & Nitro Kart (N-Gage)
        public GBAVV_AnimSet AnimSet { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        // Helpers
        public int GetAnimSpeed => AnimSpeed + 1;

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.GBAVV_IsFusion || s.GameSettings.EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
            {
                Fusion_AnimSetPointer = s.SerializePointer(Fusion_AnimSetPointer, name: nameof(Fusion_AnimSetPointer)); // Null for Nitro Kart
                FrameIndexTablePointer = s.SerializePointer(FrameIndexTablePointer, name: nameof(FrameIndexTablePointer));
                PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
                RenderBox = s.SerializeObject<GBAVV_AnimationRect>(RenderBox, name: nameof(RenderBox));
                AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
                FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
                Byte_16 = s.Serialize<byte>(Byte_16, name: nameof(Byte_16));
                Byte_17 = s.Serialize<byte>(Byte_17, name: nameof(Byte_17));

                if (s.GameSettings.GBAVV_IsFusion)
                    AnimSet = s.DoAt(Fusion_AnimSetPointer, () => s.SerializeObject<GBAVV_AnimSet>(AnimSet, name: nameof(AnimSet)));

                Palette = s.DoAt(PalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(Palette, 16, name: nameof(Palette)));
            }
            else
            {
                FrameIndexTablePointer = s.SerializePointer(FrameIndexTablePointer, name: nameof(FrameIndexTablePointer));

                if (s.GameSettings.EngineVersion != EngineVersion.GBAVV_CrashNitroKart && s.GameSettings.EngineVersion != EngineVersion.GBAVV_X2WolverinesRevenge)
                    HitBox = s.SerializeObject<GBAVV_AnimationRect>(HitBox, name: nameof(HitBox));

                RenderBox = s.SerializeObject<GBAVV_AnimationRect>(RenderBox, name: nameof(RenderBox));
                PaletteIndex = s.Serialize<byte>(PaletteIndex, name: nameof(PaletteIndex));
                AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
                FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
                Byte_13 = s.Serialize<byte>(Byte_13, name: nameof(Byte_13));

                if (s.GameSettings.EngineVersion != EngineVersion.GBAVV_CrashNitroKart && s.GameSettings.EngineVersion != EngineVersion.GBAVV_X2WolverinesRevenge)
                    s.SerializeArray<byte>(new byte[4], 4, name: "Padding");
            }

            FrameIndexTable = s.DoAt(FrameIndexTablePointer, () => s.SerializeArray<ushort>(FrameIndexTable, FramesCount, name: nameof(FrameIndexTable)));
        }
    }
}