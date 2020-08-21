namespace R1Engine
{
    /// <summary>
    /// Sprite palette for sprites on GBA
    /// </summary>
    public class GBA_SpritePalette : GBA_BaseBlock
    {
        public ARGBColor[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                Palette = s.SerializeObjectArray<ARGB1444Color>((ARGB1444Color[])Palette, BlockSize / 2, name: nameof(Palette));
            } else {
                Palette = s.SerializeObjectArray<ARGB1555Color>((ARGB1555Color[])Palette, BlockSize / 2, name: nameof(Palette));
            }
        }
    }
}