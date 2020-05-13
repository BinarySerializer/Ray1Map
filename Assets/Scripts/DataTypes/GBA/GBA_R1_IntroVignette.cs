namespace R1Engine
{
    /// <summary>
    /// Vignette intro data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_IntroVignette : R1Serializable
    {
        #region Vignette Data

        public Pointer ImageDataPointer { get; set; }

        public byte[] Unk1 { get; set; }
        
        public Pointer UnkP2 { get; set; }

        public byte[] Unk2 { get; set; }

        public Pointer PalettesPointer { get; set; }

        public byte[] Unk3 { get; set; }

        public byte[] Unk4 { get; set; }

        #endregion

        #region Parsed from Pointers

        public byte[] ImageData { get; set; }

        /// <summary>
        /// The 6 available palettes (16 colors each)
        /// </summary>
        public ARGB1555Color[] Palettes { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize data
            ImageDataPointer = s.SerializePointer(ImageDataPointer, name: nameof(ImageDataPointer));
            Unk1 = s.SerializeArray<byte>(Unk1, 4, name: nameof(Unk1));
            UnkP2 = s.SerializePointer(UnkP2, name: nameof(UnkP2));
            Unk2 = s.SerializeArray<byte>(Unk2, 4, name: nameof(Unk2));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));
            Unk3 = s.SerializeArray<byte>(Unk3, 4, name: nameof(Unk3));
            Unk4 = s.SerializeArray<byte>(Unk4, 4, name: nameof(Unk4));

            // Serialize data from pointers
            //s.DoAt(ImageDataPointer, () => ImageData = s.SerializeArray<byte>(ImageData, 0x20 * Width * Height, name: nameof(ImageData)));
            s.DoAt(PalettesPointer, () => Palettes = s.SerializeObjectArray<ARGB1555Color>(Palettes, 16 * 16, name: nameof(Palettes)));
        }

        #endregion
    }
}