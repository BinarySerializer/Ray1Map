using System.IO;
using BinarySerializer;
using Ionic.Zlib;

namespace Ray1Map
{
    /// <summary>
    /// Compresses/decompresses data using Zlib
    /// </summary>
    public class ZlibEncoder : IStreamEncoder 
    {
        public ZlibEncoder(uint length, uint decompressedLength)
        {
            Length = length;
            DecompressedLength = decompressedLength;
        }

        public string Name => "Zlib";

        protected uint Length { get; }
        protected uint DecompressedLength { get; }

        public void DecodeStream(Stream input, Stream output) 
        {
            using Reader reader = new Reader(input, isLittleEndian: false, leaveOpen: true);
            byte[] bytes = reader.ReadBytes((int)Length);

            using var zlibStream = new ZlibStream(new MemoryStream(bytes), CompressionMode.Decompress);
            zlibStream.CopyTo(output);
        }

        public void EncodeStream(Stream input, Stream output)
        {
            using var compressionStream = new ZlibStream(output, CompressionMode.Compress, leaveOpen: true);
            input.CopyTo(compressionStream);
        }
    }
}