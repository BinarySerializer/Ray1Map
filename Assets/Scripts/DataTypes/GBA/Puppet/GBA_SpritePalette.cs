using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Sprite palette for sprites on GBA
    /// </summary>
    public class GBA_SpritePalette : GBA_BaseBlock
    {
        public ushort Length { get; set; }
        public ushort PalOffset { get; set; }
        public BaseColor[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion <= EngineVersion.GBA_BatmanVengeance && !s.GetR1Settings().GBA_IsMilan) 
            {
                if (s.GetR1Settings().GBA_IsShanghai)
                    s.Goto(ShanghaiOffsetTable.GetPointer(0));

                Length = s.Serialize<ushort>(Length, name: nameof(Length));
                PalOffset = s.Serialize<ushort>(PalOffset, name: nameof(PalOffset));

                if (s.GetR1Settings().GBA_IsShanghai)
                    s.Goto(ShanghaiOffsetTable.GetPointer(1));

                Palette = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Palette, Length, name: nameof(Palette));
            } 
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage) 
            {
                Palette = s.SerializeObjectArray<BGRA4441Color>((BGRA4441Color[])Palette, BlockSize / 2, name: nameof(Palette));
            }
            else 
            {
                Palette = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Palette, BlockSize / 2, name: nameof(Palette));
            }
        }

        public override long GetShanghaiOffsetTableLength => Context.GetR1Settings().GBA_IsMilan ? 0 : 2;
    }
}