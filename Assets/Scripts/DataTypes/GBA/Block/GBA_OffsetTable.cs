using System;
using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// An offset table for GBA
    /// </summary>
    public class GBA_OffsetTable : BinarySerializable
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
        public Pointer GetPointer(int index, bool isRelativeOffset = false)
        {
            if (index >= OffsetsCount)
                throw new Exception($"Invalid offset index {index}, length is {OffsetsCount}");

            UsedOffsets[index] = true;

            var manager = (GBA_Manager)Offset.Context.GetR1Settings().GetGameManager;
            var pointerTable = PointerTables.GBA_PointerTable(Offset.Context, Context.GetFile(manager.GetROMFilePath(Context)));

            var root = pointerTable[GBA_Pointer.UiOffsetTable];

            if (manager.GetLevelType(Context) == GBA_Manager.LevelType.R3SinglePak)
                root = pointerTable[GBA_Pointer.R3SinglePak_OffsetTable];

            if (Context.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                if (Block == null) {
                    return root + Offsets[index];
                } else {
                    if (Block.DecompressedBlockOffset != null) {
                        return Block.DecompressedBlockOffset + Offsets[index];
                    } else {
                        return Block.Offset + Offsets[index];
                    }
                }
            } else {

                if (isRelativeOffset && Block != null) {
                    return Block.Offset + Offsets[index];
                } else {
                    return root + (Offsets[index] * 4);
                }
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the offset table
            if (Block != null && Block.IsGCNBlock) {
                OffsetsCount = Block.GetOffsetTableLengthGCN(s);
            } else {
                OffsetsCount = s.Serialize<int>(OffsetsCount, name: nameof(OffsetsCount));
            }
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