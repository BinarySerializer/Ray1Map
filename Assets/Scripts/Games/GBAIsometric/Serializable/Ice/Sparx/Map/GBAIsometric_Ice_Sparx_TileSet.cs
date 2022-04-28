using BinarySerializer;
using BinarySerializer.Nintendo;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Sparx_TileSet : BinarySerializable
    {
        public int Length { get; set; }
        public int TilesCount { get; set; }
        public int TileLength { get; set; } // Always 32 bytes since it's 4bpp
        public uint CompressionHeader { get; set; } // Unused in Ice
        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<int>(Length, name: nameof(Length));
            TilesCount = s.Serialize<int>(TilesCount, name: nameof(TilesCount));
            TileLength = s.Serialize<int>(TileLength, name: nameof(TileLength));
            CompressionHeader = s.Serialize<uint>(CompressionHeader, name: nameof(CompressionHeader));

            if ((CompressionHeader & 0xf0) == 0x10)
            {
                s.Goto(s.CurrentPointer - 4);
                s.DoEncoded(new GBA_LZSSEncoder(), () => ImgData = s.SerializeArray<byte>(ImgData, Length, name: nameof(ImgData)));
            }
            else
            {
                ImgData = s.SerializeArray<byte>(ImgData, Length, name: nameof(ImgData));
            }
        }
    }
}