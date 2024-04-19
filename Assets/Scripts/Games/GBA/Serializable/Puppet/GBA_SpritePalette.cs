using BinarySerializer;

namespace Ray1Map.GBA
{
    /// <summary>
    /// Sprite palette for sprites on GBA
    /// </summary>
    public class GBA_SpritePalette : GBA_BaseBlock
    {
        public ushort Length { get; set; }
        public ushort PalOffset { get; set; }
        public SerializableColor[] Palette { get; set; }

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

                Palette = s.SerializeIntoArray<SerializableColor>(Palette, Length, BitwiseColor.RGBA5551, name: nameof(Palette));
            } 
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBA_SplinterCell_NGage) 
            {
                Palette = s.SerializeIntoArray<SerializableColor>(Palette, BlockSize / 2, BitwiseColor.BGRA4441, name: nameof(Palette));
            }
            else 
            {
                Palette = s.SerializeIntoArray<SerializableColor>(Palette, BlockSize / 2, BitwiseColor.RGBA5551, name: nameof(Palette));
            }
        }

        public override long GetShanghaiOffsetTableLength => Context.GetR1Settings().GBA_IsMilan ? 0 : 2;
    }
}