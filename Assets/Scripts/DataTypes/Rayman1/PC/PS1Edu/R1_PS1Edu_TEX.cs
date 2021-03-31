using BinarySerializer;

namespace R1Engine
{
    public class R1_PS1Edu_TEX : BinarySerializable
    {
        public uint NumDescriptors { get; set; }
        public uint NumPages { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint BitDepth { get; set; }
        public uint NumPalettes { get; set; }
        public uint Unk1 { get; set; } // Padding? Not referenced in the code


        // Parsed
        public Pointer[] PagePointers { get; set; }
        public ObjectArray<RGBA5551Color>[] Palettes { get; set; }
        public R1_PS1Edu_TEXDescriptor[] Descriptors { get; set; }
        public byte[][] TexturePages { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            NumDescriptors = s.Serialize<uint>(NumDescriptors, name: nameof(NumDescriptors));
            NumPages = s.Serialize<uint>(NumPages, name: nameof(NumPages));
            Width = s.Serialize<uint>(Width, name: nameof(Width));
            Height = s.Serialize<uint>(Height, name: nameof(Height));
            BitDepth = s.Serialize<uint>(BitDepth, name: nameof(BitDepth)); // can be 4, 8 or 16
            NumPalettes = s.Serialize<uint>(NumPalettes, name: nameof(NumPalettes));
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));

            // Initialize array sizes
            if (PagePointers == null) PagePointers = new Pointer[NumPages];
            if (TexturePages == null) TexturePages = new byte[NumPages][];
            if (Palettes == null) Palettes = new ObjectArray<RGBA5551Color>[NumPalettes];
            if (Descriptors == null) Descriptors = new R1_PS1Edu_TEXDescriptor[NumDescriptors];

            PagePointers = s.SerializePointerArray(PagePointers, PagePointers.Length, anchor: Offset, name: nameof(PagePointers));
            Palettes = s.SerializeObjectArray(Palettes, Palettes.Length, onPreSerialize: a => a.Length = PaletteLength, name: nameof(Palettes));
            Descriptors = s.SerializeObjectArray(Descriptors, Descriptors.Length, name: nameof(Descriptors));

            for (int i = 0; i < TexturePages.Length; i++) {
                s.DoAt(PagePointers[i], () => {
                    TexturePages[i] = s.SerializeArray<byte>(TexturePages[i], PageLength, name: $"{nameof(TexturePages)}[{i}]");
                });
            }
        }

        private uint PaletteLength {
            get {
                uint paletteSize = 0;
                switch (BitDepth) {
                    case 4: paletteSize = 16; break;
                    case 8: paletteSize = 256; break;
                    case 16: paletteSize = 0; break; // 16 bit is direct color, no palette
                }
                return paletteSize;
            }
        }
        private uint PageLength {
            get {
                return Width * Height;// * BitDepth / 8;
            }
        }

        public ushort GetPagePixel(int pageIndex, int x, int y) {
            int actualX = x * (int)BitDepth / 8;
            //int actualW = (int)Width * 8 / (int)BitDepth;
            int ind = y * (int)Width + actualX;
            switch (BitDepth) {
                case 4:
                    if (x % 2 == 0) {
                        return (ushort)BitHelpers.ExtractBits(TexturePages[pageIndex][ind], 4, 0);
                    } else {
                        return (ushort)BitHelpers.ExtractBits(TexturePages[pageIndex][ind], 4, 4);
                    }
                case 8:
                    return TexturePages[pageIndex][ind];
                case 16:
                    return (ushort)((TexturePages[pageIndex][ind]) | (TexturePages[pageIndex][ind + 1] << 8));
            }
            return 0;
        }
    }
}