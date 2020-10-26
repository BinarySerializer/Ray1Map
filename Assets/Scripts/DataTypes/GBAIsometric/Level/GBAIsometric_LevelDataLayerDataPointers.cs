namespace R1Engine
{
    public class GBAIsometric_LevelDataLayerDataPointers : R1Serializable
    {
        public Pointer Pointer0 { get; set; }
        public Pointer Pointer1 { get; set; }
        public Pointer Pointer2 { get; set; }
        public Pointer Pointer3 { get; set; }
        public Pointer Pointer4 { get; set; }
        public Pointer Pointer5 { get; set; }
        public Pointer Pointer6 { get; set; }
        public Pointer Pointer7 { get; set; }
        public Pointer Pointer8 { get; set; }
        public Pointer Pointer9 { get; set; }
        public Pointer Pointer10 { get; set; }
        public Pointer Pointer11 { get; set; }
        public Pointer PalettesPointer { get; set; }

        // Parsed
        public ARGB1555Color[] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointer0 = s.SerializePointer(Pointer0, name: nameof(Pointer0));
            Pointer1 = s.SerializePointer(Pointer1, name: nameof(Pointer1));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));
            Pointer3 = s.SerializePointer(Pointer3, name: nameof(Pointer3));
            Pointer4 = s.SerializePointer(Pointer4, name: nameof(Pointer4));
            Pointer5 = s.SerializePointer(Pointer5, name: nameof(Pointer5));
            Pointer6 = s.SerializePointer(Pointer6, name: nameof(Pointer6));
            Pointer7 = s.SerializePointer(Pointer7, name: nameof(Pointer7));
            Pointer8 = s.SerializePointer(Pointer8, name: nameof(Pointer8));
            Pointer9 = s.SerializePointer(Pointer9, name: nameof(Pointer9));
            Pointer10 = s.SerializePointer(Pointer10, name: nameof(Pointer10));
            Pointer11 = s.SerializePointer(Pointer11, name: nameof(Pointer11));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));

            //Palettes = s.DoAt(PalettesPointer, () => s.SerializeObjectArray<ARGB1555Color>(Palettes, 16 * 45, name: nameof(Palettes)));
        }
    }
}