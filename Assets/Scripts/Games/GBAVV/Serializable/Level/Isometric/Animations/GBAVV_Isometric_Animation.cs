using System;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Isometric_Animation : BinarySerializable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public FixedPointInt32 XPos { get; set; } // What is this for?
        public FixedPointInt32 YPos { get; set; } // What is this for?
        public Pointer FramesPointer { get; set; }
        public uint FramesCountPointer { get; set; } // This points to memory since a portion of the rom is copied to memory for faster reading
        public Pointer PalettePointer { get; set; }
        public int PaletteIndex { get; set; }
        public bool IsFlipped { get; set; }

        // Serialized from pointers

        public Pointer[] FramesPointers { get; set; }
        public int FramesCount { get; set; }
        public RGBA5551Color[] Palette { get; set; }
        public byte[][] AnimFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<int>(Width, name: nameof(Width));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            XPos = s.SerializeObject<FixedPointInt32>(XPos, onPreSerialize: fpi => fpi.Pre_PointPosition = 12, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt32>(YPos, onPreSerialize: fpi => fpi.Pre_PointPosition = 12, name: nameof(YPos));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            FramesCountPointer = s.Serialize<uint>(FramesCountPointer, name: nameof(FramesCountPointer));

            if (s.DoAt(s.CurrentPointer, () => s.Serialize<int>(PaletteIndex, name: "PaletteValue")) >= 0x10)
                PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            else
                PaletteIndex = s.Serialize<int>(PaletteIndex, name: nameof(PaletteIndex));

            IsFlipped = s.Serialize<bool>(IsFlipped, name: nameof(IsFlipped));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");

            SerializeData(s);
        }

        public void SerializeData(SerializerObject s)
        {
            if (FramesCountPointer == 0 && FramesCount == 0)
            {
                FramesPointers = new Pointer[]
                {
                    FramesPointer
                };
            }
            else
            {
                if (FramesCount == 0)
                {
                    if (s.GetR1Settings().GameModeSelection == GameModeSelection.Crash2GBAUS)
                    {
                        switch (FramesCountPointer)
                        {
                            case 0x02000578: FramesCount = 0x0A; break;
                            case 0x02000574: FramesCount = 0x0E; break;
                            case 0x0200057c: FramesCount = 0x0A; break;
                            case 0x02000580: FramesCount = 0x0A; break;
                            case 0x02000588: FramesCount = 0x02; break;
                            case 0x02000584: FramesCount = 0x0F; break;

                            default: throw new ArgumentOutOfRangeException(nameof(FramesCountPointer), FramesCountPointer, null);
                        }
                    }
                    else if (s.GetR1Settings().GameModeSelection == GameModeSelection.Crash2GBAEU)
                    {
                        switch (FramesCountPointer)
                        {
                            case 0x0200058C: FramesCount = 0x0A; break;
                            case 0x02000588: FramesCount = 0x0E; break;
                            case 0x02000590: FramesCount = 0x0A; break;
                            case 0x02000594: FramesCount = 0x0A; break;
                            case 0x0200059C: FramesCount = 0x02; break;
                            case 0x02000598: FramesCount = 0x0F; break;

                            default: throw new ArgumentOutOfRangeException(nameof(FramesCountPointer), FramesCountPointer, null);
                        }
                    }
                    else if (s.GetR1Settings().GameModeSelection == GameModeSelection.Crash2GBAJP)
                    {
                        switch (FramesCountPointer)
                        {
                            case 0x0200059C: FramesCount = 0x0A; break;
                            case 0x02000598: FramesCount = 0x0E; break;
                            case 0x020005A0: FramesCount = 0x0A; break;
                            case 0x020005A4: FramesCount = 0x0A; break;
                            case 0x020005AC: FramesCount = 0x02; break;
                            case 0x020005A8: FramesCount = 0x0F; break;

                            default: throw new ArgumentOutOfRangeException(nameof(FramesCountPointer), FramesCountPointer, null);
                        }
                    }
                }

                FramesPointers = s.DoAt(FramesPointer, () => s.SerializePointerArray(FramesPointers, FramesCount, name: nameof(FramesPointer)));
            }

            if (PalettePointer != null)
                Palette = s.DoAt(PalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(Palette, 16, name: nameof(Palette)));

            if (AnimFrames == null)
                AnimFrames = new byte[FramesPointers.Length][];

            for (int i = 0; i < AnimFrames.Length; i++)
                AnimFrames[i] = s.DoAt(FramesPointers[i], () => s.SerializeArray<byte>(AnimFrames[i], (Width * Height) / 2, name: $"{nameof(AnimFrames)}[{i}]"));
        }

        public static GBAVV_Isometric_Animation CrateAndSerialize(SerializerObject s, Pointer framesPointer, int framesCount, int width, int height, int palIndex)
        {
            var anim = new GBAVV_Isometric_Animation
            {
                Width = width * 8,
                Height = height * 8,
                XPos = (FixedPointInt32)(-width * 4 / 16f),
                YPos = (FixedPointInt32)(-height * 4 / 16f),
                FramesPointer = framesPointer,
                PaletteIndex = palIndex,
                FramesCount = framesCount,
            };

            anim.SerializeData(s);

            return anim;
        }
        public static GBAVV_Isometric_Animation CrateAndSerialize(SerializerObject s, Pointer framesPointer, int framesCount, int width, int height, Pointer palPointer)
        {
            var anim = new GBAVV_Isometric_Animation
            {
                Width = width * 8,
                Height = height * 8,
                XPos = (FixedPointInt32)(-width * 4 / 16f),
                YPos = (FixedPointInt32)(-height * 4 / 16f),
                FramesPointer = framesPointer,
                PalettePointer = palPointer,
                FramesCount = framesCount,
            };

            anim.SerializeData(s);

            return anim;
        }
    }
}