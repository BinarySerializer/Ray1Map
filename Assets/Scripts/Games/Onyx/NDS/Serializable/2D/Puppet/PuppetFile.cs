namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    // NOTE: Puppet, which comes from the GBC engine, might be the wrong name. On GBA and NDS it's known as AOBJ. That
    //       probably stands for AnimatedObject (although the GBA version defines a separate struct named that).
    public class PuppetFile : OnyxFile
    {
        public ushort Ushort_00 { get; set; }
        public PuppetFlags Flags { get; set; }
        public byte TileBufferSize { get; set; } // Length of buffer for reusing tiles when allocating?
        public bool[] PaletteBufferFlags { get; set; }
        
        public FileReference<SpriteTileSetFile> TileSetReference { get; set; }
        
        public uint PalettesCount { get; set; }
        public FileReference<PaletteFile>[] PaletteReferences { get; set; }

        public uint AnimationsCount { get; set; }
        public Animation[] Animations { get; set; }

        public override void SerializeFile(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            Flags = s.Serialize<PuppetFlags>(Flags, name: nameof(Flags));
            TileBufferSize = s.Serialize<byte>(TileBufferSize, name: nameof(TileBufferSize));
            s.DoBits<ushort>(b =>
            {
                PaletteBufferFlags ??= new bool[16];

                for (int i = 0; i < PaletteBufferFlags.Length; i++)
                    PaletteBufferFlags[i] = b.SerializeBits<bool>(PaletteBufferFlags[i], 1, name: $"{nameof(PaletteBufferFlags)}[{i}]");
            });

            TileSetReference = s.SerializeObject<FileReference<SpriteTileSetFile>>(TileSetReference, name: nameof(TileSetReference));

            PalettesCount = s.Serialize<uint>(PalettesCount, name: nameof(PalettesCount));
            PaletteReferences = s.SerializeObjectArray<FileReference<PaletteFile>>(PaletteReferences, PalettesCount, name: nameof(PaletteReferences));

            AnimationsCount = s.Serialize<uint>(AnimationsCount, name: nameof(AnimationsCount));
            Animations = s.SerializeObjectArray<Animation>(Animations, AnimationsCount, name: nameof(Animations));
        }

        public override void ResolveDependencies(SerializerObject s)
        {
            TileSetReference?.Resolve(s);
            PaletteReferences?.Resolve(s);
        }
    }
}