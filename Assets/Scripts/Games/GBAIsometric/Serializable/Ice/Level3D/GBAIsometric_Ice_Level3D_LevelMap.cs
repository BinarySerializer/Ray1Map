using BinarySerializer;
using BinarySerializer.Nintendo;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_LevelMap : BinarySerializable
    {
        public GBA_Palette Palette { get; set; }
        public byte[] ImgData { get; set; } // 30x20

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObject<GBA_Palette>(Palette, name: nameof(Palette));
            s.DoEncoded(new GBA_LZSSEncoder(), () => ImgData = s.SerializeArray<byte>(ImgData, 30 * 20 * 0x20, name: nameof(ImgData)));
            s.Align();
        }
    }
}