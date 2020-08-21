namespace R1Engine
{
    /// <summary>
    /// Palette block for GBA
    /// </summary>
    public class GBA_Palette : GBA_BaseBlock {
        public uint Length { get; set; }
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s) {
            Length = s.Serialize<uint>(Length, name: nameof(Length));
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, Length, name: nameof(Palette));
        }
    }
}