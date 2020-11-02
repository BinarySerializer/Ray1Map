using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_RHR_PaletteIndexTable : R1Serializable {
        // Set in onPreSerialize
        public uint Length { get; set; }

        public Pointer PaletteIndicesPointer { get; set; }
        public Pointer SecondaryPaletteIndicesPointer { get; set; }
        public Pointer SecondaryPaletteTileIndicesPointer { get; set; } // For tiles that can have multiple palettes defined. Like water

        // Parsed
        public byte[] PaletteIndices { get; set; }
        public byte[] SecondaryPaletteIndices { get; set; }
        public ushort[] SecondaryTileIndices { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            PaletteIndicesPointer = s.SerializePointer(PaletteIndicesPointer, name: nameof(PaletteIndicesPointer));
            SecondaryPaletteIndicesPointer = s.SerializePointer(SecondaryPaletteIndicesPointer, name: nameof(SecondaryPaletteIndicesPointer));
            SecondaryPaletteTileIndicesPointer = s.SerializePointer(SecondaryPaletteTileIndicesPointer, name: nameof(SecondaryPaletteTileIndicesPointer));

            s.DoAt(PaletteIndicesPointer, () => {
                PaletteIndices = s.SerializeArray<byte>(PaletteIndices, Length, name: nameof(PaletteIndices));
            });
            s.DoAt(SecondaryPaletteIndicesPointer, () => {
                long length = SecondaryPaletteTileIndicesPointer - SecondaryPaletteIndicesPointer; // hack
                SecondaryPaletteIndices = s.SerializeArray<byte>(SecondaryPaletteIndices, length, name: nameof(SecondaryPaletteIndices));
            });
            s.DoAt(SecondaryPaletteTileIndicesPointer, () => {
                SecondaryTileIndices = s.SerializeArray<ushort>(SecondaryTileIndices, SecondaryPaletteIndices.Length, name: nameof(SecondaryTileIndices));
            });
        }

        public int GetMaxPaletteIndex() {
            int max = PaletteIndices.Where(x => x != 0xFF).Max();
            if (SecondaryPaletteIndices != null) {
                int max2 = SecondaryPaletteIndices.Where(x => x != 0xFF).Max();
                if(max2 > max) max = max2;
            }
            return max;
        }
    }
}