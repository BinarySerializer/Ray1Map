using BinarySerializer;
using BinarySerializer.GBA;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_SpriteSet : BinarySerializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public bool IsInitialized { get; set; } // Only used for when loaded in memory
        public bool Is8Bit { get; set; } // Never used by the game
        public bool IsMulti { get; set; }
        public CompressionType Compression { get; set; }
        public ushort SpritesCount { get; set; }
        public ushort TilesCount { get; set; }
        public ushort SpriteMapLength { get; set; } // In bits

        public GBAIsometric_Ice_Sprite[] Sprites { get; set; }
        public bool[] SpriteMaps { get; set; }
        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            s.DoBits<byte>(b =>
            {
                IsInitialized = b.SerializeBits<bool>(IsInitialized, 1, name: nameof(IsInitialized));
                Is8Bit = b.SerializeBits<bool>(Is8Bit, 1, name: nameof(Is8Bit));
                IsMulti = b.SerializeBits<bool>(IsMulti, 1, name: nameof(IsMulti));
                Compression = b.SerializeBits<CompressionType>(Compression, 2, name: nameof(Compression));
                b.SerializePadding(3, logIfNotNull: true);
            });
            s.SerializeMagicString("CRS", 3);
            SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));
            TilesCount = s.Serialize<ushort>(TilesCount, name: nameof(TilesCount));
            SpriteMapLength = s.Serialize<ushort>(SpriteMapLength, name: nameof(SpriteMapLength));

            IStreamEncoder encoder = null;

            switch (Compression)
            {
                case CompressionType.LZ77:
                    encoder = new GBA_LZSSEncoder();
                    break;

                case CompressionType.Huff:
                    encoder = new GBA_Huffman4Encoder();
                    break;
            }

            s.DoEncodedIf(encoder, encoder != null, () =>
            {
                Sprites = s.SerializeObjectArray<GBAIsometric_Ice_Sprite>(Sprites, IsMulti ? SpritesCount : 1, name: nameof(Sprites));
                s.Align();
                s.SerializeBitValues(bitFunc =>
                {
                    SpriteMaps ??= new bool[Sprites.Length * SpriteMapLength];

                    for (int i = 0; i < SpriteMaps.Length; i++)
                        SpriteMaps[i] = bitFunc(SpriteMaps[i] ? 1 : 0, 1, name: $"{nameof(SpriteMaps)}[{i}]") != 0;
                });
                s.Align();
                ImgData = s.SerializeArray<byte>(ImgData, TilesCount * (Is8Bit ? 0x40 : 0x20), name: nameof(ImgData));
            });
        }

        public enum CompressionType : byte
        {
            None = 0,
            LZ77 = 1,
            Huff = 2,
        }
    }
}