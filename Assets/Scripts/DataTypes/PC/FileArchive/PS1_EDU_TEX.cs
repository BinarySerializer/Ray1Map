using System;

namespace R1Engine
{
    public class PS1_EDU_TEX : R1Serializable
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
        public ObjectArray<ARGB1555Color>[] Palettes { get; set; }
        public PS1_EDU_TEXDescriptor[] Descriptors { get; set; }
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
            if (Palettes == null) Palettes = new ObjectArray<ARGB1555Color>[NumPalettes];
            if (Descriptors == null) Descriptors = new PS1_EDU_TEXDescriptor[NumDescriptors];

            PagePointers = s.SerializePointerArray(PagePointers, PagePointers.Length, name: nameof(PagePointers));
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
                return Width * Height * BitDepth / 8;
            }
        }
    }
}