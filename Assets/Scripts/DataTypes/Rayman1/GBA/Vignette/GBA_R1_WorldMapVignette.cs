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
            Width = 48;
            Height = 36;

            if (s.GameSettings.EngineVersion == EngineVersion.RayGBA)
            {
                // Get the pointer table
                var pointerTable = PointerTables.GetGBAPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);

                // Hard-code properties
                ImageDataPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignetteImageData];
                BlockIndicesPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignetteBlockIndices];
                PaletteIndicesPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignettePaletteIndices];
                PalettesPointer = pointerTable[GBA_R1_ROMPointer.WorldMapVignettePalettes];

                // Serialize data from pointers
                SerializeVignette(s, false);
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.RayDSi)
            {
                // Get the pointer table
                var pointerTable = PointerTables.GetDSiPointerTable(s.GameSettings.GameModeSelection, this.Offset.file);

                // Serialize pointers
                s.DoAt(pointerTable[DSi_R1_Pointer.WorldMapVignette], () =>
                {
                    PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));
                    ImageDataPointer = s.SerializePointer(ImageDataPointer, name: nameof(ImageDataPointer));
                    BlockIndicesPointer = s.SerializePointer(BlockIndicesPointer, name: nameof(BlockIndicesPointer));
                });

                // Serialize data from pointers
                SerializeVignette(s, false);
            }
        }
    }
}