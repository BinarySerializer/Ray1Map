using R1Engine;

namespace BinarySerializer.Ray1
{
    /// <summary>
    /// Vignette world map data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_WorldMapVignette : GBA_BaseVignette
    {
        protected override int PaletteCount => 16;

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            var settings = s.GetSettings<Ray1Settings>();

            Width = 48;
            Height = 36;

            if (settings.EngineVersion == Ray1EngineVersion.R1_GBA)
            {
                // Get the pointer table
                var pointerTable = PointerTables.R1_GBA_PointerTable(s.GetR1Settings().GameModeSelection, this.Offset.File);

                // Hard-code properties
                ImageDataPointer = pointerTable[R1_GBA_ROMPointer.WorldMapVignetteImageData];
                BlockIndicesPointer = pointerTable[R1_GBA_ROMPointer.WorldMapVignetteBlockIndices];
                PaletteIndicesPointer = pointerTable[R1_GBA_ROMPointer.WorldMapVignettePaletteIndices];
                PalettesPointer = pointerTable[R1_GBA_ROMPointer.WorldMapVignettePalettes];

                // Serialize data from pointers
                SerializeVignette(s, false);
            }
            else if (settings.EngineVersion == Ray1EngineVersion.R1_DSi)
            {
                // Get the pointer table
                var pointerTable = PointerTables.R1_DSi_PointerTable(s.GetR1Settings().GameModeSelection, this.Offset.File);

                // Serialize pointers
                s.DoAt(pointerTable[R1_DSi_Pointer.WorldMapVignette], () =>
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