using System;

namespace R1Engine
{
    public class GBACrash_Isometric_Animation : R1Serializable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public FixedPointInt XPos { get; set; } // What is this for?
        public FixedPointInt YPos { get; set; } // What is this for?
        public Pointer FramesPointer { get; set; }
        public uint FramesCountPointer { get; set; } // This points to memory since a portion of the rom is copied to memory for faster reading
        public Pointer PalettePointer { get; set; }
        public int PaletteIndex { get; set; }
        public int Flags { get; set; }

        // Serialized from pointers

        public Pointer[] FramesPointers { get; set; }
        public int FramesCount { get; set; }
        public RGBA5551Color[] Palette { get; set; }
        public byte[][] AnimFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<int>(Width, name: nameof(Width));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            XPos = s.SerializeObject<FixedPointInt>(XPos, name: nameof(XPos));
            YPos = s.SerializeObject<FixedPointInt>(YPos, name: nameof(YPos));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            FramesCountPointer = s.Serialize<uint>(FramesCountPointer, name: nameof(FramesCountPointer));

            if (s.DoAt(s.CurrentPointer, () => s.Serialize<int>(PaletteIndex, name: "PaletteValue")) >= 0x10)
                PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            else
                PaletteIndex = s.Serialize<int>(PaletteIndex, name: nameof(PaletteIndex));

            Flags = s.Serialize<int>(Flags, name: nameof(Flags));

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
                    switch (FramesCountPointer)
                    {
                        // EU
                        case 0x02000578: FramesCount = 0x0A; break;
                        case 0x02000574: FramesCount = 0x0E; break;
                        case 0x0200057c: FramesCount = 0x0A; break;
                        case 0x02000580: FramesCount = 0x0A; break;
                        case 0x02000588: FramesCount = 0x02; break;
                        case 0x02000584: FramesCount = 0x0F; break;

                        default: throw new ArgumentOutOfRangeException(nameof(FramesCountPointer), FramesCountPointer, null);
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

        public static GBACrash_Isometric_Animation CrateAndSerialize(SerializerObject s, Pointer framesPointer, int framesCount, int width, int height, int palIndex)
        {
            var anim = new GBACrash_Isometric_Animation
            {
                Width = width * 8,
                Height = height * 8,
                FramesPointer = framesPointer,
                PaletteIndex = palIndex,
                FramesCount = framesCount,
            };

            anim.SerializeData(s);

            return anim;
        }
        public static GBACrash_Isometric_Animation CrateAndSerialize(SerializerObject s, Pointer framesPointer, int framesCount, int width, int height, Pointer palPointer)
        {
            var anim = new GBACrash_Isometric_Animation
            {
                Width = width * 8,
                Height = height * 8,
                FramesPointer = framesPointer,
                PalettePointer = palPointer,
                FramesCount = framesCount,
            };

            anim.SerializeData(s);

            return anim;
        }
    }
}