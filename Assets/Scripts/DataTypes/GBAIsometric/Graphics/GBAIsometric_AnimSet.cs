namespace R1Engine
{
    public class GBAIsometric_AnimSet : R1Serializable
    {
        public byte[] Bytes_00 { get; set; }
        public Pointer PalettePointer { get; set; }
        public Pointer Pointer_14 { get; set; }
        public Pointer Pointer_18 { get; set; }
        public Pointer Pointer_1C { get; set; } // Img data?
        public Pointer Pointer_20 { get; set; }
        public Pointer<GBAIsometric_GraphicsData> GraphicsDataPointer { get; set; }
        public byte[] Bytes_28 { get; set; }
        public Pointer NamePointer { get; set; }

        public ARGB1555Color[] Palette { get; set; }
        public string Name { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 16, name: nameof(Bytes_00));
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            Pointer_14 = s.SerializePointer(Pointer_14, name: nameof(Pointer_14));
            Pointer_18 = s.SerializePointer(Pointer_18, name: nameof(Pointer_18));
            Pointer_1C = s.SerializePointer(Pointer_1C, name: nameof(Pointer_1C));
            Pointer_20 = s.SerializePointer(Pointer_20, name: nameof(Pointer_20));
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Bytes_28 = s.SerializeArray<byte>(Bytes_28, 8, name: nameof(Bytes_28));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            Palette = s.DoAt(PalettePointer, () => s.SerializeObjectArray<ARGB1555Color>(Palette, 16, name: nameof(Palette)));
            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));
        }
    }
}