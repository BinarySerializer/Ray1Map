namespace R1Engine
{
    /// <summary>
    /// An offset table for GBA
    /// </summary>
    public class GBA_OffsetTable : R1Serializable
    {
        public uint OffsetsCount { get; set; }
        public uint[] Offsets { get; set; }

        /// <summary>
        /// Gets a pointer from the table
        /// </summary>
        /// <param name="index">The offset index</param>
        /// <returns>The pointer</returns>
        public Pointer GetPointer(int index) => PointerTables.GetGBAR3PointerTable(Offset.Context.Settings.GameModeSelection, Offset.file)[GBA_R3_Pointer.UiOffsetTable] + (Offsets[index] * 4);

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the offset table
            OffsetsCount = s.Serialize<uint>(OffsetsCount, name: nameof(OffsetsCount));
            Offsets = s.SerializeArray<uint>(Offsets, OffsetsCount, name: nameof(Offsets));
        }
    }
}