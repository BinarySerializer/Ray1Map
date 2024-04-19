using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_PaletteBlock : GBAVV_BaseBlock
    {
        public SerializableColor[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            Palette = s.SerializeIntoArray<SerializableColor>(Palette, BlockLength / 2, BitwiseColor.RGBA5551, name: nameof(Palette));
        }
    }
}