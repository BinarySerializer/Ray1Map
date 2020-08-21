namespace R1Engine
{
    /// <summary>
    /// Sprite palette for sprites on GBA
    /// </summary>
    public class GBA_SpritePalette : GBA_BaseBlock
    {
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<ARGB1555Color>(Palette, BlockSize / 2, name: nameof(Palette));
        }
    }
}