namespace R1Engine
{
    /// <summary>
    /// BigRay data for Rayman 1 (PS1)
    /// </summary>
    public class R1_PS1_BigRayFile : R1_PS1BaseFile
    {
        #region File Pointers

        public Pointer DataBlockPointer => BlockPointers[0];

        public Pointer TextureBlockPointer => BlockPointers[1];

        public Pointer Palette1Pointer => BlockPointers[2];

        public Pointer Palette2Pointer => BlockPointers[3];

        #endregion

        #region Block Data

        public R1_PS1_BigRayBlock BigRayData { get; set; }

        /// <summary>
        /// The texture block
        /// </summary>
        public byte[] TextureBlock { get; set; }

        public ARGB1555Color[] Palette1 { get; set; }

        public ARGB1555Color[] Palette2 { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // HEADER
            base.SerializeImpl(s);

            // DATA BLOCK
            s.DoAt(DataBlockPointer, () => BigRayData = s.SerializeObject<R1_PS1_BigRayBlock>(BigRayData, x => x.Length = TextureBlockPointer - s.CurrentPointer, name: nameof(BigRayData)));

            // TEXTURE BLOCK
            s.DoAt(TextureBlockPointer, () => TextureBlock = s.SerializeArray<byte>(TextureBlock, Palette1Pointer - s.CurrentPointer, name: nameof(TextureBlock)));

            // PALETTE 1
            s.DoAt(Palette1Pointer, () => Palette1 = s.SerializeObjectArray<ARGB1555Color>(Palette1, 256, name: nameof(Palette1)));

            // PALETTE 2
            s.DoAt(Palette2Pointer, () => Palette2 = s.SerializeObjectArray<ARGB1555Color>(Palette2, 256, name: nameof(Palette2)));
        }

        #endregion
    }
}