namespace R1Engine
{
    /// <summary>
    /// Sprite palette for sprites on GBA
    /// </summary>
    public class GBA_SpritePalette : GBA_BaseBlock
    {
        public ushort Length { get; set; }
        public ushort UShort_02 { get; set; } // Only set for sprites
        public BaseColor[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion <= EngineVersion.GBA_BatmanVengeance && !s.GameSettings.GBA_IsMilan) 
            {
                if (s.GameSettings.EngineVersion <= EngineVersion.GBA_R3_MadTrax)
                    s.Goto(ShanghaiOffsetTable.GetPointer(0));

                Length = s.Serialize<ushort>(Length, name: nameof(Length));
                UShort_02 = s.Serialize<ushort>(UShort_02, name: nameof(UShort_02));

                if (s.GameSettings.EngineVersion <= EngineVersion.GBA_R3_MadTrax)
                    s.Goto(ShanghaiOffsetTable.GetPointer(1));

                Palette = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Palette, Length, name: nameof(Palette));
            } 
            else if (s.GameSettings.EngineVersion == EngineVersion.GBA_SplinterCell_NGage) 
            {
                Palette = s.SerializeObjectArray<BGRA4441Color>((BGRA4441Color[])Palette, BlockSize / 2, name: nameof(Palette));
            }
            else 
            {
                Palette = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Palette, BlockSize / 2, name: nameof(Palette));
            }
        }

        public override long GetShanghaiOffsetTableLength => Context.Settings.GBA_IsMilan ? 0 : 2;
    }
}