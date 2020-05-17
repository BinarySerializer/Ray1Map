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
            // Get the pointer table
            var pointerTable = PointerTables.GetGBAPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);


            // Hard-code properties

            ImageDataPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignetteImageData];
            BlockIndicesPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignetteBlockIndices];
            PaletteIndicesPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignettePaletteIndices];
            PalettesPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignettePalettes];

            Width = 48;
            Height = 36;


            // Serialize data from pointers

            base.SerializeImpl(s);
        }
    }
}