namespace R1Engine
{
    /// <summary>
    /// Sprite palette for sprites on GBA
    /// </summary>
    public class GBA_SpritePalette : GBA_BaseBlock
    {
        public ushort Length { get; set; }
        public ushort UShort_02 { get; set; }
        public ARGBColor[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBA_BatmanVengeance) {
                Length = s.Serialize<ushort>(Length, name: nameof(Length));
                UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));
                Palette = s.SerializeObjectArray<ARGB1555Color>((ARGB1555Color[])Palette, Length, name: nameof(Palette));
            } else if (s.GameSettings.EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                Palette = s.SerializeObjectArray<ARGB1444Color>((ARGB1444Color[])Palette, BlockSize / 2, name: nameof(Palette));
            } else {
                Palette = s.SerializeObjectArray<ARGB1555Color>((ARGB1555Color[])Palette, BlockSize / 2, name: nameof(Palette));
            }
        }
    }
}