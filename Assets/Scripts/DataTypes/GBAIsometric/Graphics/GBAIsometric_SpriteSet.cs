namespace R1Engine
{
    // For the flags, buttons etc.
    public class GBAIsometric_SpriteSet : R1Serializable
    {
        public uint Uint_00 { get; set; }
        public Pointer<GBAIsometric_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer PalettePointer { get; set; }
        public Pointer NamePointer { get; set; }

        public ARGB1555Color[] Palette { get; set; }
        public string Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            Palette = s.DoAt(PalettePointer, () => s.SerializeObjectArray<ARGB1555Color>(Palette, 16, name: nameof(Palette)));
            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
        }
    }
}