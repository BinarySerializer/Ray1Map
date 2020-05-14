namespace R1Engine
{
    /// <summary>
    /// Vignette world map data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_WorldMapVignette : GBA_R1_BaseVignette
    {
        protected override int PaletteCount => 16;

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Hard-code properties

            ImageDataPointer = new Pointer(0x08145208, this.Offset.file);
            BlockIndicesPointer = new Pointer(0x08151468, this.Offset.file);
            PaletteIndicesPointer = new Pointer(0x081521E8, this.Offset.file);
            PalettesPointer = new Pointer(0x081528A8, this.Offset.file);

            Width = 48;
            Height = 36;


            // Serialize data from pointers

            base.SerializeImpl(s);
        }
    }
}