using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// An offset table for GBA
    /// </summary>
    public class GBA_OffsetTable : R1Serializable
    {
        public int OffsetsCount { get; set; }
        public int[] Offsets { get; set; }

        public static List<GBA_OffsetTable> OffsetTables { get; } = new List<GBA_OffsetTable>();
        public bool[] UsedOffsets { get; set; }
        public GBA_BaseBlock Block { get; set; }

        /// <summary>
        /// Gets a pointer from the table
        /// </summary>
        /// <param name="index">The offset index</param>
        /// <returns>The pointer</returns>
        public Pointer GetPointer(int index)
        {
            UsedOffsets[index] = true;
            var pointerTable = PointerTables.GBA_PointerTable(Offset.Context.Settings.GameModeSelection, Offset.file);
            if (Context.Settings.EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                if (Block == null) {
                    return pointerTable[GBA_Pointer.UiOffsetTable] + Offsets[index];
                } else {
                    if (Block.DecompressedBlockOffset != null) {
                        return Block.DecompressedBlockOffset + Offsets[index];
                    } else {
                        return Block.Offset + Offsets[index];
                    }
                }
            } else {
                return pointerTable[GBA_Pointer.UiOffsetTable] + (Offsets[index] * 4);
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the offset table
            OffsetsCount = s.Serialize<int>(OffsetsCount, name: nameof(OffsetsCount));
            if (OffsetsCount > 0) {
                Offsets = s.SerializeArray<int>(Offsets, OffsetsCount, name: nameof(Offsets));
                UsedOffsets = new bool[OffsetsCount];
            } else {
                Offsets = new int[0];
                UsedOffsets = new bool[0];
            }
            OffsetTables.Add(this);
        }
    }
}