using BinarySerializer.Nintendo.NDS;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class BitmapFile : OnyxFile
    {
        public uint CompressedDataLength { get; set; }
        public byte[] ImgData { get; set; } // 8-bpp
        public FileReference<PaletteFile> PaletteReference { get; set; }

        // Hard-coded size to fill full NDS screen
        public int Width => Constants.ScreenWidth;
        public int Height => Constants.ScreenHeight;

        public override void SerializeFile(SerializerObject s)
        {
            CompressedDataLength = s.Serialize<uint>(CompressedDataLength, name: nameof(CompressedDataLength));
            s.DoEncoded(new LZSS_16_Encoder(), () => ImgData = s.SerializeArray<byte>(ImgData, s.CurrentLength, name: nameof(ImgData)));
            PaletteReference = s.SerializeObject<FileReference<PaletteFile>>(PaletteReference, name: nameof(PaletteReference));
        }

        public override void ResolveDependencies(SerializerObject s)
        {
            PaletteReference?.Resolve(s);
        }
    }
}