namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class SpriteTileSetFile : OnyxFile
    {
        public uint DecompressedDataLength { get; set; }
        public bool IsCompressed { get; set; }
        public uint CompressedDataLength { get; set; }
        public byte[] ImgData { get; set; } // 4 or 8-bit (determined by the animation data)

        public override void SerializeFile(SerializerObject s)
        {
            DecompressedDataLength = s.Serialize<uint>(DecompressedDataLength, name: nameof(DecompressedDataLength));
            IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));

            if (IsCompressed)
            {
                CompressedDataLength = s.Serialize<uint>(CompressedDataLength, name: nameof(CompressedDataLength));
                s.DoEncoded(new LZSS_8_Encoder(), () => 
                    ImgData = s.SerializeArray<byte>(ImgData, DecompressedDataLength, name: nameof(ImgData)));
            }
            else
            {
                ImgData = s.SerializeArray<byte>(ImgData, DecompressedDataLength, name: nameof(ImgData));
            }
        }
    }
}