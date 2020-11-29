using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Vignette intro data for Rayman Advance (GBA)
    /// </summary>
    public class R1_GBA_IntroVignette : R1Serializable
    {
        #region Vignette Data

        public int Width => (256 / 8);

        public int Height => (160 / 8) * FrameCount;

        public Pointer ImageDataPointer { get; set; }

        public byte[] Unk1 { get; set; }
        
        public Pointer ImageValuesPointer { get; set; }

        public byte[] Unk2 { get; set; }

        public Pointer PalettesPointer { get; set; }

        public byte[] Unk3 { get; set; }

        public byte FrameCount { get; set; }
        
        public byte[] Unk4 { get; set; }

        #endregion

        #region Parsed from Pointers

        public byte[] ImageData { get; set; }

        public ushort[] ImageValues { get; set; }

        /// <summary>
        /// The 6 available palettes (16 colors each)
        /// </summary>
        public RGBA5551Color[] Palettes { get; set; }

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
            ImageValuesPointer = s.SerializePointer(ImageValuesPointer, name: nameof(ImageValuesPointer));
            Unk2 = s.SerializeArray<byte>(Unk2, 4, name: nameof(Unk2));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));
            Unk3 = s.SerializeArray<byte>(Unk3, 4, name: nameof(Unk3));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Unk4 = s.SerializeArray<byte>(Unk4, 3, name: nameof(Unk4));

            // Serialize data from pointers

            s.DoAt(ImageValuesPointer, () => ImageValues = s.SerializeArray<ushort>(default, Width * Height, name: nameof(ImageValues)));
            var imgDataLength = ImageValues.Select(x => BitHelpers.ExtractBits(x, 12, 0)).Max() + 1;
            s.DoAt(ImageDataPointer, () => ImageData = s.SerializeArray<byte>(ImageData, 0x20 * imgDataLength, name: nameof(ImageData)));
            s.DoAt(PalettesPointer, () => Palettes = s.SerializeObjectArray<RGBA5551Color>(Palettes, 16 * 16, name: nameof(Palettes)));
        }

        #endregion
    }
}