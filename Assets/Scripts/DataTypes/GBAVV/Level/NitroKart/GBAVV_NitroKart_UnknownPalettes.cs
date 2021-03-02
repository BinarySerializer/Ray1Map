namespace R1Engine
{
    public class GBAVV_NitroKart_UnknownPalettes : R1Serializable
    {
        public Pointer[] PalettePointers { get; set; }

        // Serialized from pointers
        public RGBA5551Color[][] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            PalettePointers = s.SerializePointerArray(PalettePointers, 3, name: nameof(PalettePointers));

            if (Palettes == null)
                Palettes = new RGBA5551Color[PalettePointers.Length][];

            for (int i = 0; i < Palettes.Length; i++)
                Palettes[i] = s.DoAt(PalettePointers[i], () => s.SerializeObjectArray<RGBA5551Color>(Palettes[i], 256, name: $"{nameof(Palettes)}[{i}]"));
        }
    }
}